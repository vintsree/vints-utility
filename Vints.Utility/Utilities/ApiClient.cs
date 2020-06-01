namespace Vints.Utility
{
    using Enums;
    using Extensions;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    ///  Provides a base class for sending HTTP requests and receiving HTTP responses from a resource identified by a URI.
    /// </summary>
    public class ApiClient : IDisposable
    {
        #region Variable Declaration
        private readonly TimeSpan timeout;
        private HttpClient httpClient;
        private HttpClientHandler httpClientHandler;
        private readonly NetworkCredential networkCredential;
        private readonly Uri baseUri;
        private readonly HttpRequestHeaders requestHeaders;
        private readonly Dictionary<string, string> headerValues;
        private readonly Lazy<JsonSerializerSettings> settings;
        #endregion Variable Declaration

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiClient"/> class with a specific URI
        /// </summary>
        /// <param name="baseUri"></param>
        /// <param name="networkCredential"></param>
        /// <param name="timeout"></param>
        /// <param name="baseUri">The Uri the request is sent to</param>
        /// <exception cref="ArgumentNullException">
        ///     <p><paramref name="baseUri"/> The url was null.</p>
        /// </exception> 
        /// <returns>The task object representing the asynchronous operation</returns>
        public ApiClient(Uri baseUri, NetworkCredential networkCredential = null, TimeSpan? timeout = null)
        {
            if (baseUri == default(Uri))
                throw new ArgumentNullException("baseUri");

            this.baseUri = baseUri;
            this.networkCredential = networkCredential;
            this.timeout = timeout ?? TimeSpan.FromSeconds(90);
            headerValues = new Dictionary<string, string>();
            settings = new Lazy<JsonSerializerSettings>(() =>
            {
                return new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore
                };
            });
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiClient"/> class with a specific URI
        /// </summary>
        /// <param name="baseUri"></param>
        /// <param name="networkCredential"></param>
        /// <param name="timeout"></param>
        public ApiClient(Uri baseUri, Dictionary<string, string> headerValues, NetworkCredential networkCredential = null, TimeSpan? timeout = null)
        : this(baseUri, networkCredential, timeout)
        {
            this.headerValues = headerValues;
        }

        public ApiClient(Uri baseUri, HttpRequestHeaders requestHeaders, NetworkCredential networkCredential = null, TimeSpan? timeout = null)
        : this(baseUri, networkCredential, timeout)
        {
            this.requestHeaders = requestHeaders;
        }

        #endregion Constructors

        #region Async Methods

        /// <summary>
        /// Send a GET request to the specified Uri with an HTTP completion option as an asynchronous operation.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="url">The Uri the request is sent to</param>
        /// <exception cref="ArgumentNullException">
        ///     <p><paramref name="url"/> The url was null.</p>
        /// </exception> 
        /// <exception cref="HttpRequestException">
        ///  The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.
        /// </exception>
        /// <returns>The task object representing the asynchronous operation</returns>
        public async Task<TResult> GetAsync<TResult>(string url) where TResult : class, new()
        {
            var strResponse = await GetAsync(url).ConfigureAwait(false);

            return JsonConvert.DeserializeObject<TResult>(strResponse, settings.Value);
        }

        /// <summary>
        /// Send a GET request to the specified Uri with an HTTP completion option as an asynchronous operation.
        /// </summary>
        /// <param name="url">The Uri the request is sent to</param>
        /// <exception cref="ArgumentNullException">
        ///     <p><paramref name="url"/> The url was null.</p>
        /// </exception> 
        /// <exception cref="HttpRequestException">
        ///  The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.
        /// </exception>
        /// <returns>The task object representing the asynchronous operation</returns>
        public async Task<string> GetAsync(string url)
        {
            EnsureHttpClientCreated();

            using (var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false))
            {
                await response.EnsureSuccessStatusCodeAsync().ConfigureAwait(false);
                return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Send a POST request as an asynchronous operation.
        /// </summary>
        /// <param name="url">The Url the request is sent to.</param>
        /// <param name="input"></param>
        /// <param name="contentType">The HTTP request content sent to the server.</param>
        /// <exception cref="ArgumentNullException">
        ///     <p><paramref name="url"/> is null. </p>
        ///     <p>-or-</p>
        ///     <p><paramref name="input"/> is null. </p>
        /// </exception>
        /// <exception cref="HttpRequestException">
        ///  The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.
        /// </exception>
        /// <returns>The task object representing the asynchronous operation</returns>
        public async Task<string> PostAsync(string url, object input, ContentType contentType = ContentType.JSON)
        {
            EnsureHttpClientCreated();

            using (var requestContent = GetRequestContent(input, contentType))
            {
                using (var response = await httpClient.PostAsync(url, requestContent, CancellationToken.None).ConfigureAwait(false))
                {
                    await response.EnsureSuccessStatusCodeAsync().ConfigureAwait(false);
                    return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Send a POST request as an asynchronous operation.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="url">The Url the request is sent to.</param>
        /// <param name="input"></param>
        /// <param name="contentType">The HTTP request content sent to the server.</param>
        /// <exception cref="ArgumentNullException">
        ///     <p><paramref name="url"/> is null. </p>
        ///     <p>-or-</p>
        ///     <p><paramref name="input"/> is null. </p>
        /// </exception>
        /// <exception cref="HttpRequestException">
        ///  The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.
        /// </exception>
        /// <returns>The task object representing the asynchronous operation</returns>
        public async Task<TResult> PostAsync<TResult>(string url, object input, ContentType contentType = ContentType.JSON) where TResult : class, new()
        {
            var strResponse = await PostAsync(url, input, contentType).ConfigureAwait(false);

            return JsonConvert.DeserializeObject<TResult>(strResponse, settings.Value);
        }

        /// <summary>
        /// Send a PUT request as an asynchronous operation.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="url">The Url the request is sent to.</param>
        /// <param name="input"></param>
        /// <param name="contentType">The HTTP request content sent to the server.</param>
        /// <exception cref="ArgumentNullException">
        ///     <p><paramref name="url"/> is null. </p>
        ///     <p>-or-</p>
        ///     <p><paramref name="input"/> is null. </p>
        /// </exception>
        /// <exception cref="HttpRequestException">
        ///  The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.
        /// </exception>
        /// <returns>The task object representing the asynchronous operation</returns>
        public async Task<string> PutAsync(string url, object input, ContentType contentType = ContentType.JSON)
        {
            EnsureHttpClientCreated();

            using (var requestContent = GetRequestContent(input, contentType))
            {
                using (var response = await httpClient.PutAsync(url, requestContent, CancellationToken.None).ConfigureAwait(false))
                {
                    await response.EnsureSuccessStatusCodeAsync().ConfigureAwait(false);
                    return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Send a PUT request as an asynchronous operation.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="url">The Url the request is sent to.</param>
        /// <param name="input"></param>
        /// <param name="contentType">The HTTP request content sent to the server.</param>
        /// <exception cref="ArgumentNullException">
        ///     <p><paramref name="url"/> is null. </p>
        ///     <p>-or-</p>
        ///     <p><paramref name="input"/> is null. </p>
        /// </exception>
        /// <exception cref="HttpRequestException">
        ///  The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.
        /// </exception>
        /// <returns>The task object representing the asynchronous operation</returns>
        public async Task<TResult> PutAsync<TResult>(string url, object input, ContentType contentType = ContentType.JSON) where TResult : class, new()
        {
            var strResponse = await PutAsync(url, input, contentType).ConfigureAwait(false);

            return JsonConvert.DeserializeObject<TResult>(strResponse, settings.Value);
        }

        /// <summary>
        /// Send a PATCH request as an asynchronous operation.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="url">The Url the request is sent to.</param>
        /// <param name="input"></param>
        /// <param name="contentType">The HTTP request content sent to the server.</param>
        /// <exception cref="ArgumentNullException">
        ///     <p><paramref name="url"/> is null. </p>
        ///     <p>-or-</p>
        ///     <p><paramref name="input"/> is null. </p>
        /// </exception>
        /// <exception cref="HttpRequestException">
        ///  The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.
        /// </exception>
        /// <returns>The task object representing the asynchronous operation</returns>
        public async Task<string> PatchAsync(string url, object input, ContentType contentType = ContentType.JSON)
        {
            EnsureHttpClientCreated();

            using (var requestContent = GetRequestContent(input, contentType))
            {
                var httpVerb = new HttpMethod("PATCH");
                var httpRequestMessage = new HttpRequestMessage(httpVerb, url) { Content = requestContent };
                using (var response = await httpClient.SendAsync(httpRequestMessage, CancellationToken.None).ConfigureAwait(false))
                {
                    await response.EnsureSuccessStatusCodeAsync().ConfigureAwait(false);
                    return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Send a PATCH request as an asynchronous operation.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="url">The Url the request is sent to.</param>
        /// <param name="input"></param>
        /// <param name="contentType">The HTTP request content sent to the server.</param>
        /// <exception cref="ArgumentNullException">
        ///     <p><paramref name="url"/> is null. </p>
        ///     <p>-or-</p>
        ///     <p><paramref name="input"/> is null. </p>
        /// </exception>
        /// <exception cref="HttpRequestException">
        ///  The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.
        /// </exception>
        /// <returns>The task object representing the asynchronous operation</returns>
        public async Task<TResult> PatchAsync<TResult>(string url, object input, ContentType contentType = ContentType.JSON) where TResult : class, new()
        {
            var strResponse = await PatchAsync(url, input, contentType).ConfigureAwait(false);

            return JsonConvert.DeserializeObject<TResult>(strResponse, settings.Value);
        }

        /// <summary>
        /// Send a GET request to the specified Uri with an HTTP completion option as an asynchronous operation.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="url">The Uri the request is sent to</param>
        /// <exception cref="ArgumentNullException">
        ///     <p><paramref name="url"/> The url was null.</p>
        /// </exception> 
        /// <exception cref="HttpRequestException">
        ///  The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.
        /// </exception>
        /// <returns>The task object representing the asynchronous operation</returns>
        public async Task<string> DeleteAsync(string url, ContentType contentType = ContentType.JSON)
        {
            EnsureHttpClientCreated();

            using (var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false))
            {
                await response.EnsureSuccessStatusCodeAsync().ConfigureAwait(false);
                return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Send a Delete request to the specified Uri with an HTTP completion option as an asynchronous operation.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="url">The Uri the request is sent to</param>
        /// <exception cref="ArgumentNullException">
        ///     <p><paramref name="url"/> The url was null.</p>
        /// </exception> 
        /// <exception cref="HttpRequestException">
        ///  The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.
        /// </exception>
        /// <returns>The task object representing the asynchronous operation</returns>
        public async Task<TResult> DeleteAsync<TResult>(string url, ContentType contentType = ContentType.JSON) where TResult : class, new()
        {
            var strResponse = await GetAsync(url).ConfigureAwait(false);

            return JsonConvert.DeserializeObject<TResult>(strResponse, settings.Value);
        }

        /// <summary>
        ///  Send a POST request as an asynchronous operation and gets <see cref="Task{HttpResponseMessage}"/>.
        /// </summary>
        /// <param name="url">The Url the request is sent to.</param>
        /// <param name="input"></param>
        /// <param name="contentType">The HTTP request content sent to the server.</param>
        /// <exception cref="ArgumentNullException">
        ///     <p><paramref name="url"/> is null. </p>
        ///     <p>-or-</p>
        ///     <p><paramref name="input"/> is null. </p>
        /// </exception>
        /// <exception cref="HttpRequestException">
        ///  The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.
        /// </exception>
        /// <returns>The <see cref="Task{HttpResponseMessage}"/> representing the asynchronous operation</returns>
        public async Task<HttpResponseMessage> GetHttpResponseWithPostAsync(string url, object input, ContentType contentType = ContentType.JSON)
        {
            EnsureHttpClientCreated();

            using (var requestContent = GetRequestContent(input, contentType))
            {
                url = string.IsNullOrEmpty(url) ? httpClient.BaseAddress.AbsoluteUri : url;
                var response = await httpClient.PostAsync(url, requestContent, CancellationToken.None).ConfigureAwait(false);
                await response.EnsureSuccessStatusCodeAsync().ConfigureAwait(false);
                return response;
            }
        }



        #endregion Async Methods

        #region IDisposable Implementation

        protected virtual void Dispose(bool disposing)
        {
            if (httpClientHandler != null)
                httpClientHandler.Dispose();
            if (httpClient != null)
                httpClient.Dispose();
        }


        /// <summary>
        ///   Releases the unmanaged resources and disposes of the managed resources used by <see cref="ApiClient"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Implementation

        #region Private Methods

        /// <summary>
        ///  Initializes a new instance of the <see cref="HttpClient"/> class with a specific <see cref="HttpClientHandler"/>.
        /// </summary>
        private void CreateHttpClient()
        {
            httpClientHandler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
            };

            if (networkCredential != null)
            {
                httpClientHandler.Credentials = networkCredential;
                httpClientHandler.UseDefaultCredentials = false;
            }

            httpClient = new HttpClient(httpClientHandler, false)
            {
                BaseAddress = baseUri,
                Timeout = timeout,
            };

            foreach (var item in headerValues)
            {
                httpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
            }

            if (requestHeaders == default(HttpRequestHeaders))
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ContentType.JSON.ToValue()));
            else
            {
                httpClient.DefaultRequestHeaders.Authorization = requestHeaders.Authorization;
            }
        }

        /// <summary>
        /// Ensures that <see cref="HttpClient"></see> instance is available in a state to make the request.
        /// </summary>
        private void EnsureHttpClientCreated()
        {
            if (httpClient == null)
            {
                CreateHttpClient();
            }
        }

        /// <summary>
        /// Serializes the specified object to a JSON string using <see cref="JsonSerializerSettings"></see>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>A JSON string representation of the object.</returns>
        private string ConvertToJsonString(object obj)
        {
            if (obj == null)
                return string.Empty;

            var result = TryDeserializeObject<object>(obj);

            if (result != default(object))
                return obj.ToString();

            return JsonConvert.SerializeObject(obj, settings.Value);
        }

        /// <summary>
        /// DeSerializes the JSON string to a specified object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        private static T TryDeserializeObject<T>(object json)
        {
            try
            {
                if (json == default(object))
                    return default(T);

                return JsonConvert.DeserializeObject<T>(json.ToString());
            }
            catch (JsonSerializationException)
            {
                return default(T);
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        /// <summary>
        /// Get the request content based on <see cref="ContentType"/>
        /// </summary>
        /// <param name="input"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        private HttpContent GetRequestContent(object input, ContentType contentType)
        {
            if (contentType == ContentType.JSON)
                return new StringContent(ConvertToJsonString(input), Encoding.UTF8, contentType.ToValue());
            else
                return new FormUrlEncodedContent((IEnumerable<KeyValuePair<string, string>>)input);
        }

        #endregion Private Methods
    }
}

