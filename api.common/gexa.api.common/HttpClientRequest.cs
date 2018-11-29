using System;
using System.Collections.Generic;
using System.IO;

namespace api.common
{
    public class HttpClientRequest<T>
    {
        public HttpClientRequest(Api_Settings_Base api_settings)
        {
            this.Api_Settings = api_settings;
        }

        public string Api_Url
        {
            get
            {
                if (Api_Settings != null)
                    return Api_Settings.Api_Url;

                return string.Empty;
            }
        }

        public Api_Settings_Base Api_Settings { get; private set; }

        public string RelativePath { get; set; }

        public string QueryParams { get; set; }

        public Dictionary<string, string> Headers { get; set; }

        public string Url
        {
            get
            {
                var url = Api_Url;
                if (!string.IsNullOrWhiteSpace(this.RelativePath))
                {
                    url = Path.Combine(Api_Url, RelativePath);
                }

                var builder = new UriBuilder(url);
                builder.Query = QueryParams;
                return builder.ToString();
            }
        }

        public T RequestBody { get; set; }
    }
}