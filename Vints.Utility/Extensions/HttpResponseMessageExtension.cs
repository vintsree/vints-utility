namespace System.Net.Http
{
    using Threading.Tasks;
    using Vints.Utility.Exceptions;
    /// <summary>
    /// This class is an extension to <see cref="HttpResponseMessage"/>
    /// </summary>
    public static class HttpResponseMessageExtension
    {

        /// <summary>
        /// Ensure Success Status Code Async
        /// </summary>
        /// <param name="response"></param>
        /// <exception cref="HttpResponseException">
        ///     <p><paramref name="response"/> <see cref="HttpResponseMessage.IsSuccessStatusCode"> is false</see></p>
        /// </exception>
        /// <returns></returns>
        public static async Task EnsureSuccessStatusCodeAsync(this HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return;
            }

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (response.Content != null)
                response.Content.Dispose();

            throw new HttpResponseException(response.StatusCode, content);
        }

        /// <summary>
        /// Ensure Success Status Code
        /// </summary>
        /// <param name="response"></param>
        /// <exception cref="HttpResponseException">
        ///     <p><paramref name="response"/> <see cref="HttpResponseMessage.IsSuccessStatusCode"> is false</see></p>
        /// </exception>
        /// <returns></returns>
        public static void EnsureSuccessStatusCode(this HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return;
            }

            var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            if (response.Content != null)
                response.Content.Dispose();

            throw new HttpResponseException(response.StatusCode, content);
        }
    }
}
