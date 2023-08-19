using Newtonsoft.Json;

namespace ReviewGenerator
{
    public class Review
    {
        [JsonProperty("reviewText")]
        public string? ReviewText { get; set; }
    }
}