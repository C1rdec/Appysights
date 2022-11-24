using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Appysights.Services
{
    public abstract class HttpServiceBase : IDisposable
    {
        #region Fields

        private readonly HttpClient _client;

        #endregion

        #region Constructors

        public HttpServiceBase()
        {
            _client = new HttpClient();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() => Dispose(true);

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="isDisposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                _client.Dispose();
            }
        }

        /// <summary>
        /// Creates the authorize URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>The text.</returns>
        protected async Task<string> GetText(string url, Dictionary<string, string> headers)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }

            var response = await _client.SendAsync(request);

            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>The content.</returns>
        protected async Task<byte[]> GetContent(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var response = await _client.SendAsync(request);
            return await response.Content.ReadAsByteArrayAsync();
        }

        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <typeparam name="T">The type of the reponse.</typeparam>
        /// <param name="url">The URL.</param>
        /// <returns>The byte­[].</returns>
        protected async Task<T> GetAsync<T>(string url, Dictionary<string, string> headers)
        {
            var value = await GetText(url, headers);
            return System.Text.Json.JsonSerializer.Deserialize<T>(value);
        }

        #endregion
    }
}
