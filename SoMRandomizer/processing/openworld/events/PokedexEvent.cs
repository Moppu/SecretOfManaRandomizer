using SoMRandomizer.config.settings;
using SoMRandomizer.logging;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.common.structure;
using System;
using System.IO;
using System.Reflection;
using System.Xml;

namespace SoMRandomizer.processing.openworld.events
{
    /// <summary>
    /// Change the dialogue of an NPC in pandora to be a random pokedex entry.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    class PokedexEvent : RandoProcessor
    {
        protected override string getName()
        {
            return "Random pokedex entry event";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            // pokedex entry in lemonlime haired pandora lady
            try
            {
                Random r = context.randomFunctional;
                string pokemonNumber = "0";
                string pokemonName = "";
                string pokemonDescription = "";
                int generation = 0;
                Assembly assemb = Assembly.GetExecutingAssembly();
                using (Stream stream = assemb.GetManifestResourceStream("SoMRandomizer.Resources.pokedata3.xml"))
                {
                    XmlDataDocument xmldoc = new XmlDataDocument();
                    xmldoc.Load(stream);
                    XmlNodeList pokeDexNodes = xmldoc.GetElementsByTagName("pokedex");
                    for (int i = 0; i <= pokeDexNodes.Count - 1; i++)
                    {
                        XmlNode pokeDexNode = pokeDexNodes[i];
                        XmlNodeList pokemonNodes = pokeDexNode.ChildNodes;
                        XmlNode randomPokemon = pokemonNodes[r.Next() % pokemonNodes.Count];
                        XmlAttributeCollection attribs = randomPokemon.Attributes;
                        foreach (XmlAttribute attrib in attribs)
                        {
                            if (attrib.Name == "id")
                            {
                                pokemonNumber = attrib.InnerText;
                            }
                        }

                        int pokeNum = Int32.Parse(pokemonNumber);
                        if (pokeNum >= 1 && pokeNum <= 151)
                        {
                            generation = 1;
                        }
                        if (pokeNum >= 152 && pokeNum <= 251)
                        {
                            generation = 2;
                        }
                        if (pokeNum >= 252 && pokeNum <= 386)
                        {
                            generation = 3;
                        }
                        if (pokeNum >= 387 && pokeNum <= 493)
                        {
                            generation = 4;
                        }

                        foreach (XmlNode node in randomPokemon.ChildNodes)
                        {
                            if (node.Name == "name")
                            {
                                pokemonName = node.InnerText;
                            }
                            if (node.Name == "description")
                            {
                                pokemonDescription = node.InnerText;
                            }
                        }
                    }

                    if (pokemonNumber != "0" && pokemonName != "" && pokemonDescription != "" && generation != 0)
                    {
                        // lemon-lime colored lady in the upper right of pandora town
                        EventScript newEvent143 = new EventScript();
                        context.replacementEvents[0x143] = newEvent143;

                        string pokemonDialogue = "Pokemon of the Day:\n " + "No. " + pokemonNumber + ": " + pokemonName + "\n Generation " + generation + "\n";
                        string pokemonDescriptionWordwrap = VanillaEventUtil.wordWrapText(pokemonDescription);
                        pokemonDialogue += pokemonDescriptionWordwrap;
                        Logging.log(pokemonDialogue, "spoiler");
                        newEvent143.AddDialogueBox(pokemonDialogue);
                        newEvent143.End();
                    }
                }

            }
            catch (Exception e)
            {
                // well, she'll have vanilla dialogue i guess
            }
            return true;
        }
    }
}
