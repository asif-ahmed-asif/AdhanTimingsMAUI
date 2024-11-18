using Newtonsoft.Json;

namespace AdhanTimingsMAUI.Model
{
    public class AddressComponent
    {
        [JsonProperty("long_name")]
        public string LongName { get; set; }

        [JsonProperty("types")]
        public List<string> Types { get; set; }
    }
}
