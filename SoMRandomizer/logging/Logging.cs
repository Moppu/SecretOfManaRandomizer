using System;
using System.Collections.Generic;
using System.Reflection;

namespace SoMRandomizer.logging
{
    /// <summary>
    /// Combined utility class and logging interface for sending messages around.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public abstract class Logging
    {
        public static Dictionary<string, List<Logging>> specificMessageTypeLoggers = new Dictionary<string, List<Logging>>();
        public static List<MethodBase> debugMethods = new List<MethodBase>();
        public static void AddLogger(Logging logger)
        {
            AddLogger("general", logger);
        }
        public static void AddLogger(string messageType, Logging logger)
        {
            if (logger is NullWriter)
            {
                return;
            }

            if (specificMessageTypeLoggers.TryGetValue(messageType, out List<Logging> loggers))
            {
                loggers.Add(logger);
            }
            else
            {
                specificMessageTypeLoggers[messageType] = new List<Logging>{logger};
            }
        }

        public static void ClearLoggers()
        {
            specificMessageTypeLoggers.Clear();
        }

        public static bool HasLogger(string messageType)
        {
            return specificMessageTypeLoggers.TryGetValue(messageType, out List<Logging> loggers) && loggers.Count > 0;
        }

        // implementation method
        public abstract void logMessage(String msg);
        public abstract void forceLogFlush();
        public abstract void close();
        
        public static bool debugEnabled = false;

        // log with general category/file
        public static void log(string msg)
        {
            log(msg, "general");
        }

        // log with specific category/file
        public static void log(string msg, string messageType)
        {
            if (specificMessageTypeLoggers.TryGetValue(messageType, out List<Logging> loggers))
            {
                foreach (Logging logger in loggers)
                {
                    logger.logMessage("[" + messageType + "] " + msg);
                }
            }
        }
    }
}
