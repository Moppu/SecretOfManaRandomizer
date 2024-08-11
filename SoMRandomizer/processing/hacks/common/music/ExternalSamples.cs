﻿using SoMRandomizer.logging;
using SoMRandomizer.util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

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
                using (XmlReader reader = XmlReader.Create(xmlLoadPath))
                {
                    XmlWrapperUtil xml = new XmlWrapperUtil(reader);
                    while (reader.Read())
                    {
                        // Only detect start elements.
                        if (reader.IsStartElement())
                        {
                            // Get element name and switch on it.
                            switch (reader.Name)
                            {
                                case "sample":
                                    {
                                        // sample import definition
                                        ImportedSample sample = new ImportedSample();
                                        sample.name = reader["name"];
                                        string filePath = reader["sourceFile"];
                                        string sampleFullPath = xmlLoadDir + "/" + filePath;
                                        byte[] sampleData = File.ReadAllBytes(sampleFullPath);
                                        // prepend with length as with the rest of SoM's samples
                                        sample.data = new byte[2 + sampleData.Length];
                                        sample.data[0] = (byte)sampleData.Length;
                                        sample.data[1] = (byte)(sampleData.Length >> 8);
                                        Array.Copy(sampleData, 0, sample.data, 2, sampleData.Length);
                                        sample.loop = xml.loadRequiredUshortProperty("loop", false);
                                        sample.baseFreq = xml.loadRequiredUshortProperty("baseFreq", false);
                                        sample.adsr = xml.loadRequiredUshortProperty("adsr", true);
                                        sample.newId = startId++;
                                        importedSamples.Add(sample);
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            // handle end block
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
