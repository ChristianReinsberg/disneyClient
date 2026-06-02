using DisneyApi.Data;
using Microsoft.EntityFrameworkCore;
using DisneyApi.Models;

namespace DisneyApi.Services
{
    public class DisneyCrawlerService: BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<DisneyCrawlerService> _logger;
        private readonly DisneyService _disneyService;

        public DisneyCrawlerService(IServiceScopeFactory scopeFactory, ILogger<DisneyCrawlerService> logger, DisneyService disneyService)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _disneyService = disneyService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Disney Crawler started...");
            await DoCrawlingWorkAsync();

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
                _logger.LogInformation("start daily check for changes...");
                await DoCrawlingWorkAsync();
            }
        }

        private async Task DoCrawlingWorkAsync()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var disneyService = scope.ServiceProvider.GetRequiredService<DisneyService>();
                var tmdbService = scope.ServiceProvider.GetRequiredService<TmdbService>();

                var uniqueMovies = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var uniqueShorts = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var uniqueSeries = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                try
                {
                    _logger.LogInformation("crawler process running...");
                    _logger.LogInformation("Step 1: get all Characters...");
                    for(var i = 1; i <= await _disneyService.GetPageCount(); i++)
                    {
                        await _disneyService.GetCharacters(i, 50);
                    }
                    var characters = context.Characters.ToList();
                    foreach(var chara in characters)
                    {
                        foreach(var movie in chara.Films)
                        {
                            if (!string.IsNullOrWhiteSpace(movie))
                            {
                                uniqueMovies.Add(movie.Trim());
                            }
                            
                        }
                        foreach(var shorts in chara.ShortFilms)
                        {
                            if (!string.IsNullOrWhiteSpace(shorts))
                            {
                                uniqueShorts.Add(shorts.Trim());
                            }
                        }
                        foreach(var series in chara.TvShows)
                        {
                            if (!string.IsNullOrWhiteSpace(series))
                            {
                                uniqueSeries.Add(series.Trim());
                            }
                        }
                    }
                    _logger.LogInformation($"Step 2: save {uniqueMovies.Count} unique movies");
                    foreach(var title in uniqueMovies)
                    {
                        Media? mediaEntity = context.Medias.Local.FirstOrDefault(t => t.Name.ToLower() == title.ToLower() && t.MediaType == "Movie");
    
                        if (mediaEntity == null)
                        {
                            mediaEntity = await context.Medias
                                .Include(m => m.Characters) 
                                .FirstOrDefaultAsync(t => t.Name.ToLower() == title.ToLower() && t.MediaType == "Movie");
                        }

                        if (mediaEntity == null)
                        {
                            var tmdbData = await tmdbService.GetTmdbMovieAsync(title);
                            var data = tmdbData?.FirstOrDefault();
                            
                            if (data != null)
                            {
                                mediaEntity = context.Medias.Local.FirstOrDefault(m => m.Id == data.Id && m.MediaType == "Movie")
                                    ?? await context.Medias.Include(m => m.Characters).FirstOrDefaultAsync(m => m.Id == data.Id && m.MediaType == "Movie");

                                if (mediaEntity == null)
                                {
                                    mediaEntity = new Media
                                    {
                                        Id = data.Id,
                                        MediaType = "Movie",
                                        Name = !string.IsNullOrEmpty(data.Name) ? data.Name : data.Title,
                                        Overview = data.Overview,
                                        PosterPath = data.Poster_Path,
                                        ReleaseDate = data.Release_Date,
                                        VoteAvg = data.Vote_Average,
                                        VoteCount = data.Vote_Count,
                                        Characters = new List<Character>()
                                    };
                                    context.Medias.Add(mediaEntity);
                                }
                            }
                        }

                        if (mediaEntity != null)
                        {
                            var matchingCharacters = characters
                                .Where(c => c.Films.Any(f => f.Equals(title, StringComparison.OrdinalIgnoreCase)))
                                .ToList();

                            foreach (var chara in matchingCharacters)
                            {
                                if (!mediaEntity.Characters.Any(c => c.Id == chara.Id))
                                {
                                    mediaEntity.Characters.Add(chara);
                                }
                            }

                            await context.SaveChangesAsync();
                        }
                    }
                    _logger.LogInformation($"Step 3: save {uniqueShorts.Count} unique shorts");
                    foreach(var title in uniqueShorts)
                    {
                        Media? mediaEntity = context.Medias.Local.FirstOrDefault(t => t.Name.ToLower() == title.ToLower() && t.MediaType == "Short");
                        
                        if (mediaEntity == null)
                        {
                            mediaEntity = await context.Medias
                                .Include(m => m.Characters) 
                                .FirstOrDefaultAsync(t => t.Name.ToLower() == title.ToLower() && t.MediaType == "Short");
                        }

                        if (mediaEntity == null)
                        {
                            var tmdbData = await tmdbService.GetTmdbMovieAsync(title);
                            var data = tmdbData?.FirstOrDefault();
                            
                            if (data != null)
                            {
                                mediaEntity = context.Medias.Local.FirstOrDefault(m => m.Id == data.Id && m.MediaType == "Short")
                                    ?? await context.Medias.Include(m => m.Characters).FirstOrDefaultAsync(m => m.Id == data.Id && m.MediaType == "Short");

                                if (mediaEntity == null)
                                {
                                    mediaEntity = new Media
                                    {
                                        Id = data.Id,
                                        MediaType = "Short",
                                        Name = !string.IsNullOrEmpty(data.Name) ? data.Name : data.Title,
                                        Overview = data.Overview,
                                        PosterPath = data.Poster_Path,
                                        ReleaseDate = data.Release_Date,
                                        VoteAvg = data.Vote_Average,
                                        VoteCount = data.Vote_Count,
                                        Characters = new List<Character>()
                                    };
                                    context.Medias.Add(mediaEntity);
                                }
                            }
                        }

                        if (mediaEntity != null)
                        {
                            var matchingCharacters = characters
                                .Where(c => c.ShortFilms.Any(f => f.Equals(title, StringComparison.OrdinalIgnoreCase)))
                                .ToList();

                            foreach (var chara in matchingCharacters)
                            {
                                if (!mediaEntity.Characters.Any(c => c.Id == chara.Id))
                                {
                                    mediaEntity.Characters.Add(chara);
                                }
                            }

                            await context.SaveChangesAsync();
                        }
                    }
                    _logger.LogInformation($"Step 4: save {uniqueSeries.Count} unique series");
                    foreach(var title in uniqueSeries)
                    {
                        Media? mediaEntity = context.Medias.Local.FirstOrDefault(t => t.Name.ToLower() == title.ToLower() && t.MediaType == "TV");
                        
                        if (mediaEntity == null)
                        {
                            mediaEntity = await context.Medias
                                .Include(m => m.Characters) 
                                .FirstOrDefaultAsync(t => t.Name.ToLower() == title.ToLower() && t.MediaType == "TV");
                        }

                        if (mediaEntity == null)
                        {
                            var tmdbData = await tmdbService.GetTmdbMovieAsync(title);
                            var data = tmdbData?.FirstOrDefault();
                            
                            if (data != null)
                            {
                                mediaEntity = context.Medias.Local.FirstOrDefault(m => m.Id == data.Id && m.MediaType == "TV")
                                    ?? await context.Medias.Include(m => m.Characters).FirstOrDefaultAsync(m => m.Id == data.Id && m.MediaType == "TV");

                                if (mediaEntity == null)
                                {
                                    mediaEntity = new Media
                                    {
                                        Id = data.Id,
                                        MediaType = "TV",
                                        Name = !string.IsNullOrEmpty(data.Name) ? data.Name : data.Title,
                                        Overview = data.Overview,
                                        PosterPath = data.Poster_Path,
                                        ReleaseDate = data.Release_Date,
                                        VoteAvg = data.Vote_Average,
                                        VoteCount = data.Vote_Count,
                                        Characters = new List<Character>()
                                    };
                                    context.Medias.Add(mediaEntity);
                                }
                            }
                        }

                        if (mediaEntity != null)
                        {
                            var matchingCharacters = characters
                                .Where(c => c.TvShows.Any(f => f.Equals(title, StringComparison.OrdinalIgnoreCase)))
                                .ToList();

                            foreach (var chara in matchingCharacters)
                            {
                                if (!mediaEntity.Characters.Any(c => c.Id == chara.Id))
                                {
                                    mediaEntity.Characters.Add(chara);
                                }
                            }

                            await context.SaveChangesAsync();
                        }
                    }
                    _logger.LogInformation("Crawler finished and DB syncronized");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error crawling characters: {ex.Message}");
                }
            }
        }
    }
}