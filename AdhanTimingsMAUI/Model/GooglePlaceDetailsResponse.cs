using Newtonsoft.Json;

namespace AdhanTimingsMAUI.Model
{
    public class GooglePlaceDetailsResponse
    {
        [JsonProperty("result")]
        public Result Result { get; set; }
    }
}
