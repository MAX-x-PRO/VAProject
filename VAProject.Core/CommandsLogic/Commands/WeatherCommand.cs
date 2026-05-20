using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows.Media;
using VAProject.Core.Interfaces;
using VAProject.Core.Models.Notifications;
using VAProject.Core.Utils.Memorization;

namespace VAProject.Core.CommandsLogic.Commands
{
    internal class WeatherCommand : IVoiceCommand
    {
        public List<string> Triggers => new List<string>
        {
            "whether",
            "weather",
            "forecast",
            "temperature"
        };

        private LruCache<string, Task<CommandResult>> _weatherCache = new LruCache<string, Task<CommandResult>>(capacity: 5);

        private readonly IHttpClientFactory _clientFactory;
        
        public WeatherCommand(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public Task<CommandResult> OnExecute(string cmdText)
        {
            string city = GetCityFromCommand(cmdText);

            try
            {
                return _weatherCache.GetOrAdd(city, (city) => FetchWeather(city));
            }
            catch (Exception ex)
            {
                return Task.FromResult(new CommandResult
                {
                    Success = false,
                    LogMessage = $"Failed to fetch weather for {city}: {ex.Message}",
                    TTSResponse = $"Sorry, I couldn't get the weather for {city}.",
                    NotificationPayload = new TextPayload
                    {
                        Text = $"Sorry, I couldn't get the weather for {city}.",
                        AccentColor = Colors.Red
                    }
                });
            }
        }

        private string GetCityFromCommand(string cmdText)
        {
            Regex regex = new Regex(@"(in\s+(?<city>\w+))?", RegexOptions.IgnoreCase);
            Match match = regex.Match(cmdText);

            if (match.Groups["city"].Success)
                return match.Groups["city"].Value;
            return "Kyiv";
        }

        private Task<CommandResult> FetchWeather(string city)
        {
            string url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&units=metric";

            using (var client = _clientFactory.CreateClient("WeatherApi"))
            {
                var response = client.GetAsync(url).Result;

                if (!response.IsSuccessStatusCode)
                    return Task.FromResult(new CommandResult
                    {
                        Success = false,
                        LogMessage = $"Failed to fetch weather for {city}: {response.ReasonPhrase}",
                        TTSResponse = $"Sorry, I couldn't get the weather for {city}.",
                        NotificationPayload = new TextPayload
                        {
                            Text = $"Sorry, I couldn't get the weather for {city}.",
                            AccentColor = Colors.Red
                        }
                    });

                string json = response.Content.ReadAsStringAsync().Result;

                return ParseWeather(JsonDocument.Parse(json));
            }
        }

        private Task<CommandResult> ParseWeather(JsonDocument doc)
        {
            var root = doc.RootElement;

            double temp = root.GetProperty("main").GetProperty("temp").GetDouble();
            int tempRounded = (int)Math.Round(temp);

            string description = root.GetProperty("weather")[0].GetProperty("description").GetString();
            string city = root.GetProperty("name").GetString();

            return Task.FromResult(new CommandResult
            {
                Success = true,
                LogMessage = $"Fetched weather for {city}",
                TTSResponse = $"The current weather in {city} is {description}. Temperature: {tempRounded}°C",
                NotificationPayload = new WeatherPayload
                {
                    City = city,
                    Description = description,
                    Temperature = tempRounded
                }
            });
        }
    }
}
