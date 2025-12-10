using Microsoft.Extensions.Logging.Abstractions;
using NLog;
using System;

namespace AccessControlAPI.Utils
{
    public class LogHelper
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        public void WriteLog(
                    NLog.LogLevel level,
                    int? userId,
                    int? roleId,
                    string action,
                    bool success,
                    string message)
        {
            var logEvent = new LogEventInfo(level, logger.Name, message);

            logEvent.Properties["UserId"] = userId.ToString() ?? "";
            logEvent.Properties["RoleId"] = roleId.ToString() ?? "";
            logEvent.Properties["Action"] = action ?? "";
            logEvent.Properties["Success"] = success;

            logger.Log(logEvent);
        }
    }
}
