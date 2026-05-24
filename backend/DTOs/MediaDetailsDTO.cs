namespace DisneyApi.DTOs
{
    public class MediaDetailsDto
    {
        public int Id {get; set;}
        public string MediaType {get; set;} = string.Empty;
        public string Title {get; set;} = string.Empty;
        public string Overview {get; set;} = string.Empty;
        public string PosterPath {get; set;} = string.Empty;
        public float VoteAvg {get; set;} = 0;
        public int VoteCount {get; set;} = 0;
        public string ReleaseDate {get; set;} = string.Empty;
        public List<FlatCharacterDto> Characters {get; set;} = [];
    }

    public class FlatCharacterDto
    {
        public int Id {get; set;}
        public string Name {get; set;} = string.Empty;
        public string ImageUrl {get; set;} = string.Empty;
    }
}