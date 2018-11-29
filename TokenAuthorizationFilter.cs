using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Net.Http;
using System.Net;
using System.Security.Principal;
using System.Threading;
using System.Web;
using gexa.api.common;
using gexa.azure.api.Models;
using System.Security.Claims;
using gexa.azure.datamodels.User;
using gexa.api.common.ApiExtensions;
using gexa.api.common.Constants;

namespace gexa.azure.api.Filters
{
    public class TokenAuthorizationFilter : AuthorizeAttribute
    {
        private readonly Azure_Api_Settings api_settings;

        public TokenAuthorizationFilter(Azure_Api_Settings api_settings)
        {
            if (api_settings == null)
                throw new ArgumentNullException("Api Settings is null. Please Check your application settings.");

            this.api_settings = api_settings;
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (SkipAuthorization(actionContext))
                return;

            if (!this.api_settings.AuthorizationTokenCheck)
                return;

            var token = actionContext.Request.GetApiHeaderToken();
            if (!string.IsNullOrWhiteSpace(token))
            {
                if (token.Equals(this.api_settings.GexaAdminToken, StringComparison.CurrentCultureIgnoreCase))
                    return;

                var user = DataHelper.Users.IsTokenValid(token);
                if (user != null && Affirm_Access(actionContext, user))
                {
                    // Extend the user session by TokenExpiryInMinutes
                    DataHelper.Users.ExtendUserSession(token, this.api_settings.TokenExpiryInMinutes);
                    SetClaimsPrincipal(user);
                    return;
                }
            }

            base.OnAuthorization(actionContext);
        }

        private Boolean Affirm_Access(HttpActionContext actionContext, User usr)
        {
            if (actionContext.ActionDescriptor.GetCustomAttributes<Auth_AssertionAttribute>(true).Any() )
            {
                Boolean result = false;
                Func<Auth_AssertionAttribute, Boolean> checker = att=>
                {
                    var parse_val = (string)(att.Location == "Route" ? actionContext.ControllerContext.RouteData.Values[att.Attribute_key]
                                        : actionContext.Request.GetQueryNameValuePairs().Where(u => u.Key.IndexOf(att.Attribute_key, StringComparison.OrdinalIgnoreCase) >= 0)
                                            .DefaultIfEmpty(new KeyValuePair<string, string>(string.Empty, string.Empty)).First().Value);

                    if (att.Auth_Type == Auth_Type.Direct)
                    {
                        var affirm = usr.Account_permissions
                            .Where(t => t.AccountType == att.Claim_Type && t.AccountNumber == parse_val
                                  ).FirstOrDefault();

                        return (affirm != null);
                    }
                    else if (att.Auth_Type == Auth_Type.PayMethod)
                    {
                        var customer = usr.Account_permissions.Where(t => t.AccountType == AccountType.Customer_Account_Id).FirstOrDefault();
                        if(customer == null) return false;
                        return string.Format("{0}-1", customer.AccountNumber) == parse_val;
                    }
                    else if (att.Auth_Type == Auth_Type.AutoPay)
                    {
                        var customer = usr.Account_permissions.Where(t => t.AccountType == AccountType.Customer_Account_Id).FirstOrDefault();
                        if (customer == null) return false;
                        var autoPays = DataHelper.GetCustomerAutoPay(parse_val);
                        if (!autoPays.Any()) return false;
                        return string.Format("{0}-1", customer.AccountNumber) == autoPays.FirstOrDefault().User_Key;
                    }
                    else
                    {
                        return false;
                    }
                };

                actionContext.ActionDescriptor.GetCustomAttributes<Auth_AssertionAttribute>().ToList().ForEach(o =>
                {
                    result = result || checker(o);
                });

                return result;
            }
            return true;
        }
        private void SetClaimsPrincipal(User user)
        {
            var identity = new GenericIdentity(user.Profile.Username);

            // TODO add roles when ready.
            var roles = new string[0];
            var principal = new GenericPrincipal(identity, roles);

            var claimsPrincipal = new ClaimsPrincipal(principal);
            claimsPrincipal.AddIdentity(GetClaimsIdentity(user));

            Thread.CurrentPrincipal = claimsPrincipal;
            if (HttpContext.Current != null)
            {
                HttpContext.Current.User = claimsPrincipal;
            }
        }

        private ClaimsIdentity GetClaimsIdentity(User user)
        {
            var claims = new List<Claim>();
            foreach (var permission in user.Account_permissions)
            {
                var claim = new Claim(permission.AccountType.ToString(), permission.AccountNumber);
                claims.Add(claim);
            }

            claims.Add(new Claim(ClaimTypes.Email, user.Profile.Email_Address));
            claims.Add(new Claim(Constants.UserName, user.Profile.Username));

            var claimsIdentity = new ClaimsIdentity(claims, Constants.ClaimsIdentityName);
            return claimsIdentity;
        }

        protected override void HandleUnauthorizedRequest(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            var error = new HttpError("UnAuthorized Exception. Authorization token has expired or is not valid.") { };
            actionContext.Response = actionContext.Request.CreateErrorResponse(
                   HttpStatusCode.Unauthorized,
                   error
               );
        }

        private static bool SkipAuthorization(HttpActionContext actionContext)
        {
            return actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>(true).Any()
                   || actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>(true).Any();
        }
    }
}