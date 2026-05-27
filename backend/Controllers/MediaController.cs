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
        public async Task<ActionResult<IEnumerable<Media>>> GetMedia()
        {
            return await _context.Medias.ToListAsync();
        }

        [HttpGet("movies")]
        public async Task<ActionResult<IEnumerable<Media>>> GetMovies()
        {
            return await _context.Medias.Where(m => m.MediaType == "Movie").ToListAsync();
        }

        [HttpGet("series")]
        public async Task<ActionResult<IEnumerable<Media>>> GetSeries()
        {
            return await _context.Medias.Where(m => m.MediaType == "TV").ToListAsync();
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