using api.common.Log;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace api.common.Exceptions
{
    public static class Helper
    {
        const string internalServerError = "Internal Server Error";
        const string badRequest = "Bad Request";

        public static Api_Exception Get_Api_Exception<T>(HttpClientRequest<T> request, HttpResponseMessage response)
        {
            Logger.Instance.Info(string.Format("Error response from the services url {0} call.", request.Url));
            Logger.Instance.Info(string.Format(response.ReasonPhrase));

            var error = response.Content.ReadAsStringAsync().Result;
            var api_Exception = Helper.Create_Api_Exception(response);

            if ((int)response.StatusCode >= 500)
            {
                Logger.Instance.Error(error);
                Logger.Instance.Error(api_Exception.Message, api_Exception);
            }
            else
            {
                Logger.Instance.Info(error);
                Logger.Instance.Info(api_Exception.Message, api_Exception);
            }
            
            return api_Exception;
        }

        private static Api_Exception Create_Api_Exception(HttpResponseMessage response)
        {
            // Now wrap into an exception which best fullfills the needs of your application:
            var exception = new Api_Exception(response);

            var httpErrorObject = response.Content.ReadAsStringAsync().Result;

            // Create an anonymous object to use as the template for deserialization:
            var anonymousErrorObject = new { message = "", ExceptionMessage = "" };

            // Deserialize:
            try
            {
                // html errors from the service cannot be parsed.
                // html errors from webapi could be error loading assembly or page not found.
                var deserializedErrorObject = JsonConvert.DeserializeAnonymousType(httpErrorObject, anonymousErrorObject);
            }
            catch
            {
                exception.Data.Add("message", httpErrorObject);
                return exception;
            }

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            var exceptionList = GetInnerExceptions(JsonConvert.DeserializeObject<HttpExceptionResponse>(httpErrorObject, settings));

            foreach (var innerException in exceptionList)
            {
                var errorString = JsonConvert.SerializeObject(innerException, settings);
                var error = JsonConvert.DeserializeObject<Dictionary<string, string>>(errorString);
                foreach (var kvp in error)
                {
                    // Wrap the errors up into the base Exception.Data Dictionary:
                    exception.Data.Add(kvp.Key, kvp.Value);
                }
            }

            return exception;
        }

        /// <summary>
        /// Returns an array of the entire exception list in reverse order
        /// (innermost to outermost exception)
        /// </summary>
        /// <param name="ex">The original exception to work off</param>
        /// <returns>Array of Exceptions from innermost to outermost</returns>
        public static HttpExceptionResponse[] GetInnerExceptions(HttpExceptionResponse ex)
        {
            var exceptions = new List<HttpExceptionResponse>();
            exceptions.Add(ex);

            var currentEx = ex;
            while (currentEx.InnerException != null)
            {
                exceptions.Add(currentEx);
            }

            // Reverse the order to the innermost is first
            exceptions.Reverse();

            return exceptions.ToArray();
        }

        public static JObject ParseApiException(Exception exception, string url)
        {
            if (exception is AggregateException)
            {
                var agg_Exception = (exception as AggregateException).Flatten().InnerException;
                if (agg_Exception is Api_Exception)
                {
                   return  SetApiException(agg_Exception);
                }
                else
                {
                    //Logger.Instance.Error(agg_Exception.Message, agg_Exception);

                    dynamic httpError = new JObject();
                    httpError.HttpStatusCode = HttpStatusCode.InternalServerError;
                    httpError.ErrorMessage = internalServerError;
                    return httpError;
                }
            }
            else if (exception is Api_Exception)
            {
               return  SetApiException(exception);
            }
            else if (exception is InvalidOperationException || exception is ArgumentNullException)
            {
                //Logger.Instance.Error(exception.Message, exception);

                dynamic httpError = new JObject();
                httpError.HttpStatusCode = HttpStatusCode.BadRequest;
                httpError.ErrorMessage = badRequest;
                return httpError;
            }
            else
            {
                //Logger.Instance.Error(exception.Message, exception);

                dynamic httpError = new JObject();
                httpError.HttpStatusCode = HttpStatusCode.InternalServerError;
                httpError.ErrorMessage = internalServerError;
                return httpError;
            }
        }

        private static JObject SetApiException(Exception exception)
        {
            var api_Exception = exception as Api_Exception;
            //Logger.Instance.Error(api_Exception.ErrorMessage, api_Exception);

            dynamic httpError = new JObject();
            httpError.HttpStatusCode = api_Exception.StatusCode;
            var customMessage = GetCustomMessage(api_Exception.StatusCode);
            if (!string.IsNullOrWhiteSpace(customMessage))
            {
                httpError.ErrorMessage = customMessage;
            }
            else
            {
                httpError.ErrorMessage = api_Exception.ErrorMessage;
            }

            return httpError;
        }

        private static string GetCustomMessage(HttpStatusCode statusCode)
        {
            switch (statusCode)
            {
                case HttpStatusCode.NotFound:
                    return "File or directory not found. The resource you are looking for might have been removed, had its name changed, or is temporarily unavailable.";

                case HttpStatusCode.InternalServerError:
                    return "Internal Server Error";

                default:
                    break;
            }

            return null;
        }
    }

    public class HttpExceptionResponse
    {
        public string Message { get; set; }
        public string ExceptionMessage { get; set; }
        public string ExceptionType { get; set; }
        public string StackTrace { get; set; }
        public HttpExceptionResponse InnerException { get; set; }
    }
}