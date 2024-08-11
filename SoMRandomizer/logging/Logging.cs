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
            List<Logging> loggers;
            if(specificMessageTypeLoggers.ContainsKey(messageType))
            {
                loggers = specificMessageTypeLoggers[messageType];
            }
            else
            {
                loggers = new List<Logging>();
                specificMessageTypeLoggers[messageType] = loggers;
            }
            loggers.Add(logger);
        }

        public static void ClearLoggers()
        {
            specificMessageTypeLoggers.Clear();
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
            if (specificMessageTypeLoggers.ContainsKey(messageType))
            {
                foreach (Logging logger in specificMessageTypeLoggers[messageType])
                {
                    logger.logMessage("[" + messageType + "] " + msg);
                }
            }
        }
    }
}
