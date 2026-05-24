using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DisneyApi.DTOs
{
    public class TmdbData
    {
        [JsonPropertyName("results")]
        public List<TmdbResult> Results {get; set;} = [];
    }

    public class TmdbResult
    {
        [JsonPropertyName("adult")]
        public bool Adult {get; set;}
        [JsonPropertyName("backdrop_path")]
        public string Backdrop_Path {get; set;} = string.Empty;
        [JsonPropertyName("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id {get; set;}
        [JsonPropertyName("original_language")]
        public string Original_Language {get; set;} = string.Empty;
        [JsonPropertyName("original_title")]
        public string Original_Title {get; set;} = string.Empty;
        [JsonPropertyName("overview")]
        public string Overview {get; set;} = string.Empty;
        [JsonPropertyName("popularity")]
        public double Popularity {get; set;}
        [JsonPropertyName("poster_path")]
        public string Poster_Path {get; set;} = string.Empty;
        [JsonPropertyName("release_date")]
        public string Release_Date {get; set;} = string.Empty;
        [JsonPropertyName("first_air_date")]
        public string First_Air_Date {get; set;} = string.Empty;
        [JsonPropertyName("title")]
        public string Title {get; set;} = string.Empty;
        [JsonPropertyName("name")]
        public string Name {get; set;} = string.Empty;
        [JsonPropertyName("vote_average")]
        public float Vote_Average {get; set;}
        [JsonPropertyName("vote_count")]
        public int Vote_Count {get; set;}
    }
}