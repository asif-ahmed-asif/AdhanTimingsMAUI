using Newtonsoft.Json;

namespace AdhanTimingsMAUI.Model
{
    public class Geometry
    {
        [JsonProperty("location")]
        public Location Location { get; set; }
    }
}
