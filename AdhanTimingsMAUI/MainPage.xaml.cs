using Adhan.Internal;
using Adhan;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using AdhanTimingsMAUI.Model;

namespace AdhanTimingsMAUI
{
    public partial class MainPage : ContentPage
    {
        private const string GoogleApiKey = "AIzaSyCRIIew-eQp2QjI5mRLFOE-qoUnl-qKC38";
        private ObservableCollection<LocationSuggestion> LocationSuggestions = new();

        public MainPage()
        {
            InitializeComponent();
            LocationSuggestionsList.ItemsSource = LocationSuggestions;
        }

        private async void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(e.NewTextValue))
                {
                    LocationSuggestions.Clear();
                    return;
                }

                string query = e.NewTextValue;
                string url = $"https://maps.googleapis.com/maps/api/place/autocomplete/json?input={query}&key={GoogleApiKey}";

                using var httpClient = new HttpClient();
                var response = await httpClient.GetStringAsync(url);

                var result = JsonConvert.DeserializeObject<GooglePlacesAutocompleteResponse>(response);
                if (result?.Predictions == null) return;

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

        private async void OnLocationSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is not LocationSuggestion selectedLocation) return;

            try
            {
                string placeDetailsUrl = $"https://maps.googleapis.com/maps/api/place/details/json?place_id={selectedLocation.PlaceId}&key={GoogleApiKey}";
                using var httpClient = new HttpClient();
                var response = await httpClient.GetStringAsync(placeDetailsUrl);

                var placeDetails = JsonConvert.DeserializeObject<GooglePlaceDetailsResponse>(response);
                var location = placeDetails.Result.Geometry.Location;

                string timeZoneUrl = $"https://maps.googleapis.com/maps/api/timezone/json?location={location.Lat},{location.Lng}&timestamp={DateTimeOffset.UtcNow.ToUnixTimeSeconds()}&key={GoogleApiKey}";
                var timeZoneResponse = await httpClient.GetStringAsync(timeZoneUrl);

                var timeZoneData = JsonConvert.DeserializeObject<GoogleTimeZoneResponse>(timeZoneResponse);
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneData.TimeZoneId);

                await LoadPrayerTimesAsync(location.Lat, location.Lng, timeZone);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching location or time zone details: {ex.Message}");
            }
            finally
            {
                LocationSuggestionsList.SelectedItem = null;
            }
        }

        private async Task LoadPrayerTimesAsync(double latitude, double longitude, TimeZoneInfo timeZone)
        {
            try
            {
                Coordinates coordinates = new(latitude, longitude);
                DateComponents dateComponents = DateComponents.From(DateTime.Now);
                CalculationParameters parameters = CalculationMethod.NORTH_AMERICA.GetParameters();

                PrayerTimes prayerTimes = new(coordinates, dateComponents, parameters);

                Device.BeginInvokeOnMainThread(() =>
                {
                    FajrLabel.Text = $"Fajr: {TimeZoneInfo.ConvertTimeFromUtc(prayerTimes.Fajr, timeZone)}";
                    SunriseLabel.Text = $"Sunrise: {TimeZoneInfo.ConvertTimeFromUtc(prayerTimes.Sunrise, timeZone)}";
                    DhuhrLabel.Text = $"Dhuhr: {TimeZoneInfo.ConvertTimeFromUtc(prayerTimes.Dhuhr, timeZone)}";
                    AsrLabel.Text = $"Asr: {TimeZoneInfo.ConvertTimeFromUtc(prayerTimes.Asr, timeZone)}";
                    MaghribLabel.Text = $"Maghrib: {TimeZoneInfo.ConvertTimeFromUtc(prayerTimes.Maghrib, timeZone)}";
                    IshaLabel.Text = $"Isha: {TimeZoneInfo.ConvertTimeFromUtc(prayerTimes.Isha, timeZone)}";
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading prayer times: {ex.Message}");
            }
        }
    }
}
