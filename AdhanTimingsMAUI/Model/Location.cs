using Newtonsoft.Json;

namespace AdhanTimingsMAUI.Model
{
    public class Location
    {
        [JsonProperty("lat")]
        public double Lat { get; set; }

        [JsonProperty("lng")]
        public double Lng { get; set; }
    }
}
