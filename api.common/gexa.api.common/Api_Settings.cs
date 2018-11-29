
using System.Collections.Generic;

namespace api.common
{
    public class Api_Settings_Base
    {
        public string Api_Url { get; set; }

        public Dictionary<string, string> Headers { get; set; }

        public bool Check_For_Cert
        {
            get;
            set;
        }

        public string ServerCertThumbprint
        {
            get;
            set;
        }

        public bool IsAzureHosted
        {
            get;
            set;
        }

        public int TokenExpiryInMinutes
        {
            get;
            set;
        }
    }

    public class Azure_Api_Settings : Api_Settings_Base
    {
        public bool Send_Cert
        {
            get;
            set;
        }

        public string ClientCertThumbprint
        {
            get;
            set;
        }

        public bool AuthorizationTokenCheck
        {
            get;
            set;
        }

        public bool ExpUserId
        {
            get;
            set;
        }

        public bool ExpPassword
        {
            get;
            set;
        }

        public bool ExpDBHost
        {
            get;
            set;
        }

        public bool ExpECALSURL
        {
            get;
            set;
        }

        public bool ExpEAI
        {
            get;
            set;
        }

        public bool ExpPreamble
        {
            get;
            set;
        }

        public bool ExpSubCode
        {
            get;
            set;
        }

        public bool ExpVendor
        {
            get;
            set;
        }

        public bool Documents_Url
        {
            get;
            set;
        }

        public byte[] Crypto_Key { get; set; }

        public byte[] Crypto_Iv { get; set; }

    }

    public class External_Api_Settings : Api_Settings_Base
    {
        public string DocumentsUrl
        {
            get;
            set;
        }

        public string Address_Search_Endpoint
        {
            get;
            set;
        }

        public string Address_Search_Identity
        {
            get;
            set;
        }
    }
}