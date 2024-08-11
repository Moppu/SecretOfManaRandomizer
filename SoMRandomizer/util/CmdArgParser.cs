using System;
using System.Collections.Generic;

namespace SoMRandomizer.util
{
    /// <summary>
    /// Utility to process command-line arguments of the form key=value into a dictionary of the keys to the values.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class CmdArgParser
    {
        public static Dictionary<string, string> processCmdArgs(string[] args)
        {
            Dictionary<string, string> processed = new Dictionary<string, string>();
            // arg [0] is the path to the binary
            for (int i=1; i < args.Length; i++)
            {
                Console.WriteLine(args[i]);
                int equalsIndex = args[i].IndexOf('=');
                if(equalsIndex > 0)
                {
                    string argName = args[i].Substring(0, equalsIndex);
                    string argValue = args[i].Substring(equalsIndex + 1);
                    processed[argName] = argValue;
                }
            }
            return processed;
        }
    }
}
