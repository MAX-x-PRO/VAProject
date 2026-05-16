using System;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows.Media;
using VAProject.Core.Interfaces;
using VAProject.Core.Logger;
using VAProject.Core.Models.Notifications;

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

        private LruCache<string, CommandResult> _weatherCache = new LruCache<string, CommandResult>(capacity: 5);

        public CommandResult OnExecute(string cmdText)
        {
            string city = GetCityFromCommand(cmdText);

            try
            {
                return _weatherCache.GetOrAdd(city, (city) => FetchWeather(city));
            }
            catch (Exception ex)
            {
                return new CommandResult
                {
                    Success = false,
                    LogMessage = $"Failed to fetch weather for {city}: {ex.Message}",
                    TTSResponse = $"Sorry, I couldn't get the weather for {city}.",
                    NotificationPayload = new TextPayload
                    {
                        Text = $"Sorry, I couldn't get the weather for {city}.",
                        AccentColor = Colors.Red
                    }
                };
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

        private CommandResult FetchWeather(string city)
        {
            string apiKey = Environment.GetEnvironmentVariable("OPENWEATHER_API_KEY", EnvironmentVariableTarget.User);

            if (string.IsNullOrEmpty(apiKey))
            {
                return new CommandResult
                {
                    Success = false,
                    LogMessage = "OPENWEATHER_API_KEY environment variable is not set.",
                    TTSResponse = "Sorry, I can't fetch the weather right now.",
                    NotificationPayload = new TextPayload
                    {
                        Text = "Sorry, I can't fetch the weather right now.",
                        AccentColor = Colors.Red
                    }
                };
            }

            string url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={apiKey}&units=metric";

            using (var client = new HttpClient())
            {
                var response = client.GetAsync(url).Result;

                if (!response.IsSuccessStatusCode)
                    return new CommandResult
                    {
                        Success = false,
                        LogMessage = $"Failed to fetch weather for {city}: {response.ReasonPhrase}",
                        TTSResponse = $"Sorry, I couldn't get the weather for {city}.",
                        NotificationPayload = new TextPayload
                        {
                            Text = $"Sorry, I couldn't get the weather for {city}.",
                            AccentColor = Colors.Red
                        }
                    };

                string json = response.Content.ReadAsStringAsync().Result;

                return ParseWeather(JsonDocument.Parse(json));
            }
        }

        private CommandResult ParseWeather(JsonDocument doc)
        {
            var root = doc.RootElement;

            double temp = root.GetProperty("main").GetProperty("temp").GetDouble();
            int tempRounded = (int)Math.Round(temp);

            string description = root.GetProperty("weather")[0].GetProperty("description").GetString();
            string city = root.GetProperty("name").GetString();

            return new CommandResult
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
            };
        }
    }
}
