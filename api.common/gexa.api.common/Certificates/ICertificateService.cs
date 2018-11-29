using System.Security.Cryptography.X509Certificates;

namespace api.common.Certificates
{
    public interface ICertificateService
    {
        X509Certificate2 GetCertificate(string certThumbprint);
    }
}
