
namespace SoMRandomizer.logging
{
    /// <summary>
    /// Non-logger used for when certain types of logging are disabled.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class NullWriter : Logging
    {
        public override void forceLogFlush()
        {
            // nothing
        }

        public override void logMessage(string msg)
        {
            // nothing
        }

        public override void close()
        {
            // nothing
        }
    }
}
