using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;

namespace api.common.Controllers
{
    public class BaseController : ApiController
    {
        private Api_Settings_Base Api_settings { get; set; }

        public BaseController(Api_Settings_Base api_settings)
        {
            if (api_settings == null)
                throw new ArgumentNullException("Api Settings is null. Please Check your application settings.");

            Api_settings = api_settings;
        }

        #region GET

        protected virtual async Task<TInput> GetAPIWithUriQueryParamsAsync<TInput>()
        {
            var request = this.GetHttpClientRequest<TInput>();
            return await HttpClientHelper.Get<TInput>(request);
        }

        protected virtual async Task<TInput> GetAPIWithQueryParamsAsync<TInput>(string relativePath, string queryParams)
        {
            var request = this.GetHttpClientRequest<TInput>(relativePath, queryParams);
            return await HttpClientHelper.Get<TInput>(request);
        }

        protected virtual async Task<TInput> GetAPIWithRequest<TInput>(HttpClientRequest<TInput> request)
        {
            return await HttpClientHelper.Get<TInput>(request);
        }

        protected virtual async Task<HttpResponseMessage> GetAPIFileStreamAsync<TInput>(HttpClientRequest<HttpResponseMessage> request)
        {
            return await HttpClientHelper.GetFileContent_GET(request);
        }

        protected virtual async Task<HttpResponseMessage> GetAPIFileStreamAsync_POST<TInput>(HttpClientRequest<TInput> request)
        {
            return await HttpClientHelper.GetFileContent_POST(request);
        }

        protected virtual async Task<IHttpActionResult> GetPDFResponse(string url, string relativePath, string fileName = "")
        {
            var api_settings = new Api_Settings_Base();
            api_settings.Api_Url = url;

            var request = new HttpClientRequest<HttpResponseMessage>(api_settings);
            request.RelativePath = relativePath;

            var fileResponse = await this.GetAPIFileStreamAsync<HttpResponseMessage>(request);

            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StreamContent(await fileResponse.Content.ReadAsStreamAsync());
            response.Content.Headers.ContentType = fileResponse.Content.Headers.ContentType;
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline");
            if (fileResponse.Content.Headers.ContentDisposition != null && !string.IsNullOrWhiteSpace(fileResponse.Content.Headers.ContentDisposition.FileName))
            {
                response.Content.Headers.ContentDisposition.FileName = fileResponse.Content.Headers.ContentDisposition.FileName;
            }
            else
            {
                response.Content.Headers.ContentDisposition.FileName = fileName;
            }

            // for client side apps (angular apps) with CORS we have to explicitly set the response header.
            response.Content.Headers.Add("Access-Control-Expose-Headers", "ContentDisposition");
            return ResponseMessage(response);
        }

        protected virtual async Task<IHttpActionResult> GetPDFResponse_POST<TInput>(string url, string relativePath, TInput body, string fileName = "")
        {
            var api_settings = new Api_Settings_Base();
            api_settings.Api_Url = url;

            var request = new HttpClientRequest<TInput>(api_settings);
            request.RelativePath = relativePath;
            request.RequestBody = body;

            var fileResponse = await this.GetAPIFileStreamAsync_POST(request);

            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StreamContent(await fileResponse.Content.ReadAsStreamAsync());
            response.Content.Headers.ContentType = fileResponse.Content.Headers.ContentType;
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline");
            if (fileResponse.Content.Headers.ContentDisposition != null && !string.IsNullOrWhiteSpace(fileResponse.Content.Headers.ContentDisposition.FileName))
            {
                response.Content.Headers.ContentDisposition.FileName = fileResponse.Content.Headers.ContentDisposition.FileName;
            }
            else
            {
                response.Content.Headers.ContentDisposition.FileName = fileName;
            }

            // for client side apps (angular apps) with CORS we have to explicitly set the response header.
            response.Content.Headers.Add("Access-Control-Expose-Headers", "ContentDisposition");
            return ResponseMessage(response);
        }

        #endregion GET

        #region POST

        protected virtual async Task<TOutput> PostAPIWithUriQueryParamsAsync<TInput, TOutput>(TInput requestBody)
        {
            var client_request = this.GetHttpClientRequest<TInput>(requestBody);
            return await HttpClientHelper.Post<TInput, TOutput>(client_request);
        }

        protected virtual async Task<TOutput> PostAPIWithQueryParamsAsync<TInput, TOutput>(string relativePath, string queryParams, TInput requestBody)
        {
            var client_request = this.GetHttpClientRequest<TInput>(relativePath, queryParams, requestBody);
            return await HttpClientHelper.Post<TInput, TOutput>(client_request);
        }

        protected virtual async Task<TOutput> PostAPIWithUriQueryParamsAsync<TInput, TOutput>(HttpClientRequest<TInput> request)
        {
            return await HttpClientHelper.Post<TInput, TOutput>(request);
        }

        #endregion POST

        #region PUT

        protected virtual async Task<TOutput> PutAPIWithUriQueryParamsAsync<TInput, TOutput>(TInput requestBody)
        {
            var client_request = this.GetHttpClientRequest<TInput>(requestBody);
            return await HttpClientHelper.Put<TInput, TOutput>(client_request);
        }

        protected virtual async Task<TOutput> PutAPIWithQueryParamsAsync<TInput, TOutput>(string relativePath, string queryParams, TInput requestBody)
        {
            var client_request = this.GetHttpClientRequest<TInput>(relativePath, queryParams, requestBody);
            return await HttpClientHelper.Put<TInput, TOutput>(client_request);
        }

        protected virtual async Task<TOutput> PutAPIWithUriQueryParamsAsync<TInput, TOutput>(HttpClientRequest<TInput> request)
        {
            return await HttpClientHelper.Put<TInput, TOutput>(request);
        }

        #endregion

        #region DELETE

        protected virtual async Task<TOutput> DeleteAPIWithUriQueryParamsAsync<TInput, TOutput>()
        {
            var request = this.GetHttpClientRequest<TInput>();
            return await HttpClientHelper.Delete<TInput, TOutput>(request);
        }

        protected virtual async Task<TOutput> DeleteAPIWithQueryParamsAsync<TInput, TOutput>(string relativePath, string queryParams)
        {
            var request = this.GetHttpClientRequest<TInput>(relativePath, queryParams);
            return await HttpClientHelper.Delete<TInput, TOutput>(request);
        }

        protected virtual async Task<TOutput> DeleteAPIWithUriQueryParamsAsync<TInput, TOutput>(HttpClientRequest<TInput> request)
        {
            return await HttpClientHelper.Delete<TInput, TOutput>(request);
        }

        #endregion DELETE

        #region Http Client Requests

        /// <summary>
        /// Gets the request with default uri params from the request
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <returns></returns>
        protected virtual HttpClientRequest<TInput> GetHttpClientRequest<TInput>()
        {
            var clientRequest = new HttpClientRequest<TInput>(Api_settings);
            if (!string.IsNullOrWhiteSpace(Request.RequestUri.Query))
            {
                clientRequest.QueryParams = Request.RequestUri.Query.Substring(1, Request.RequestUri.Query.Length - 1);
            }

            // remove the leading / since the the base url already has / to it.
            clientRequest.RelativePath = Request.RequestUri.AbsolutePath.Substring(1, Request.RequestUri.AbsolutePath.Length - 1);
            return clientRequest;
        }

        protected virtual HttpClientRequest<TInput> GetHttpClientRequest<TInput>(TInput requestBody)
        {
            var clientRequest = GetHttpClientRequest<TInput>();
            clientRequest.RequestBody = requestBody;
            return clientRequest;
        }

        protected virtual HttpClientRequest<TInput> GetHttpClientRequest<TInput>(string relativePath, string queryParams)
        {
            var clientRequest = new HttpClientRequest<TInput>(Api_settings);
            if (!string.IsNullOrWhiteSpace(queryParams))
            {
                clientRequest.QueryParams = queryParams;
            }

            // remove the leading / since the the base url already has / to it.
            clientRequest.RelativePath = relativePath;
            return clientRequest;
        }

        protected virtual HttpClientRequest<TInput> GetHttpClientRequest<TInput>(string relativePath, string queryParams, TInput requestBody)
        {
            var clientRequest = GetHttpClientRequest<TInput>(relativePath, queryParams);
            clientRequest.RequestBody = requestBody;
            return clientRequest;
        }

        #endregion
    }
}
