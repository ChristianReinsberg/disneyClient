using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DisneyApi.Data;
using DisneyApi.Models;
using Microsoft.Extensions.Caching.Memory;
using DisneyApi.DTOs;
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
        private DisneyService _disneyService;

        public CharacterController(AppDbContext context, IMemoryCache cache, IHttpClientFactory httpClientFactory, DisneyService disneyService)
        {
            _context = context;
            _cache = cache;
            _httpClient = httpClientFactory;
            _disneyService = disneyService;
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
                await _disneyService.GetCharacters(page, pageSize);
                totalItems = await _context.Characters.CountAsync();
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
            await _disneyService.AddCharacter(character);
            return Ok(new {message = "Charakter added"});
        }

        public async Task<CharacterDetailsDto> FetchCharacter(int id)
        {
            var character = await _context.Characters
                .Include(c => c.Medias)
                .FirstOrDefaultAsync(c => c.Id == id);
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
                    PosterPath = m.PosterPath,
                    VoteAvg = m.VoteAvg,
                    VoteCount = m.VoteCount
                }).ToList()
            };
            return result;
        }
    }
}