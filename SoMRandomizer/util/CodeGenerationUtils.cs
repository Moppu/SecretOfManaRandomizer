using SoMRandomizer.logging;

namespace SoMRandomizer.util
{
    /// <summary>
    /// Utilities for generating assembly code.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class CodeGenerationUtils
    {
        /// <summary>
        /// Make sure we don't overrun bank boundaries
        /// </summary>
        public static bool ensureSpaceInBank(ref int newCodeOffset, int numBytes)
        {
            Logging.log("Checking for x" + numBytes.ToString("X") + " bytes available in-bank at " + newCodeOffset.ToString("X"), "debug");
            byte bank1 = (byte)(newCodeOffset >> 16);
            byte bank2 = (byte)((newCodeOffset + numBytes) >> 16);
            if (bank1 != bank2)
            {
                newCodeOffset = (bank2 << 16);
                Logging.log("Not enough space.  Moving pointer to " + newCodeOffset.ToString("X"), "debug");
                return true;
            }
            return false;
        }
    }
}
