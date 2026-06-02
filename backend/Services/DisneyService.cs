using System.Text.Json;
using DisneyApi.Data;
using DisneyApi.DTOs;
using DisneyApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DisneyApi.Services
{
    public class DisneyService
    {
        private IHttpClientFactory _httpClientFactory;
        private TmdbService _tmdbService;
        private AppDbContext _context;

        public DisneyService(IHttpClientFactory httpClientFactory, TmdbService tmdbService, AppDbContext context)
        {
            _httpClientFactory = httpClientFactory;
            _tmdbService = tmdbService;
            _context = context;
        }

        public async Task AddCharacter(Character character)
        {
            var existingChar = await _context.Characters
                .Include(c => c.Medias)
                .FirstOrDefaultAsync(c => c.Id == character.Id);
            bool isNewChar = false;
            if (existingChar == null)
            {
                isNewChar = true;
                existingChar = new Character
                {
                    Id = character.Id,
                    Name = character.Name,
                    ImageUrl = character.ImageUrl,
                    Films = character.Films,
                    ShortFilms = character.ShortFilms,
                    TvShows = character.TvShows,
                    Medias = new List<Media>()
                };
            }
            if (isNewChar)
            {
                _context.Characters.Add(existingChar);
            }
        }

        public async Task AddMedia(IEnumerable<string> medias, string mediaType, Character existingChar)
        {
            foreach (var title in medias)
            {
                var tmdbData = await _tmdbService.GetTmdbMovieAsync(title);
                await Task.Delay(50);
                var data = tmdbData?.FirstOrDefault();

                if (data != null)
                {
                    var existingMedia = _context.Medias.Local.FirstOrDefault(m => m.Id == data.Id && m.MediaType == mediaType);
                    if (existingMedia == null)
                    {
                        await _context.Medias
                        .FirstOrDefaultAsync(m => m.Id == data.Id && m.MediaType == mediaType);
                    }
                    if (existingMedia == null)
                    {
                        existingMedia = new Media
                        {
                            Id = data.Id,
                            MediaType = mediaType,
                            Name = !string.IsNullOrEmpty(data.Name) ? data.Name : data.Title,
                            Overview = data.Overview,
                            PosterPath = data.Poster_Path,
                            ReleaseDate = mediaType == "Movie" ?  data.Release_Date : data.First_Air_Date,
                            VoteAvg = data.Vote_Average,
                            VoteCount = data.Vote_Count
                        };
                        _context.Medias.Add(existingMedia);
                    }
                    if (existingMedia != null && !existingChar.Medias.Contains(existingMedia))
                    {
                        existingChar.Medias.Add(existingMedia);
                    }
                }
            }
        }

        public async Task GetCharacters(int page = 1, int pageSize = 50)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("disneyClient");
                var url = $"character?page={page}&pageSize={pageSize}";

                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var rawJson = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions {PropertyNameCaseInsensitive = true};
                List<DisneyResult> finalCharacters = [];

                using (JsonDocument doc = JsonDocument.Parse(rawJson))
                {
                    var root = doc.RootElement;
                    if (root.TryGetProperty("data", out JsonElement dataElement))
                    {
                        if (dataElement.ValueKind == JsonValueKind.Array)
                        {
                            var listData = JsonSerializer.Deserialize<List<DisneyResult>>(dataElement.GetRawText(), options);
                            if (listData != null)
                            {
                                finalCharacters = listData;
                            }
                        }
                        else if (dataElement.ValueKind == JsonValueKind.Object)
                        {
                            var singleData = JsonSerializer.Deserialize<DisneyResult>(dataElement.GetRawText(), options);
                            if (singleData != null)
                            {
                                finalCharacters.Add(singleData);
                            }
                        }
                    }
                }
                if (finalCharacters.Any())
                {
                    foreach (var chara in finalCharacters)
                    {
                        await AddCharacter(new Character
                        {
                            Id = chara.Id,
                            Name = chara.Name,
                            ImageUrl = chara.ImageUrl,
                            Films = chara.Films,
                            ShortFilms = chara.ShortFilms,
                            TvShows = chara.TvShows
                        });
                    }
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[API Error] {ex.Message}");
            }
        }

        public async Task<int> GetPageCount()
        {
            var client = _httpClientFactory.CreateClient("disneyClient");
            var url = $"character?page=1&pageSize=50";

            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var rawJson = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions {PropertyNameCaseInsensitive = true};
            var pages = new DisneyPages();

            using (JsonDocument doc = JsonDocument.Parse(rawJson))
            {
                var root = doc.RootElement;
                if (root.TryGetProperty("info", out JsonElement infoElement))
                {
                    var listData = JsonSerializer.Deserialize<DisneyPages>(infoElement.GetRawText(), options);
                    pages = listData;
                }
            }
            return pages.TotalPages;
        }
    }
}