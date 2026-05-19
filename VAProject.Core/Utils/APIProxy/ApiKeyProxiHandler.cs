using System.Net.Http;
using VAProject.Core.Logger;

namespace VAProject.Core.Utils.APIProxy
{
    public class ApiKeyProxiHandler : DelegatingHandler
    {
        private readonly string _apiKey;

        public ApiKeyProxiHandler()
        {
            _apiKey = Environment.GetEnvironmentVariable("OPENWEATHER_API_KEY", EnvironmentVariableTarget.User);
            if (string.IsNullOrEmpty(_apiKey))
            {
                LogManager.Log("API key for OpenWeather not found in environment variables.", LogLevel.Error);
            }
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var uriBuilder = new UriBuilder(request.RequestUri);
            var query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);

            query["appid"] = _apiKey;
            uriBuilder.Query = query.ToString();
            request.RequestUri = uriBuilder.Uri;

            LogManager.Log($"[API Proxy] sending request to: {request.RequestUri.Host}", LogLevel.Debug);

            var response = await base.SendAsync(request, cancellationToken);

            return response;
        }
    }
}
