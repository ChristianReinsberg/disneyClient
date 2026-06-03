namespace DisneyApi.DTOs
{
    public class SuggestDTO
    {
        public List<SuggestionData> Suggestions {get; set;} = [];
    }
    
    public class SuggestionData
    {
        public string Name {get; set;} = string.Empty;
        public string Type {get; set;} = string.Empty;
        public int Id {get; set;}
    }
}