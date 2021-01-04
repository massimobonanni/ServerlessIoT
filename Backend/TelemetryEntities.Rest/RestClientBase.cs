using System;
using System.Net.Http;

namespace StatefulPatternFunctions.Rest
{
    public abstract class RestClientBase
    {
        protected virtual string BaseEndpoint { get; }

        protected readonly HttpClient _httpClient;
        protected readonly string _baseUrl;
        protected readonly string _apiKey;

        public RestClientBase(HttpClient httpClient, string baseUrl, string apiKey)
        {
            this._httpClient = httpClient;
            if (baseUrl.EndsWith("/"))
                baseUrl = baseUrl.Remove(baseUrl.Length - 1, 1);
            this._baseUrl = baseUrl;
            this._apiKey = apiKey;
        }

        protected virtual Uri CreateAPIUri(string apiEndpoint = null)
        {
            string url;

            if (string.IsNullOrWhiteSpace(apiEndpoint))
            {
                url = $"{this._baseUrl}/{this.BaseEndpoint}";
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
