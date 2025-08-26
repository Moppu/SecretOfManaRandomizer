using System.Collections.Generic;

namespace SoMRandomizer.logging
{
    /// <summary>
    /// A logger for testing that tracks the messages logged so they can be searched later to verify certain events happened correctly.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class DebugLogger : Logging
    {
        public List<string> logMessages = new List<string>();
        public override void close()
        {
            // nothing
        }

        public override void forceLogFlush()
        {
            // nothing
        }

        public override void logMessage(string msg)
        {
            logMessages.Add(msg);
        }
    }
}
