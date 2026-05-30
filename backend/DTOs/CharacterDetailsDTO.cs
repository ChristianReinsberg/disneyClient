namespace DisneyApi.DTOs
{
    public class CharacterDetailsDto
    {
        public int Id {get; set;}
        public string Name {get; set;} = string.Empty;
        public string ImageUrl {get; set;} = string.Empty;
        public List<FlatMediaDto> Medias {get; set;} = [];
    }

    public class FlatMediaDto
    {
        public int Id {get; set;}
        public string Title {get; set;} = string.Empty;
        public string MediaType {get; set;} = string.Empty;
        public string PosterPath {get; set;} = string.Empty;
        public float VoteAvg {get; set;} = 0;
        public int VoteCount {get; set;} = 0;
    }
}