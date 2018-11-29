using api.common.Attributes;
using api.common.Certificates;
using api.common.Log;

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace api.common.Filters
{
    public class CertificateAuthorizationFilter : AuthorizeAttribute
    {
        private readonly Api_Settings_Base api_settings;
        private readonly ICertificateService certificateService;

        public CertificateAuthorizationFilter(Api_Settings_Base api_settings)
        {
            if (api_settings == null)
                throw new ArgumentNullException("Api Settings is null. Please Check your application settings.");

            this.api_settings = api_settings;
            this.certificateService = new CertificateService(api_settings);
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var ignoreCertificateAttribute = GetIgnoreCertificateAuthorizeAttribute(actionContext.ActionDescriptor);

            // if the controller or the action method has ignore cert attribtue dont do cert check.
            if (ignoreCertificateAttribute != null)
            {
                Logger.Instance.Info(string.Format("The controller or action method has ignore cert attribute and hence no cert check is required - {0}", actionContext.Request.RequestUri.AbsoluteUri));
                return;
            }

            // if the send cert from the config is false then ignore cert check.
            if (!this.api_settings.Check_For_Cert)
            {
                Logger.Instance.Info(string.Format("The application is configured to ignore certificate check in the application settings"));
                return;
            }

            var clientCertificate = actionContext.RequestContext.ClientCertificate;
            if (clientCertificate == null)
            {
                Logger.Instance.Info(string.Format("The certificate is not sent in the request - {0}", actionContext.Request.RequestUri.AbsoluteUri));
                SetErrorResponse(actionContext);
                return;
            }

            Logger.Instance.Info(string.Format("Authorizing certificate {0} from the request - {1}", clientCertificate.Thumbprint, actionContext.Request.RequestUri.AbsoluteUri));
            var serverCertificate = this.certificateService.GetCertificate(this.api_settings.ServerCertThumbprint);
            if (serverCertificate == null)
            {
                Logger.Instance.Info(string.Format("The certificate is null from the server - {0}", actionContext.Request.RequestUri.AbsoluteUri));
                SetErrorResponse(actionContext);
                return;
            }

            if (serverCertificate.Equals(clientCertificate))
            {
                Logger.Instance.Info(string.Format("The server certifcate {0} and client certificate {1} are equal.", serverCertificate.Thumbprint, clientCertificate.Thumbprint));
                return;
            }
            else
            {
                Logger.Instance.Error(string.Format("The server certifcate {0} and client certificate {1} are not equal.", serverCertificate.Thumbprint, clientCertificate.Thumbprint));
            }

            SetErrorResponse(actionContext);
        }

        protected override void HandleUnauthorizedRequest(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            SetErrorResponse(actionContext);
        }

        private void SetErrorResponse(HttpActionContext actionContext)
        {
            Logger.Instance.Info(string.Format("The user is not authorized to access the url - {0}", actionContext.Request.RequestUri.AbsoluteUri));
            var customError = new HttpError("Sorry, you do not have the required presmission to perform this action.") { };
            actionContext.Response = actionContext.Request.CreateErrorResponse(
                   HttpStatusCode.Unauthorized,
                   customError
               );
        }

        private IgnoreCertificateAuthorize GetIgnoreCertificateAuthorizeAttribute(HttpActionDescriptor actionDescriptor)
        {
            // Check if the attribute exists on the action method.
            var attribute = (IgnoreCertificateAuthorize)actionDescriptor.GetCustomAttributes<IgnoreCertificateAuthorize>().SingleOrDefault();
            if (attribute != null)
            {
                return attribute;
            }

            // Check if the attribute exists on the controller
            return (IgnoreCertificateAuthorize)actionDescriptor.ControllerDescriptor.GetCustomAttributes<IgnoreCertificateAuthorize>().SingleOrDefault();
        }
    }
}