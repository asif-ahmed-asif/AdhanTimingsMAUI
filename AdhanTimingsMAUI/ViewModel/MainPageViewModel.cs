using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using AdhanTimingsMAUI.Model;
using Adhan.Internal;
using Adhan;

namespace AdhanTimingsMAUI.ViewModel
{
    public partial class MainPageViewModel : ObservableObject
    {
        private const string GoogleApiKey = "AIzaSyCRIIew-eQp2QjI5mRLFOE-qoUnl-qKC38";

        private static readonly LocationSuggestion DefaultLocation = new LocationSuggestion
        {
            Description = "New York, NY",
            Latitude = 40.7128,
            Longitude = -74.0060,
            TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time")
        };

        [ObservableProperty]
        private ObservableCollection<LocationSuggestion> locationSuggestions = new();

        [ObservableProperty]
        private ObservableCollection<PrayerTimeItem> prayerTimes = new();

        [ObservableProperty]
        private string searchText;

        [ObservableProperty]
        private LocationSuggestion selectedLocation;

        [ObservableProperty]
        private DateTime selectedDate = DateTime.Today;

        [ObservableProperty]
        private CalculationMethod selectedCalculationMethod = CalculationMethod.NORTH_AMERICA;

        public IEnumerable<CalculationMethod> CalculationMethods { get; } = Enum.GetValues(typeof(CalculationMethod)).Cast<CalculationMethod>();

        public MainPageViewModel()
        {
            SelectedLocation = DefaultLocation;
            _ = LoadPrayerTimesAsync(DefaultLocation.Latitude, DefaultLocation.Longitude, DefaultLocation.TimeZone);
        }

        partial void OnSearchTextChanged(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                LocationSuggestions.Clear();
            }
            else
            {
                _ = SearchLocationsAsync(value);
            }
        }

        partial void OnSelectedDateChanged(DateTime value)
        {
            if (SelectedLocation != null)
            {
                _ = LoadPrayerTimesAsync(SelectedLocation.Latitude, SelectedLocation.Longitude, SelectedLocation.TimeZone);
            }
        }

        partial void OnSelectedLocationChanged(LocationSuggestion value)
        {
            if (value != null)
            {
                SearchText = string.Empty;
                LocationSuggestions.Clear();
                _ = SelectLocationAsync(value);
            }
        }

        partial void OnSelectedCalculationMethodChanged(CalculationMethod value)
        {
            if (SelectedLocation != null)
            {
                _ = LoadPrayerTimesAsync(SelectedLocation.Latitude, SelectedLocation.Longitude, SelectedLocation.TimeZone);
            }
        }

        private async Task SearchLocationsAsync(string query)
        {
            var url = $"https://maps.googleapis.com/maps/api/place/autocomplete/json?input={query}&types=(regions)&key={GoogleApiKey}";

            try
            {
                using var httpClient = new HttpClient();
                var response = await httpClient.GetStringAsync(url);
                var result = JsonConvert.DeserializeObject<GooglePlacesAutocompleteResponse>(response);

                if (result?.Predictions == null)
                    return;

                LocationSuggestions.Clear();
                foreach (var prediction in result.Predictions)
                {
                    LocationSuggestions.Add(new LocationSuggestion
                    {
                        Description = prediction.Description,
                        PlaceId = prediction.PlaceId
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching autocomplete suggestions: {ex.Message}");
            }
        }

        [RelayCommand]
        public async Task SelectLocationAsync(LocationSuggestion selectedLocation)
        {
            if (selectedLocation == null) return;

            var placeDetailsUrl = $"https://maps.googleapis.com/maps/api/place/details/json?place_id={selectedLocation.PlaceId}&key={GoogleApiKey}";

            try
            {
                using var httpClient = new HttpClient();
                var response = await httpClient.GetStringAsync(placeDetailsUrl);
                var placeDetails = JsonConvert.DeserializeObject<GooglePlaceDetailsResponse>(response);

                var location = placeDetails?.Result?.Geometry?.Location;

                var zipCodeComponent = placeDetails?.Result?.AddressComponents.FirstOrDefault(c =>
                    c.Types.Contains("postal_code"));
                selectedLocation.ZipCode = zipCodeComponent?.LongName;

                var timeZoneUrl = $"https://maps.googleapis.com/maps/api/timezone/json?location={location?.Lat},{location?.Lng}&timestamp={DateTimeOffset.UtcNow.ToUnixTimeSeconds()}&key={GoogleApiKey}";
                var timeZoneResponse = await httpClient.GetStringAsync(timeZoneUrl);

                var timeZoneData = JsonConvert.DeserializeObject<GoogleTimeZoneResponse>(timeZoneResponse);
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneData?.TimeZoneId);

                selectedLocation.Latitude = location.Lat;
                selectedLocation.Longitude = location.Lng;
                selectedLocation.TimeZone = timeZone;

                await LoadPrayerTimesAsync(location.Lat, location.Lng, timeZone);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching place details: {ex.Message}");
            }
        }

        private async Task LoadPrayerTimesAsync(double latitude, double longitude, TimeZoneInfo timeZone)
        {
            try
            {
                Coordinates coordinates = new(latitude, longitude);
                DateComponents dateComponents = DateComponents.From(SelectedDate);
                CalculationParameters parameters = SelectedCalculationMethod.GetParameters();

                PrayerTimes prayerTimesResult = new(coordinates, dateComponents, parameters);

                PrayerTimes.Clear();
                PrayerTimes.Add(new PrayerTimeItem { Name = "Fajr", Time = TimeZoneInfo.ConvertTimeFromUtc(prayerTimesResult.Fajr, timeZone).ToString("hh:mm tt") });
                PrayerTimes.Add(new PrayerTimeItem { Name = "Sunrise", Time = TimeZoneInfo.ConvertTimeFromUtc(prayerTimesResult.Sunrise, timeZone).ToString("hh:mm tt") });
                PrayerTimes.Add(new PrayerTimeItem { Name = "Dhuhr", Time = TimeZoneInfo.ConvertTimeFromUtc(prayerTimesResult.Dhuhr, timeZone).ToString("hh:mm tt") });
                PrayerTimes.Add(new PrayerTimeItem { Name = "Asr", Time = TimeZoneInfo.ConvertTimeFromUtc(prayerTimesResult.Asr, timeZone).ToString("hh:mm tt") });
                PrayerTimes.Add(new PrayerTimeItem { Name = "Maghrib", Time = TimeZoneInfo.ConvertTimeFromUtc(prayerTimesResult.Maghrib, timeZone).ToString("hh:mm tt") });
                PrayerTimes.Add(new PrayerTimeItem { Name = "Isha", Time = TimeZoneInfo.ConvertTimeFromUtc(prayerTimesResult.Isha, timeZone).ToString("hh:mm tt") });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading prayer times: {ex.Message}");
            }
        }
    }
}
