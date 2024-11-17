using Newtonsoft.Json;

namespace AdhanTimingsMAUI.Model
{
    public class GooglePlacesAutocompleteResponse
    {
        [JsonProperty("predictions")]
        public List<Prediction> Predictions { get; set; }
    }
}