using gexa.api.common.Filters;
using gexa.azure.api.Filters;
using gexa.azure.api.Models;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace gexa.azure.api
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(HttpConfiguration config, IUnityContainer container)
        {
            config.Filters.Add(new ExceptionFilter());
            config.Filters.Add(container.Resolve<CertificateAuthorizationFilter>());
            config.Filters.Add(container.Resolve<TokenAuthorizationFilter>());
        }
    }
}