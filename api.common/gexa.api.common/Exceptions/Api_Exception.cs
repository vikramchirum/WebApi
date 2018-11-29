using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace api.common.Exceptions
{
    public class Api_Exception : Exception
    {
        private HttpResponseMessage Response { get; set; }

        public Api_Exception(HttpResponseMessage response)
            : base(response.Content.ReadAsStringAsync().Result)
        {
            this.Response = response;
            this.ReasonPhrase = this.Response.ReasonPhrase;
        }

        public string ReasonPhrase { get; private set; }

        public HttpStatusCode StatusCode
        {
            get
            {
                return this.Response.StatusCode;
            }
        }

        public List<string> Errors
        {
            get
            {
                return this.Data.Values.Cast<string>().ToList();
            }
        }

        public string ErrorMessage
        {
            get
            {
                return string.Join(" ,", Errors);
            }
        }
    }
}