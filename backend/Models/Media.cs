using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace DisneyApi.Models
{
    public class Media
    {
        [Key]
        public int Id {get; set;}
        [Key]
        public required string MediaType {get; set;}
        public string Overview {get; set;} = string.Empty;
        public string? PosterPath {get; set;} = string.Empty;
        public string ReleaseDate {get; set;} = string.Empty;
        public string Name {get; set;} = string.Empty;
        public float VoteAvg {get; set;} = 0;
        public int VoteCount {get; set;} = 0;

        public List<Character> Characters {get; set;} = [];
    }
}