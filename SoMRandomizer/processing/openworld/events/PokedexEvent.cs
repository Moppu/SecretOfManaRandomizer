using SoMRandomizer.config.settings;
using SoMRandomizer.logging;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.common.structure;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using U8Xml;

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
                    using (var xml = XmlParser.Parse(stream))
                    {
                        var pokeDexNode = xml.Root;
                        var pokemonNodes = pokeDexNode.Children;
                        var randomPokemon = pokemonNodes.ElementAt(r.Next() % pokemonNodes.Count);
                        pokemonNumber = randomPokemon.FindAttribute("id").Value.ToString();

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

                        foreach (var node in randomPokemon.Children)
                        {
                            if (node.Name.ToString() == "name")
                            {
                                pokemonName = node.InnerText.ToString();
                            }
                            if (node.Name.ToString() == "description")
                            {
                                pokemonDescription = node.InnerText.ToString();
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
