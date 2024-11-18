using Microsoft.Maui.Controls.Shapes;
using Newtonsoft.Json;

namespace AdhanTimingsMAUI.Model
{
    public class Result
    {
        [JsonProperty("geometry")]
        public Geometry Geometry { get; set; }

        [JsonProperty("address_components")]
        public List<AddressComponent> AddressComponents { get; set; }
    }
}
