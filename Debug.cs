using System.Collections.Generic;

namespace DDA
{
    static class Debug
    {
        static readonly List<DebugLog> logs = new List<DebugLog>();

        public static IReadOnlyList<DebugLog> Logs
        {
            get
            {
                return logs;
            }
        }

        public static void Log(object value, string caption)
        {
            logs.Add(new DebugLog(value.ToString(), caption));
        }
        public static void Log(object value)
        {
            logs.Add(new DebugLog(value.ToString(), string.Empty));
        }
        public static void ClearLogs()
        {
            logs.Clear();
        }
    }
}
