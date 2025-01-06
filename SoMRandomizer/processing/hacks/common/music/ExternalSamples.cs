using SoMRandomizer.logging;
using SoMRandomizer.util;
using System;
using System.Collections.Generic;
using System.IO;
using U8Xml;

namespace SoMRandomizer.processing.hacks.common.music
{
    /// <summary>
    /// XML reader for externally configured custom SPC700 samples.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class ExternalSamples
    {
        public static List<ImportedSample> getAllCustomSamples(ref ushort startId)
        {
            List<ImportedSample> importedSamples = new List<ImportedSample>();
            string baseDir = ".";
            string xmlLoadDir = baseDir + "/music";
            string xmlLoadPath = xmlLoadDir + "/samples.xml";
            try
            {
                using (var xml = XmlParser.ParseFile(xmlLoadPath))
                {
                    // NOTE: we only look at top level samples
                    foreach (var node in xml.Root.Children)
                    {
                        if (node.Name.ToString() == "sample")
                        {
                            // sample import definition
                            ImportedSample sample = new ImportedSample();
                            sample.name = node.FindAttribute("name").Value.ToString();
                            string filePath = node.FindAttribute("sourceFile").Value.ToString();
                            string sampleFullPath = xmlLoadDir + "/" + filePath;
                            byte[] sampleData = File.ReadAllBytes(sampleFullPath);
                            // prepend with length as with the rest of SoM's samples
                            sample.data = new byte[2 + sampleData.Length];
                            sample.data[0] = (byte)sampleData.Length;
                            sample.data[1] = (byte)(sampleData.Length >> 8);
                            Array.Copy(sampleData, 0, sample.data, 2, sampleData.Length);
                            XmlWrapperUtil wrap = new XmlWrapperUtil(node);
                            sample.loop = wrap.loadRequiredUshortProperty("loop", false);
                            sample.baseFreq = wrap.loadRequiredUshortProperty("baseFreq", false);
                            sample.adsr = wrap.loadRequiredUshortProperty("adsr", true);
                            sample.newId = startId++;
                            importedSamples.Add(sample);
                        }
                        else
                        {
                            throw new InvalidDataException("Unknown top level node: " + node.Name);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logging.log("Couldn't load external samples: " + e);
                return null;
            }

            return importedSamples;
        }

        public class ImportedSample
        {
            public string name;
            public byte[] data;
            public ushort loop;
            public ushort baseFreq;
            public ushort adsr;
            public ushort newId;
        }

    }
}
