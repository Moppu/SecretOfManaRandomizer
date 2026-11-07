using System;

namespace SoMRandomizer.Cli
{
    /// <summary>
    /// Cross-platform entry point. Runs headless open world generation only, because the
    /// Windows Forms user interface is not available under .NET 8 on macOS or Linux.
    /// </summary>
    internal static class CliProgram
    {
        private static int Main(string[] args)
        {
            // CommandLineGenerator expects the Environment.GetCommandLineArgs layout, where
            // element 0 is the program path, so prepend a placeholder to the user arguments.
            string[] cmdLine = new string[args.Length + 1];
            cmdLine[0] = "SoMRandomizer.Cli";
            Array.Copy(args, 0, cmdLine, 1, args.Length);
            if (args.Length == 0)
            {
                Console.WriteLine("usage: SoMRandomizer.Cli srcRom=(path) dstRom=(path) seed=(value) options=\"key=value key=value ...\"");
                return CommandLineGenerator.EXIT_USAGE_ERROR;
            }
            return CommandLineGenerator.Run(cmdLine);
        }
    }
}
