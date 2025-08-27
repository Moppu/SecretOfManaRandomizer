using System;
using System.Collections.Generic;

namespace SoMRandomizer.util
{
    /// <summary>
    /// Utilities for handling exceptions.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class ErrorUtil
    {
        public static Exception appendExceptionInfo(String errorMsg, Exception e)
        {
            return new Exception(errorMsg +Environment.NewLine + e.Message, e);
        }

        public static void checkNotNull(object obj, string exceptionMessage)
        {
            if (obj == null)
            {
                throw new Exception(exceptionMessage);
            }
        }

        public static void checkAnyEntries<T>(List<T> list, string exceptionMessage)
        {
            if (list == null || list.Count == 0)
            {
                throw new Exception(exceptionMessage);
            }
        }
    }
}
