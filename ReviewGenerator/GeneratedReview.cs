using Newtonsoft.Json;

namespace ReviewGenerator
{
    public class GeneratedReview
    {
        [JsonProperty("reviewText")]
        public string? ReviewText { get; set; }

        [JsonProperty("starRating")]
        public int StarRating { get; set; }
    }
}