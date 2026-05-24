using DisneyApi.DTOs;

namespace DisneyApi.Services
{
    public class TmdbService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public TmdbService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<List<TmdbResult>> GetTmdbMovieAsync(string title)
        {
            var client = _httpClientFactory.CreateClient("tmdbClient");
            var escapedTitle = Uri.EscapeDataString(title.IndexOf('(') > -1 ? title[0..(title.IndexOf('(') - 1)] : title);
            var url = $"search/movie?query={escapedTitle}";

            try
            {
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var tmdbResponse = await response.Content.ReadFromJsonAsync<TmdbData>();
                return tmdbResponse?.Results ?? [];
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TMDBService Movie Error] {ex.Message}");
                return [];
            }
        }

        public async Task <List<TmdbResult>> GetTmdbSeriesAsync(string title)
        {
            var client = _httpClientFactory.CreateClient("tmdbClient");
            var escapedTitle = Uri.EscapeDataString(title.IndexOf('(') > -1 ? title[0..(title.IndexOf('(') - 1)] : title);
            var url = $"search/tv?query={Uri.EscapeDataString(escapedTitle)}";

            try
            {
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var tmdbResponse = await response.Content.ReadFromJsonAsync<TmdbData>();
                return tmdbResponse?.Results ?? [];
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TMDBService Series Error] {ex.Message}");
                return [];
            }
        }
    }
}