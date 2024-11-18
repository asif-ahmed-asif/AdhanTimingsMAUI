namespace AdhanTimingsMAUI.Model
{
    public class LocationSuggestion
    {
        public string Description { get; set; }
        public string PlaceId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public TimeZoneInfo TimeZone { get; set; }
        public string ZipCode { get; set; }
    }
}
