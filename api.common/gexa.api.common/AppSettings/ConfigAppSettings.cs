using System;
using System.Configuration;

namespace api.common.AppSettings
{
    public class ConfigAppSettings : IAppSettings
    {
        public T Get<T>(string name)
        {
            var configValue = (ConfigurationManager.AppSettings[name]);
            if (configValue == null)
                configValue = default(T) == null ? string.Empty : default(T).ToString();

            return (T)Convert.ChangeType(configValue, typeof(T));
        }
    }
}
