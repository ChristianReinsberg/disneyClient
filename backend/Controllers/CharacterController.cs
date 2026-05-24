using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DisneyApi.Data;
using DisneyApi.Models;
using Microsoft.Extensions.Caching.Memory;
using DisneyApi.DTOs;
using System.Text.Json;
using DisneyApi.Services;

namespace DisneyApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CharacterController: ControllerBase
    {
        private readonly AppDbContext _context;
        private IMemoryCache _cache;
        private IHttpClientFactory _httpClient;
        private TmdbService _tmdbService;

        public CharacterController(AppDbContext context, IMemoryCache cache, IHttpClientFactory httpClientFactory, TmdbService tmdbService)
        {
            _context = context;
            _cache = cache;
            _httpClient = httpClientFactory;
            _tmdbService = tmdbService;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<Character>>> GetCharacters(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50
        )
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 || pageSize > 100 ? 50 : pageSize;
            int totalItems = await _context.Characters.CountAsync();
            if (totalItems < page * pageSize)
            {
                try
                {
                    var client = _httpClient.CreateClient("disneyClient");
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
                        totalItems = await _context.Characters.CountAsync();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[API Error] {ex.Message}");
                }
            }

            var items = await _context.Characters
                .AsNoTracking()
                .Include(c => c.Medias)
                .OrderBy(c => c.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            var result = new PagedResult<Character>
            {
                Items = items,
                PageNumber = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCharacter(int id)
        {
            string cacheKey = $"char_{id}";
            if (!_cache.TryGetValue(cacheKey, out CharacterDetailsDto? character))
            {
                character = (CharacterDetailsDto)await FetchCharacter(id);

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(30))
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                _cache.Set(cacheKey, character, cacheOptions);
            }
            return Ok(character);
        }

        [HttpPost]
        public async Task<IActionResult> AddCharacter([FromBody] Character character)
        {
            if (character == null)
            {
                return BadRequest("data missing");
            }
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

            var films = character.Films.Concat(character.ShortFilms);
            foreach (var title in films)
            {
                var tmdbData = await _tmdbService.GetTmdbMovieAsync(title);
                var data = tmdbData?.FirstOrDefault();

                if (data != null)
                {
                    var existingMedia = _context.Medias.Local.FirstOrDefault(m => m.Id == data.Id && m.MediaType == "Movie");
                    if (existingMedia == null)
                    {
                        await _context.Medias
                        .FirstOrDefaultAsync(m => m.Id == data.Id && m.MediaType == "Movie");
                    }
                    if (existingMedia == null)
                    {
                        existingMedia = new Media
                        {
                            Id = data.Id,
                            MediaType = "Movie",
                            Name = !string.IsNullOrEmpty(data.Name) ? data.Name : data.Title,
                            Overview = data.Overview,
                            PosterPath = data.Poster_Path,
                            ReleaseDate = data.Release_Date,
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
            var series = character.TvShows;
            foreach (var title in series)
            {
                var tmdbData = await _tmdbService.GetTmdbSeriesAsync(title);
                var data = tmdbData.FirstOrDefault();

                if (data != null)
                {
                    var existingMedia = _context.Medias.Local.FirstOrDefault(m => m.Id == data.Id && m.MediaType == "TV");
                    if (existingMedia == null)
                    {
                        existingMedia = await _context.Medias
                        .FirstOrDefaultAsync(m => m.Id == data.Id && m.MediaType == "TV");
                    }
                    
                    if (existingMedia == null)
                    {
                        existingMedia = new Media
                        {
                            Id = data.Id,
                            MediaType = "TV",
                            Name = !string.IsNullOrEmpty(data.Name) ? data.Name : data.Title,
                            Overview = data.Overview,
                            PosterPath = data.Poster_Path,
                            ReleaseDate = data.First_Air_Date,
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
            if (isNewChar)
            {
                _context.Characters.Add(existingChar);
            }
            return Ok(new {message = "Charakter added"});
        }

        public async Task<IActionResult> FetchCharacter(int id)
        {
            var character = await _context.Characters
                .Include(c => c.Medias)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (character == null)
            {
                return NotFound();
            }
            var result = new CharacterDetailsDto
            {
                Id = character.Id,
                Name = character.Name,
                ImageUrl = character.ImageUrl,
                Medias = character.Medias.Select(m => new FlatMediaDto
                {
                    Id = m.Id,
                    Title = m.Name,
                    MediaType = m.MediaType,
                    PosterPath = m.PosterPath
                }).ToList()
            };
            return Ok(result);
        }
    }
}