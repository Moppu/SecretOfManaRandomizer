/*
    Secret of Mana Randomizer
    Copyright (C) 2024  Moppleton

    This library is free software; you can redistribute it and/or
    modify it under the terms of the GNU Lesser General Public
    License as published by the Free Software Foundation; either
    version 2.1 of the License, or (at your option) any later version.

    This library is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
    Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public
    License along with this library; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301
    USA
 */
using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.openworld;
using SoMRandomizer.util;
using System;
using System.Collections.Generic;

namespace SoMRandomizer
{
    /// <summary>
    /// Headless open world ROM generation driven by command-line arguments.
    /// </summary>
    ///
    /// <remarks>
    /// This class must not depend on Windows Forms or System.Drawing. It is shared by the
    /// Windows Forms entry point in Program.cs and by the cross-platform SoMRandomizer.Cli
    /// project, which compiles it against .NET 8 on macOS and Linux.
    ///
    /// Expected arguments, all required: srcRom=(path) dstRom=(path) seed=(value) options=(value).
    /// The options value is a space-separated list of key=value pairs, in the same form the UI
    /// produces. Only open world mode is supported.
    ///
    /// The destination ROM is overwritten without confirmation if it already exists.
    /// </remarks>
    public static class CommandLineGenerator
    {
        public const int EXIT_SUCCESS = 0;
        public const int EXIT_USAGE_ERROR = 1;
        public const int EXIT_GENERATION_ERROR = 2;

        private static readonly string[] REQUIRED_ARGS = { "srcRom", "dstRom", "seed", "options" };
        private static readonly string[] REQUIRED_ARG_HINTS = { "(path)", "(path)", "(value)", "(value)" };

        /// <summary>
        /// Parse the command line and generate an open world ROM.
        /// </summary>
        /// <param name="cmdLine">Full command line in the form returned by
        /// Environment.GetCommandLineArgs, where element 0 is the program path and is ignored.</param>
        /// <returns>EXIT_SUCCESS on success, EXIT_USAGE_ERROR for missing or malformed
        /// arguments, or EXIT_GENERATION_ERROR if ROM generation throws.</returns>
        public static int Run(string[] cmdLine)
        {
            if (cmdLine == null)
            {
                Console.WriteLine("missing command line");
                return EXIT_USAGE_ERROR;
            }

            Dictionary<string, string> cmdArgsProcessed;
            Dictionary<string, string> allEntriesMap;
            try
            {
                cmdArgsProcessed = CmdArgParser.processCmdArgs(cmdLine);
                for (int i = 0; i < REQUIRED_ARGS.Length; i++)
                {
                    if (!cmdArgsProcessed.ContainsKey(REQUIRED_ARGS[i]))
                    {
                        Console.WriteLine("missing " + REQUIRED_ARGS[i] + "=" + REQUIRED_ARG_HINTS[i]);
                        return EXIT_USAGE_ERROR;
                    }
                }
                allEntriesMap = parseOptions(cmdArgsProcessed["options"]);
                if (allEntriesMap == null)
                {
                    return EXIT_USAGE_ERROR;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("exception encountered: " + e.Message);
                return EXIT_USAGE_ERROR;
            }

            try
            {
                // create default settings and apply our overrides
                CommonSettings commonSettings = new CommonSettings();
                OpenWorldSettings openWorldSettings = new OpenWorldSettings(commonSettings);
                // set a few common options for the log that the UI normally sets
                commonSettings.set(CommonSettings.PROPERTYNAME_MODE, OpenWorldSettings.MODE_KEY);
                commonSettings.set(CommonSettings.PROPERTYNAME_ALL_ENTERED_OPTIONS, cmdArgsProcessed["options"]);
                commonSettings.set(CommonSettings.PROPERTYNAME_VERSION, RomGenerator.VERSION_NUMBER);

                openWorldSettings.processNewSettings(allEntriesMap);
                OpenWorldGenerator openWorldGenerator = new OpenWorldGenerator();
                Dictionary<string, RomGenerator> generatorsByRomType = new Dictionary<string, RomGenerator> { { OpenWorldSettings.MODE_KEY, openWorldGenerator } };
                Dictionary<string, RandoSettings> settingsByRomType = new Dictionary<string, RandoSettings> { { OpenWorldSettings.MODE_KEY, openWorldSettings } };
                RomGenerator.initGeneration(cmdArgsProcessed["srcRom"], cmdArgsProcessed["dstRom"], cmdArgsProcessed["seed"], generatorsByRomType, commonSettings, settingsByRomType);
                Console.WriteLine("done!");
                return EXIT_SUCCESS;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                return EXIT_GENERATION_ERROR;
            }
        }

        /// <summary>
        /// Split the options value into key=value pairs, similar to how OptionsManager does it for the UI.
        /// </summary>
        /// <returns>The parsed map, or null after printing a message if any entry is not of the form key=value.</returns>
        private static Dictionary<string, string> parseOptions(string options)
        {
            string[] allEntries = options.Trim().Split(new char[] { ' ' });
            Dictionary<string, string> allEntriesMap = new Dictionary<string, string>();
            foreach (string entry in allEntries)
            {
                string str = entry.Trim();
                int equalsIndex = str.IndexOf('=');
                if (equalsIndex < 0)
                {
                    Console.WriteLine("Unexpected string: " + entry);
                    return null;
                }
                allEntriesMap[str.Substring(0, equalsIndex)] = str.Substring(equalsIndex + 1);
            }
            return allEntriesMap;
        }
    }
}
