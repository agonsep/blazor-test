using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace ardu_store.Components.Pages;

public partial class WeatherWidget : IAsyncDisposable
{
    [Inject]
    private IJSRuntime JS { get; set; } = default!;

    private string currentTime = DateTime.Now.ToString("hh:mm:ss tt");
    private string temperature = "--";
    private string weatherDescription = "";
    private string errorMessage = "";
    private bool isLoading = true;
    private IJSObjectReference? module;
    private DotNetObjectReference<WeatherWidget>? objRef;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            try
            {
                // Load the JavaScript module
                module = await JS.InvokeAsync<IJSObjectReference>("import", "./Components/Pages/WeatherWidget.razor.js");
                
                // Create a reference to this component for JS callbacks
                objRef = DotNetObjectReference.Create(this);
                
                // Start the clock
                await module.InvokeVoidAsync("startClock", objRef);
                
                // Fetch weather data
                await FetchWeatherData();
            }
            catch (Exception ex)
            {
                errorMessage = "Error initializing";
                Console.WriteLine($"Error in WeatherWidget: {ex.Message}");
            }
        }
    }

    private async Task FetchWeatherData()
    {
        try
        {
            isLoading = true;
            StateHasChanged();

            // Using Open-Meteo API (free, no API key required)
            // Coordinates for Denver: 39.7392°N, 104.9903°W
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(
                "https://api.open-meteo.com/v1/forecast?latitude=39.7392&longitude=-104.9903&current=temperature_2m,weather_code&temperature_unit=fahrenheit&timezone=America/Denver");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var weatherData = System.Text.Json.JsonSerializer.Deserialize<WeatherResponse>(json);
                
                if (weatherData?.Current != null)
                {
                    temperature = weatherData.Current.Temperature2m.ToString("F1");
                    weatherDescription = GetWeatherDescription(weatherData.Current.WeatherCode);
                    errorMessage = "";
                }
            }
            else
            {
                errorMessage = "Weather unavailable";
            }
        }
        catch (Exception ex)
        {
            errorMessage = "Unable to fetch";
            Console.WriteLine($"Error fetching weather: {ex.Message}");
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    [JSInvokable]
    public void UpdateTime(string time)
    {
        currentTime = time;
        StateHasChanged();
    }

    private string GetWeatherDescription(int code)
    {
        return code switch
        {
            0 => "Clear sky",
            1 or 2 or 3 => "Partly cloudy",
            45 or 48 => "Foggy",
            51 or 53 or 55 => "Drizzle",
            61 or 63 or 65 => "Rain",
            71 or 73 or 75 => "Snow",
            77 => "Snow grains",
            80 or 81 or 82 => "Rain showers",
            85 or 86 => "Snow showers",
            95 => "Thunderstorm",
            96 or 99 => "Thunderstorm with hail",
            _ => ""
        };
    }

    public async ValueTask DisposeAsync()
    {
        if (module != null)
        {
            try
            {
                await module.InvokeVoidAsync("stopClock");
                await module.DisposeAsync();
            }
            catch { }
        }
        objRef?.Dispose();
    }

    private class WeatherResponse
    {
        public CurrentWeather? Current { get; set; }
    }

    private class CurrentWeather
    {
        [System.Text.Json.Serialization.JsonPropertyName("temperature_2m")]
        public double Temperature2m { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("weather_code")]
        public int WeatherCode { get; set; }
    }
}
