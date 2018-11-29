using log4net;

namespace api.common.Extension
{
    public static class SecurityExtensions
    {
        static readonly log4net.Core.Level networkLevel = new log4net.Core.Level(50000, "Network");

        public static void LogNetwork(this ILog log, string message)
        {
            log.Logger.Log(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, networkLevel, message, null);
        }
    }
}
