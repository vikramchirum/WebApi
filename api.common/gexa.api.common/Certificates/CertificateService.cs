using api.common.Log;

using System;
using System.Security.Cryptography.X509Certificates;

namespace api.common.Certificates
{
    public class CertificateService : ICertificateService
    {
        private readonly Api_Settings_Base api_settings;

        public CertificateService(Api_Settings_Base api_settings)
        {
            if (api_settings == null)
            {
                throw new ArgumentNullException("Api Settings is null. Please Check your application settings.");
            }

            this.api_settings = api_settings;
        }

        public X509Certificate2 GetCertificate(string certThumbprint)
        {
            try
            {
                Logger.Instance.Info(string.Format("The certificate thumb print from the application config is {0}", certThumbprint));
                
                X509Store certificateStore;

                if (this.api_settings.IsAzureHosted)
                {
                    Logger.Instance.Info(string.Format("The api is hosted on azure and the certificate is retrieved from azure StoreLocation of CurrentUser"));
                    certificateStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                }
                else
                {
                    Logger.Instance.Info(string.Format("The api is not hosted on azure and the certificate is retrieved from azure StoreLocation of LocalMachine"));
                    certificateStore = new X509Store(StoreName.My, StoreLocation.LocalMachine);
                }

                certificateStore.Open(OpenFlags.ReadOnly);
                var certCollection = certificateStore.Certificates.Find(X509FindType.FindByThumbprint, certThumbprint, false);
                certificateStore.Close();

                var cert = new X509Certificate2();
                if (certCollection.Count > 0)
                {
                    Logger.Instance.Info(string.Format("The certificate is found for server thumb print {0}", certThumbprint));
                    return certCollection[0];
                }

                Logger.Instance.Info(string.Format("The certificate is not found for server thumb print {0}", certThumbprint));
                return null;
            }
            catch (Exception ex)
            {
                Logger.Instance.Fatal(string.Format("Error in getting the certificate for server thumb print {0}", certThumbprint));
                Logger.Instance.Error(ex.Message, ex);
                return null;
            }
        }
    }
}
