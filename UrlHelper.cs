using api.common.ApiExtensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace api.common
{
    public static class UrlHelper
    {
        public async static Task<string> ToQueryStringAsync(this object request)
        {
            var keyValues = GetKeyValues(request);
            if (keyValues.IsNullOrEmpty())
            {
                return null;
            }

            var formUrlEncodedContent = new FormUrlEncodedContent(keyValues);
            var urlEncodedString = await formUrlEncodedContent.ReadAsStringAsync();
            return urlEncodedString;
        }

        private static IDictionary<string, string> GetKeyValues(object request)
        {
            if (request == null)
            {
                return null;
            }

            var token = request as JToken;
            if (token == null)
            {
                var requesttoken = JObject.FromObject(request, new JsonSerializer { NullValueHandling = NullValueHandling.Ignore });
                return UrlHelper.GetKeyValues(requesttoken);
            }

            if (token.HasValues)
            {
                var contentData = new Dictionary<string, string>();
                foreach (var child in token.Children().ToList())
                {
                    var childContent = UrlHelper.GetKeyValues(child);
                    if (childContent != null)
                    {
                        contentData = contentData.Concat(childContent).ToDictionary(k => k.Key, v => v.Value);
                    }
                }

                return contentData;
            }

            var jValue = token as JValue;
            if (jValue == null || jValue.Value == null || string.IsNullOrWhiteSpace(jValue.ToString()))
            {
                return null;
            }

            var value = jValue.Type == JTokenType.Date ?
                            jValue.ToString("o", CultureInfo.InvariantCulture) :
                            jValue.ToString(CultureInfo.InvariantCulture);

            return new Dictionary<string, string> { { token.Path, value } };
        }
    }
}
