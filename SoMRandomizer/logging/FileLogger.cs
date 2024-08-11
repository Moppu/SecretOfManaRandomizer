using System;
using System.IO;

namespace SoMRandomizer.logging
{
    /// <summary>
    /// Output log messages to a file.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class FileLogger : Logging
    {
        string logFilename;
        StreamWriter fileWriter;

        public FileLogger(string logFilename)
        {
            this.logFilename = logFilename;
            fileWriter = new StreamWriter(logFilename, false);
        }

        public override void logMessage(string msg)
        {
            string msg2 = "";
            DateTime d = DateTime.Now;
            string h = d.Hour.ToString("D2");
            string m = d.Minute.ToString("D2");
            string s = d.Second.ToString("D2");
            string dt = h + ":" + m + ":" + s;
            msg2 = dt + " - " + msg;

            fileWriter.WriteLine(msg2);
            fileWriter.Flush();
            fileWriter.BaseStream.Flush();
        }

        public override void forceLogFlush()
        {
        }

        public override void close()
        {
            fileWriter.Close();
        }
    }
}
