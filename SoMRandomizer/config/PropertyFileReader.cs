using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using SoMRandomizer.logging;

namespace SoMRandomizer.config
{
    /// <summary>
    /// Parser for .ini/.properties files; key=value
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class PropertyFileReader
    {

        public Dictionary<string, string> readByteArray(byte[] data)
        {
            try
            {
                StreamReader sr = new StreamReader(new MemoryStream(data), Encoding.Default);
                return readPropertyStream(sr);
            }
            catch (Exception e)
            {
                Logging.log(e.ToString());
                return new Dictionary<string, string>();
            }
        }

        public Dictionary<string, string> readFile(string filename)
        {
            try
            {
                StreamReader sr = new StreamReader(new FileStream(filename, FileMode.Open), Encoding.Default);
                return readPropertyStream(sr);
            }
            catch (Exception e)
            {
                Logging.log(e.ToString());
                return new Dictionary<string, string>();
            }
        }

        public Dictionary<string, string> readPropertyResourceFile(string resourceName)
        {
            try
            {
                Assembly assemb = Assembly.GetExecutingAssembly();
                string[] names = assemb.GetManifestResourceNames();
                try
                {
                    Stream stream = assemb.GetManifestResourceStream("SoMRandomizer.Resources." + resourceName);
                    StreamReader reader = new StreamReader(stream, Encoding.Default);
                    return readPropertyStream(reader);
                }
                catch (Exception e)
                {
                    return new Dictionary<string, string>();
                }
            }
            catch (Exception e)
            {
                Logging.log(e.ToString());
                return new Dictionary<string,string>();
            }
        }

        private Dictionary<string, string> readPropertyStream(StreamReader sr)
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();
            string line = sr.ReadLine();
            while (line != null)
            {
                line = line.Trim();
                if (!line.StartsWith("#"))
                {
                    int equals = line.IndexOf('=');
                    if (equals > 0)
                    {
                        string before = line.Substring(0, equals).Trim();
                        string after = line.Substring(equals + 1).Trim();
                        if (before.Length > 0 && after.Length > 0)
                        {
                            ret[before] = after;
                        }
                    }
                }
                line = sr.ReadLine();
            }
            sr.Close();
            return ret;
        }

        protected void writePropertyFile(string filename, Dictionary<string, string> data)
        {
            try
            {
                FileStream fs = new FileStream(filename, FileMode.Create);
                StreamWriter sw = new StreamWriter(fs);
                foreach (string key in data.Keys)
                {
                    sw.WriteLine(key + "=" + data[key]);
                }
                sw.Close();
            }
            catch (Exception e)
            {
                Logging.log(e.ToString());
            }
        }
    }
}
