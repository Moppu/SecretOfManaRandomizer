using SoMRandomizer.logging;
using SoMRandomizer.util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace SoMRandomizer.processing.hacks.common.music
{
    /// <summary>
    /// XML reader for externally configured custom music.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class ExternalMusic
    {
        public static List<ImportedSong> getAllCustomSongs(Dictionary<string, ushort> sampleNamesToIds, ref ushort startId)
        {
            string baseDir = ".";
            List<ImportedSong> importedSongs = new List<ImportedSong>();
            string xmlLoadDir = baseDir + "/music";
            string xmlLoadPath = xmlLoadDir + "/music.xml";
            try
            {
                using (XmlReader reader = XmlReader.Create(xmlLoadPath))
                {
                    ImportedSong currentSong = null;
                    XmlWrapperUtil xml = new XmlWrapperUtil(reader);
                    while (reader.Read())
                    {
                        // Only detect start elements.
                        if (reader.IsStartElement())
                        {
                            // Get element name and switch on it.
                            switch (reader.Name)
                            {
                                case "song":
                                    {
                                        ImportedSong song = new ImportedSong();
                                        song.name = reader["name"];
                                        string filePath = reader["sourceFile"];
                                        string songFullPath = xmlLoadDir + "/" + filePath;
                                        song.data = File.ReadAllBytes(songFullPath);
                                        string categoriesString = reader["categories"];
                                        song.echoVal = 0x13;
                                        byte? xmlEchoValue = xml.loadOptionalByteProperty("echo", true);
                                        if(xmlEchoValue != null)
                                        {
                                            song.echoVal = (byte)xmlEchoValue;
                                        }
                                        string[] categories = categoriesString.Split(new char[] { ',' });
                                        song.categories = categories;
                                        song.newId = startId++;
                                        importedSongs.Add(song);
                                        currentSong = song;
                                    }
                                    break;
                                case "sample":
                                    {
                                        if(currentSong != null)
                                        {
                                            // sample table entry
                                            int sampleId = xml.loadRequiredIntProperty("id", false);
                                            string sourceString = reader["source"];
                                            string[] sourceParts = sourceString.Split(new char[] { ' ' });
                                            if(sourceParts.Length == 2 && sourceParts[0] == "vanilla")
                                            {
                                                currentSong.sampleTable[sampleId] = UInt16.Parse(sourceParts[1]);
                                            }
                                            else if (sourceParts.Length == 2 && sourceParts[0] == "custom")
                                            {
                                                currentSong.sampleTable[sampleId] = sampleNamesToIds[sourceParts[1]];
                                            }
                                            else
                                            {
                                                throw new XmlException("unsupported sample source: " + sourceString);
                                            }
                                        }
                                        else
                                        {
                                            throw new XmlException("Stray sample block");
                                        }
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            // handle end block
                            switch (reader.Name)
                            {
                                case "song":
                                    {
                                        currentSong = null;
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logging.log("Couldn't load external music: " + e);
                return null;
            }

            return importedSongs;
        }
        
        public class ImportedSong
        {
            public string name;
            public byte[] data;
            public string[] categories;
            public byte echoVal;
            public Dictionary<int, ushort> sampleTable = new Dictionary<int, ushort>();
            public ushort newId;
        }
    }
}
