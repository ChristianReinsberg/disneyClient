using System.Text.Json.Serialization;

namespace DisneyApi.DTOs
{
    public class DisneyData
    {
        [JsonPropertyName("data")]
        public List<DisneyResult> Data {get; set;} = [];
    }

    public class DisneyInfo
    {
        [JsonPropertyName("info")]
        public DisneyPages Info {get; set;} = new DisneyPages();
    }

    public class DisneyPages
    {
        [JsonPropertyName("totalPages")]
        public int TotalPages {get; set;}
    }

    public class DisneyResult
    {
        [JsonPropertyName("_id")]
        public int Id {get; set;}
        [JsonPropertyName("films")]
        public string[] Films {get; set;} = [];
        [JsonPropertyName("shortFilms")]
        public string[] ShortFilms {get; set;} = [];
        [JsonPropertyName("tvShows")]
        public string[] TvShows {get; set;} = [];
        [JsonPropertyName("allies")]
        public string[] Allies {get; set;} = [];
        [JsonPropertyName("enemies")]
        public string[] Enemies {get; set;} = [];
        [JsonPropertyName("name")]
        public string Name {get; set;} = string.Empty;
        [JsonPropertyName("imageUrl")]
        public string ImageUrl {get; set;} = string.Empty;
    }
}