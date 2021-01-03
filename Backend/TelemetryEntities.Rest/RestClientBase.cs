using System;
using System.Net.Http;

namespace StatefulPatternFunctions.Rest
{
    public abstract class RestClientBase
    {
        protected readonly HttpClient _httpClient;
        protected readonly string _baseUrl;
        protected readonly string _apiKey;

        public RestClientBase(HttpClient httpClient, string baseUrl, string apiKey)
        {
            this._httpClient = httpClient;
            this._baseUrl = baseUrl;
            this._apiKey = apiKey;
        }

        protected virtual Uri CreateAPIUri(string apiEndpoint)
        {
            string url;

            if (string.IsNullOrWhiteSpace(apiEndpoint))
            {
                url = $"{this._baseUrl}";
            }
            else
            {
                if (apiEndpoint.StartsWith("/"))
                    apiEndpoint = apiEndpoint.Remove(0, 1);
                url = $"{this._baseUrl}/{apiEndpoint}";
            }

            if (!string.IsNullOrWhiteSpace(this._apiKey))
            {
                if (!url.Contains("?"))
                {
                    url = $"{url}?code={this._apiKey}";
                }
                else
                {
                    url = $"{url}&code={this._apiKey}";
                }
            }

            return new Uri(url);
        }

    }
}
