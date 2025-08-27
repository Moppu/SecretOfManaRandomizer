using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.hacks.common.util;
using System;

namespace SoMRandomizer.processing.openworld
{
    /// <summary>
    /// Randomize some enemy names for open world.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class OpenWorldSillyEnemyNamePicker : RandoProcessor
    {
        protected override string getName()
        {
            return "Pick silly names for enemies";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            Random r_cosmetic = context.randomCosmetic;
            NamesOfThings namesOfThings = context.namesOfThings;
            // random replacement names for things
            int rabiteName = (r_cosmetic.Next() % 3);
            if (rabiteName == 0)
            {
                // rabite
                namesOfThings.setName(NamesOfThings.INDEX_ENEMIES_START + 0, "Pogopuschel");
                namesOfThings.setName(NamesOfThings.INDEX_ENEMIES_START + 20, "Turbopuschel");
            }
            else if (rabiteName == 1)
            {
                // sd3 name
                namesOfThings.setName(NamesOfThings.INDEX_ENEMIES_START + 20, "Rabilion");
            }
            if ((r_cosmetic.Next() % 2) == 0)
            {
                // dumb
                namesOfThings.setName(NamesOfThings.INDEX_ENEMIES_START + 87, "Antis Mant");
            }
            if ((r_cosmetic.Next() % 2) == 0)
            {
                // dumb
                namesOfThings.setName(NamesOfThings.INDEX_ENEMIES_START + 57, "Metroid");
            }
            if ((r_cosmetic.Next() % 2) == 0)
            {
                // dumb
                namesOfThings.setName(NamesOfThings.INDEX_ENEMIES_START + 105, "Snek");
                namesOfThings.setName(NamesOfThings.INDEX_ENEMIES_START + 123, "Snek");
            }
            else if ((r_cosmetic.Next() % 2) == 0)
            {
                // dumb
                namesOfThings.setName(NamesOfThings.INDEX_ENEMIES_START + 105, "Danger Noodle");
                namesOfThings.setName(NamesOfThings.INDEX_ENEMIES_START + 123, "Danger Noodle");
            }
            else if ((r_cosmetic.Next() % 2) == 0)
            {
                // dumb
                namesOfThings.setName(NamesOfThings.INDEX_ENEMIES_START + 105, "Nope Rope");
                namesOfThings.setName(NamesOfThings.INDEX_ENEMIES_START + 123, "Nope Rope");
            }
            if ((r_cosmetic.Next() % 8) == 0)
            {
                // dumb
                namesOfThings.setName(NamesOfThings.INDEX_ENEMIES_START + 102, "Gordon Bull");
            }
            if ((r_cosmetic.Next() % 2) == 0)
            {
                // dumb
                namesOfThings.setName(NamesOfThings.INDEX_ENEMIES_START + 110, "Melon");
            }
            else if ((r_cosmetic.Next() % 2) == 0)
            {
                // dumb
                namesOfThings.setName(NamesOfThings.INDEX_ENEMIES_START + 110, "Aegrfdiujeplf");
            }
            if ((r_cosmetic.Next() % 2) == 0)
            {
                // sd3
                namesOfThings.setName(NamesOfThings.INDEX_ENEMIES_START + 2, "Myconid");
            }
            if ((r_cosmetic.Next() % 2) == 0)
            {
                namesOfThings.setName(NamesOfThings.INDEX_ENEMIES_START + 3, "Ewok");
            }
            if ((r_cosmetic.Next() % 2) == 0)
            {
                namesOfThings.setName(NamesOfThings.INDEX_ENEMIES_START + 28, "Ewok");
            }
            if ((r_cosmetic.Next() % 2) == 0)
            {
                namesOfThings.setName(NamesOfThings.INDEX_ENEMIES_START + 13, "Sit on me");
            }
            if ((r_cosmetic.Next() % 2) == 0)
            {
                namesOfThings.setName(NamesOfThings.INDEX_ENEMIES_START + 15, "Puking Cloud");
            }
            if ((r_cosmetic.Next() % 2) == 0)
            {
                namesOfThings.setName(NamesOfThings.INDEX_ENEMIES_START + 29, "Puking Cloud");
            }
            int mimicName = (r_cosmetic.Next() % 4);
            if (mimicName == 1)
            {
                namesOfThings.setName(NamesOfThings.INDEX_ENEMIES_START + 45, "Free Items");
            }
            else if (mimicName == 2)
            {
                namesOfThings.setName(NamesOfThings.INDEX_ENEMIES_START + 45, "Legit Chest");
            }
            else if (mimicName == 3)
            {
                namesOfThings.setName(NamesOfThings.INDEX_ENEMIES_START + 45, "Open me");
            }
            if ((r_cosmetic.Next() % 2) == 0)
            {
                namesOfThings.setName(NamesOfThings.INDEX_ENEMIES_START + 21, "Flappy Death");
            }
            if ((r_cosmetic.Next() % 2) == 0)
            {
                // sd3
                namesOfThings.setName(NamesOfThings.INDEX_ENEMIES_START + 22, "Molebear");
            }
            else if ((r_cosmetic.Next() % 2) == 0)
            {
                namesOfThings.setName(NamesOfThings.INDEX_ENEMIES_START + 22, "Sonic");
            }
            if ((r_cosmetic.Next() % 2) == 0)
            {
                // sd3
                namesOfThings.setName(NamesOfThings.INDEX_ENEMIES_START + 122, "Pakkun Lizard");
                namesOfThings.setName(NamesOfThings.INDEX_ENEMIES_START + 95, "Pakkun Dragon");
            }
            if ((r_cosmetic.Next() % 2) == 0)
            {
                // cuddle cats
                namesOfThings.setName(NamesOfThings.INDEX_ENEMIES_START + 91, "Schmusekater");
                namesOfThings.setName(NamesOfThings.INDEX_ENEMIES_START + 107, "Kuscheltiger");
            }
            if ((r_cosmetic.Next() % 2) == 0)
            {
                // mech rider
                namesOfThings.setName(NamesOfThings.INDEX_ENEMIES_START + 96, "Alpha Cybermax");
                namesOfThings.setName(NamesOfThings.INDEX_ENEMIES_START + 100, "Beta Cybermax");
                namesOfThings.setName(NamesOfThings.INDEX_ENEMIES_START + 114, "Gamma Cybermax");
            }
            int hydraName = (r_cosmetic.Next() % 3);
            if (hydraName == 0)
            {
                // jabber
                namesOfThings.setName(NamesOfThings.INDEX_ENEMIES_START + 92, "Gleeok");
            }
            else if (hydraName == 1)
            {
                // hydra
                namesOfThings.setName(NamesOfThings.INDEX_ENEMIES_START + 109, "Gleeok");
            }

            if ((r_cosmetic.Next() % 2) == 0)
            {
                namesOfThings.setName(NamesOfThings.INDEX_ENEMIES_START + 127, "Flammie");
            }
            else if ((r_cosmetic.Next() % 2) == 0)
            {
                namesOfThings.setName(NamesOfThings.INDEX_ENEMIES_START + 127, "Falkor");
            }

            return true;
        }
    }
}
