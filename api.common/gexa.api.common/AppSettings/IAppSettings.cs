
namespace api.common.AppSettings
{
    public interface IAppSettings
    {
        T Get<T>(string name);
    }
}
