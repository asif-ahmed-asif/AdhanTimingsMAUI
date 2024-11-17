using Newtonsoft.Json;

namespace AdhanTimingsMAUI.Model
{
    public class Prediction
    {
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("place_id")]
        public string PlaceId { get; set; }
    }
}
