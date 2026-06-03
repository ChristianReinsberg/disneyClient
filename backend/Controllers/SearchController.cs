using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DisneyApi.Data;
using DisneyApi.DTOs;

namespace DisneyApi.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class SearchController: ControllerBase
    {
        private readonly AppDbContext _context;

        public SearchController (AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<SuggestDTO>> GetAutofill([FromQuery] string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return Ok(new SuggestDTO{ Suggestions = [] });
            }
            var searchTerm = term.ToLower().Trim();
            var spaceSearchTerm = " " + term;
            var characters = await _context.Characters
            .Where(c => c.Name.ToLower().StartsWith(searchTerm) || c.Name.ToLower().Contains(spaceSearchTerm))
            .OrderBy(c => c.Name)
            .ToListAsync();
            var medias = await _context.Medias
            .Where(m => m.Name.ToLower().StartsWith(searchTerm) || m.Name.ToLower().Contains(spaceSearchTerm))
            .OrderBy(m => m.Name)
            .ToListAsync();
            var suggestions = new List<SuggestionData>();
            foreach(var chara in characters)
            {
                suggestions.Add(new SuggestionData
                {
                    Name = chara.Name,
                    Type = "Character",
                    Id = chara.Id
                });
            }
            foreach(var media in medias)
            {
                suggestions.Add(new SuggestionData
                {
                    Name = media.Name,
                    Type = media.MediaType,
                    Id = media.Id
                });
            }
            var result = new SuggestDTO
            {
                Suggestions = suggestions
            };
            return Ok(result);
        }
    }
}