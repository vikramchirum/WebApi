using gexa.api.common;
using gexa.api.common.AppSettings;
using gexa.azure.mongo.repository;
using Microsoft.Practices.Unity;
using System;
using System.Configuration;
using System.Web.Http;
using Unity.WebApi;
using System.Linq;
using System.Web.Http.Filters;
using gexa.api.common.Filters;
using System.Net.Http;
using gexa.api.common.Certificates;
using System.Net;
using gexa.gems.enrollment_processor.Models;
using gexa.azure.api.Models;

namespace gexa.azure.api
{
    public static class UnityConfig
    {
        public static IUnityContainer RegisterComponents(HttpConfiguration config)
        {
            var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();

            var configAppSettings = new ConfigAppSettings();

            var api_settings = new Azure_Api_Settings();
            api_settings.Api_Url = configAppSettings.Get<string>("GexaExternalServicesBaseUrl");
            api_settings.Send_Cert = configAppSettings.Get<bool>("sendCert");
            api_settings.ClientCertThumbprint = configAppSettings.Get<string>("clientcertthumbprint");
            api_settings.Check_For_Cert = configAppSettings.Get<bool>("checkForCert");
            api_settings.ServerCertThumbprint = configAppSettings.Get<string>("servercertthumbprint");
            api_settings.AuthorizationTokenCheck = configAppSettings.Get<bool>("AuthorizationTokenCheck");
            api_settings.TokenExpiryInMinutes = configAppSettings.Get<int>("TokenExpiryInMinutes");
            api_settings.IsAzureHosted = configAppSettings.Get<bool>("isAzure");
            api_settings.GexaAdminToken = configAppSettings.Get<string>("Gexa_Admin_Token");

            HttpClientHelper.Initialize_HttpClient_Azure(api_settings);
            DataHelper.SetApiSettings(api_settings);

            container.RegisterInstance<Api_Settings_Base>(api_settings);
            container.RegisterInstance<Azure_Api_Settings>(api_settings);



            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
            return container;
        }
    }
}
