using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using SoMRandomizer.logging;

namespace SoMRandomizer.config
{
    /// <summary>
    /// Parser and accessor for program-wide configuration values read from a properties file.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class AppConfig : PropertyFileReader
    {
        private string filename;
        public static AppConfig APPCONFIG;
        static AppConfig()
        {
            APPCONFIG = new AppConfig();
        }
        public AppConfig()
        {
            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string specificFolder = Path.Combine(appDataFolder, "SomRandomizer");
            string configFile = Path.Combine(specificFolder, "config.properties");
            try
            {
                if (!Directory.Exists(specificFolder))
                {
                    Directory.CreateDirectory(specificFolder);
                }
                if (!File.Exists(configFile))
                {
                    // previous versions kept config with the exe, so if we've got one of those, copy it into the new appdata area
                    string localConfig = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "/config.properties";
                    if (File.Exists(localConfig))
                    {
                        File.Copy(localConfig, configFile);
                    }
                }
                filename = configFile;
            }
            catch(Exception e)
            {
                // can't write to appdata area for some reason, so i guess use local config
                filename = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "/config.properties";
            }
            loadConfigFile();
        }
        public AppConfig(string filename)
        {
            this.filename = filename;
            loadConfigFile();
        }
        Dictionary<string, string> defaultProperties = new Dictionary<string, string>();
        Dictionary<string, string> configProperties = new Dictionary<string, string>();

        public void addDefaultProperty(string key, string value)
        {
            Logging.log("Adding default property: " + key + " -> " + value, "debug");
            // -> defaultProperties
            defaultProperties[key] = value;
        }

        private void loadConfigFile()
        {
            Logging.log("Loading config file", "debug");
            if (!File.Exists(filename))
            {
                Logging.log("Creating blank config file", "debug");
                StreamWriter sw = File.CreateText(filename);
                sw.Close();
            }
            Logging.log("Processing config file", "debug");
            // -> configProperties
            configProperties = readFile(filename);
        }

        public void setConfigProperty(string key, string value)
        {
            // -> configProperties
            configProperties[key] = value;
            // -> file
            writePropertyFile(filename, configProperties);
        }

        public string getStringProperty(string key)
        {
            string p = null;
            try
            {
                p = configProperties[key];
            } catch(KeyNotFoundException e) 
            {
                try
                {
                    p = defaultProperties[key];
                }
                catch (KeyNotFoundException ee)
                {
                    p = null;
                }
            }
            return p;
        }

        public int getIntProperty(string key)
        {
            string p = getStringProperty(key);
            if (p != null)
            {
                return Int32.Parse(p);
            }
            return 0;
        }
    }
}
