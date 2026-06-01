using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;
using System;
using DisneyApi.Data;
using Microsoft.EntityFrameworkCore;

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
                var uniqueSeries = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                try
                {
                    _logger.LogInformation("crawler process running...");
                    for(var i = 1; i < await _disneyService.GetPageCount(); i++)
                    {
                        await _disneyService.GetCharacters(i, 50);
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