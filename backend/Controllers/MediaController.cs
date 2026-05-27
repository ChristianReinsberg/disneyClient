using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DisneyApi.Data;
using DisneyApi.Models;
using DisneyApi.DTOs;
using Microsoft.Extensions.Caching.Memory;

namespace DisneyApi.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class MediaController: ControllerBase
    {
        private readonly AppDbContext _context;
        private IMemoryCache _cache;

        public MediaController (AppDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [HttpGet("all")]
        public async Task<ActionResult<PagedResult<Media>>> GetMedia(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50
        )
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 || pageSize > 100 ? 50 : pageSize;
            int totalItems = await _context.Medias.CountAsync();
            var items = await _context.Medias
            .AsNoTracking()
            .Include(m => m.Characters)
            .OrderBy(m => m.Name)
            .Skip(page - 1 * pageSize)
            .Take(pageSize)
            .ToListAsync();
            var result = new PagedResult<Media>
            {
                Items = items,
                PageNumber = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };
            
            return Ok(result);
        }

        [HttpGet("movies")]
        public async Task<ActionResult<PagedResult<Media>>> GetMovies(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50
        )
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 || pageSize > 100 ? 50 : pageSize;
            int totalItems = await _context.Medias.Where(m => m.MediaType == "Movie").CountAsync();
            var items = await _context.Medias
            .Where(m => m.MediaType == "Movie")
            .AsNoTracking()
            .Include(m => m.Characters)
            .OrderBy(m => m.Name)
            .Skip(page - 1 * pageSize)
            .Take(pageSize)
            .ToListAsync();
            var result = new PagedResult<Media>
            {
                Items = items,
                PageNumber = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };
            
            return Ok(result);
        }

        [HttpGet("series")]
        public async Task<ActionResult<PagedResult<Media>>> GetSeries(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50
        )
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 || pageSize > 100 ? 50 : pageSize;
            int totalItems = await _context.Medias.Where(m => m.MediaType == "TV").CountAsync();
            var items = await _context.Medias
            .Where(m => m.MediaType == "TV")
            .AsNoTracking()
            .Include(m => m.Characters)
            .OrderBy(m => m.Name)
            .Skip(page - 1 * pageSize)
            .Take(pageSize)
            .ToListAsync();
            var result = new PagedResult<Media>
            {
                Items = items,
                PageNumber = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };
            
            return Ok(result);
        }

        [HttpGet("movie/{id}")]
        public async Task<IActionResult> GetMovie(int id) {
            string cacheKey = $"movie_{id}";
            if (!_cache.TryGetValue(cacheKey, out MediaDetailsDto? movie)) {
                movie = await FetchMedia(id, "Movie");
                if (movie == null)
                {
                    return NotFound();
                }
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(30))
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                _cache.Set(cacheKey, movie, cacheOptions);
            }

            return Ok(movie);
        }

        [HttpGet("series/{id}")]
        public async Task<IActionResult> GetSeries(int id) {
            string cacheKey = $"series_{id}";
            if (!_cache.TryGetValue(cacheKey, out MediaDetailsDto? series)) {
                series = await FetchMedia(id, "TV");
                if (series == null)
                {
                    return NotFound();
                }
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(30))
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                _cache.Set(cacheKey, series, cacheOptions);
            }

            return Ok(series);
        }

        public async Task<MediaDetailsDto> FetchMedia(int id, string mediaType)
        {
            var media = await _context.Medias
                .Include(m => m.Characters)
                .FirstOrDefaultAsync(m => m.Id == id && m.MediaType == mediaType);
            if (media == null)
            {
                return null;
            }
            return new MediaDetailsDto
            {
                Id = media.Id,
                Title = media.Name,
                PosterPath = media.PosterPath,
                MediaType = media.MediaType,
                ReleaseDate = media.ReleaseDate,
                VoteAvg = media.VoteAvg,
                VoteCount = media.VoteCount,
                Characters = media.Characters.Select(c => new FlatCharacterDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    ImageUrl = c.ImageUrl
                }).ToList()
            };
        }
    }
}