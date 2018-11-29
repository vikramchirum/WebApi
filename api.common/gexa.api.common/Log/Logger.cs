using api.common.Extension;

using log4net;
using System;

namespace api.common.Log
{
    public class Logger : ILogger
    {
        private readonly ILog log;

        public Logger(Type type)
        {
            if (type == null)
            {
                throw new InvalidOperationException("Type cannot be null for logger.");
            }

            this.log = LogManager.GetLogger(type);
        }

        private static Logger _instance = new Logger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static Logger Instance
        {
            get
            {
                return Logger._instance;
            }
        }

        public void LogNetwork(string message)
        {
            this.log.LogNetwork(message);
        }

        public void Debug(object message)
        {
            this.log.Debug(message);
        }

        public void Debug(object message, Exception exception)
        {
            this.log.Debug(message, exception);
        }

        public void Error(object message)
        {
            this.log.Error(message);
        }

        public void Error(object message, Exception exception)
        {
            this.log.Error(message, exception);
        }

        public void Fatal(object message)
        {
            this.log.Fatal(message);
        }

        public void Fatal(object message, Exception exception)
        {
            this.log.Fatal(message, exception);
        }

        public void Info(object message)
        {
            this.log.Info(message);
        }

        public void Info(object message, Exception exception)
        {
            this.log.Info(message, exception);
        }

        public void Warn(object message)
        {
            this.log.Warn(message);
        }

        public void Warn(object message, Exception exception)
        {
            this.log.Warn(message, exception);
        }
    }
}