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
using SoMRandomizer.forms;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.openworld;
using SoMRandomizer.util;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SoMRandomizer
{
    /// <summary>
    /// Main entry point for mana rando app
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    static class Program
    {
        [DllImport("kernel32.dll")]
        static extern bool AttachConsole(int dwProcessId);

        private const int ATTACH_PARENT_PROCESS = -1;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string[] cmdLine = Environment.GetCommandLineArgs();
            // arg [0] is the path to the binary
            // if any args are passed, process as a command-line rom generate and don't open the UI
            if (cmdLine.Length <= 1)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
            else
            {
                // workaround for console under mono environment; need to call this for console to appear in windows env
                try
                {
                    AttachConsole(ATTACH_PARENT_PROCESS);
                }
                catch
                {
                    // ignore
                }

                // process commandline args for open world.  require all of these:
                // srcRom=""
                // dstRom=""
                // seed=""
                // options=""

                // note that this currently only supports open world mode, though it wouldn't be too hard to make it run for any mode.
                try
                {
                    Dictionary<string, string> cmdArgsProcessed = CmdArgParser.processCmdArgs(cmdLine);
                    if (!cmdArgsProcessed.ContainsKey("srcRom"))
                    {
                        Console.WriteLine("missing srcRom=(path)");
                        Environment.Exit(1);
                    }
                    if (!cmdArgsProcessed.ContainsKey("dstRom"))
                    {
                        Console.WriteLine("missing dstRom=(path)");
                        Environment.Exit(1);
                    }
                    if (!cmdArgsProcessed.ContainsKey("seed"))
                    {
                        Console.WriteLine("missing seed=(value)");
                        Environment.Exit(1);
                    }
                    if (!cmdArgsProcessed.ContainsKey("options"))
                    {
                        Console.WriteLine("missing options=(value)");
                        Environment.Exit(1);
                    }

                    // process individual options, similar to how OptionsManager does it for the UI
                    string[] allEntries = cmdArgsProcessed["options"].Trim().Split(new char[] { ' ' });
                    Dictionary<string, string> allEntriesMap = new Dictionary<string, string>();
                    foreach (string entry in allEntries)
                    {
                        string str = entry.Trim();
                        int equalsIndex = str.IndexOf('=');
                        List<string> values = new List<string>();
                        if (equalsIndex >= 0)
                        {
                            values.Add(str.Substring(0, equalsIndex));
                            values.Add(str.Substring(equalsIndex + 1));
                        }
                        if (values.Count == 2)
                        {
                            allEntriesMap[values[0]] = values[1];
                        }
                        else
                        {
                            Console.WriteLine("Unexpected string: " + entry);
                            Environment.Exit(1);
                        }
                    }

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
                    // run rom generation
                    // note there are no checks here for whether the dstRom exists - it will overwrite
                    try
                    {
                        RomGenerator.initGeneration(cmdArgsProcessed["srcRom"], cmdArgsProcessed["dstRom"], cmdArgsProcessed["seed"], generatorsByRomType, commonSettings, settingsByRomType);
                        Console.WriteLine("done!");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error: " + e.Message);
                    }
                }
                catch(Exception ee)
                {
                    Console.WriteLine("exception encountered: " + ee.Message);
                }
            }
        }
    }
}
