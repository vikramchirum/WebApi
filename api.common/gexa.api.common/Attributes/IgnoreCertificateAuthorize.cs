using System;

namespace api.common.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class IgnoreCertificateAuthorize : Attribute
    {
        public IgnoreCertificateAuthorize()
        {
        }
    }
}