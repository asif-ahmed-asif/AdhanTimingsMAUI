﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using AdhanTimingsMAUI.Model;
using Adhan.Internal;
using Adhan;
using System.ComponentModel;

namespace AdhanTimingsMAUI.ViewModel
{
    public partial class MainPageViewModel : ObservableObject
    {
        private const string GoogleApiKey = "YOUR_GOOGLE_API_KEY"; // Replace with your actual API key

        // Observable properties
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

        public MainPageViewModel()
        {
        }

        // Property changed partial methods
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
            _ = LoadPrayerTimesAsync(SelectedLocation.Latitude, SelectedLocation.Longitude, SelectedLocation.TimeZone);
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

        // Methods
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

                var location = placeDetails?.Result.Geometry.Location;

                // Extract zip code if needed
                var zipCodeComponent = placeDetails?.Result.AddressComponents.FirstOrDefault(c =>
                    c.Types.Contains("postal_code"));
                selectedLocation.ZipCode = zipCodeComponent?.LongName;

                // Fetch Time Zone
                var timeZoneUrl = $"https://maps.googleapis.com/maps/api/timezone/json?location={location?.Lat},{location?.Lng}&timestamp={DateTimeOffset.UtcNow.ToUnixTimeSeconds()}&key={GoogleApiKey}";
                var timeZoneResponse = await httpClient.GetStringAsync(timeZoneUrl);

                var timeZoneData = JsonConvert.DeserializeObject<GoogleTimeZoneResponse>(timeZoneResponse);
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneData?.TimeZoneId);

                // Update selected location details
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
                CalculationParameters parameters = CalculationMethod.NORTH_AMERICA.GetParameters();

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
