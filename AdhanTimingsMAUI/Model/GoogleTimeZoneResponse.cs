using Newtonsoft.Json;

namespace AdhanTimingsMAUI.Model
{
    public class GoogleTimeZoneResponse
    {
        [JsonProperty("timeZoneId")]
        public string TimeZoneId { get; set; }
    }
}
