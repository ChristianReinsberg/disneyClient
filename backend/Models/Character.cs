using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace DisneyApi.Models
{
    public class Character
    {
        [Key]
        public int Id {get; set;}
        public string Name {get; set;} = string.Empty;
        public string ImageUrl {get; set;} = string.Empty;
        public string[] Films {get; set;} = [];
        public string[] ShortFilms {get; set;} = [];
        public string[] TvShows {get; set;} = [];
        public List<Media> Medias {get; set;} = [];
    }
}