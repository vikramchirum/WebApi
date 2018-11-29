using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace api.common.ApiExtensions
{
    public static class HttpRequestExtension
    {
        public static string GetApiHeaderToken(this HttpRequestMessage request)
        {
            IEnumerable<string> header_tokens;
            if (request.Headers.TryGetValues(Constants.Constants.ApiToken, out header_tokens)
                && header_tokens != null && header_tokens.Any(x => !string.IsNullOrWhiteSpace(x)))
            {
                return header_tokens.First(x => !string.IsNullOrWhiteSpace(x));
            }

            return null;
        }
    }
}
