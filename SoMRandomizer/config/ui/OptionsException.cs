using System;

namespace SoMRandomizer.config.ui
{
    /// <summary>
    /// An exception that occurred while processing user-entered options.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class OptionsException : Exception
    {
        // null if no version, or false if entry didn't match the current
        public bool? versionMatch;
        // error description
        public string valueError;

        public OptionsException(bool? versionMatch, string valueError)
        {
            this.versionMatch = versionMatch;
            this.valueError = valueError;
        }
    }
}
