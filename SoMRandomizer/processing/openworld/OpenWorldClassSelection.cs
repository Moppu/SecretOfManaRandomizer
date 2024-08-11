using SoMRandomizer.config.settings;
using SoMRandomizer.logging;
using SoMRandomizer.processing.common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoMRandomizer.processing.openworld
{
    /// <summary>
    /// Process randomizations for character classes and set a few properties used by other hacks.
    /// Actual implementation of character classes can be found in the CharacterClasses hack.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class OpenWorldClassSelection : RandoProcessor
    {
        public const string BOY_CLASS = "boyClass";
        public const string GIRL_CLASS = "girlClass";
        public const string SPRITE_CLASS = "spriteClass";
        public const string GIRL_MAGIC_EXISTS = "girlMagicExists";
        public const string SPRITE_MAGIC_EXISTS = "spriteMagicExists";
        public const string ANY_MAGIC_EXISTS = "anyMagicExists";

        protected override string getName()
        {
            return "Open world class selection";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            Random r = context.randomFunctional;
            StringValueSettings working = context.workingData;

            // set character classes
            string boyClass = settings.get(OpenWorldSettings.PROPERTYNAME_BOY_CLASS);
            string girlClass = settings.get(OpenWorldSettings.PROPERTYNAME_GIRL_CLASS);
            string spriteClass = settings.get(OpenWorldSettings.PROPERTYNAME_SPRITE_CLASS);

            List<string> randomClass = new string[] { "OGboy", "OGgirl", "OGsprite" }.ToList();
            List<string> randomUniqueClass = new string[] { "OGboy", "OGgirl", "OGsprite" }.ToList();

            if (boyClass == "random")
            {
                boyClass = randomClass[r.Next() % randomClass.Count];
            }
            else
            {
                randomUniqueClass.Remove(boyClass);
            }
            if (girlClass == "random")
            {
                girlClass = randomClass[r.Next() % randomClass.Count];
            }
            else
            {
                randomUniqueClass.Remove(girlClass);
            }
            if (spriteClass == "random")
            {
                spriteClass = randomClass[r.Next() % randomClass.Count];
            }
            else
            {
                randomUniqueClass.Remove(spriteClass);
            }

            if (boyClass == "randomunique")
            {
                if (randomUniqueClass.Count > 0)
                {
                    boyClass = randomUniqueClass[r.Next() % randomUniqueClass.Count];
                    randomUniqueClass.Remove(boyClass);
                }
                else
                {
                    boyClass = randomClass[r.Next() % randomClass.Count];
                }
            }
            if (girlClass == "randomunique")
            {
                if (randomUniqueClass.Count > 0)
                {
                    girlClass = randomUniqueClass[r.Next() % randomUniqueClass.Count];
                    randomUniqueClass.Remove(girlClass);
                }
                else
                {
                    girlClass = randomClass[r.Next() % randomClass.Count];
                }
            }
            if (spriteClass == "randomunique")
            {
                if (randomUniqueClass.Count > 0)
                {
                    spriteClass = randomUniqueClass[r.Next() % randomUniqueClass.Count];
                    randomUniqueClass.Remove(spriteClass);
                }
                else
                {
                    spriteClass = randomClass[r.Next() % randomClass.Count];
                }
            }

            Logging.log("Boy character role: " + boyClass, "spoiler");
            Logging.log("Girl character role: " + girlClass, "spoiler");
            Logging.log("Sprite character role: " + spriteClass, "spoiler");

            bool girlMagicExists = false;
            bool spriteMagicExists = false;
            if ((working.getBool(OpenWorldCharacterSelection.BOY_EXISTS) && boyClass == "OGgirl") || 
                (working.getBool(OpenWorldCharacterSelection.GIRL_EXISTS) && girlClass == "OGgirl") || 
                (working.getBool(OpenWorldCharacterSelection.SPRITE_EXISTS) && spriteClass == "OGgirl"))
            {
                girlMagicExists = true;
            }
            if ((working.getBool(OpenWorldCharacterSelection.BOY_EXISTS) && boyClass == "OGsprite") || 
                (working.getBool(OpenWorldCharacterSelection.GIRL_EXISTS) && girlClass == "OGsprite") || 
                (working.getBool(OpenWorldCharacterSelection.SPRITE_EXISTS) && spriteClass == "OGsprite"))
            {
                spriteMagicExists = true;
            }

            // set data for other hacks - character classes and whether any magic exists based on selected roles & character existence
            working.set(BOY_CLASS, boyClass);
            working.set(GIRL_CLASS, girlClass);
            working.set(SPRITE_CLASS, spriteClass);
            working.setBool(GIRL_MAGIC_EXISTS, girlMagicExists);
            working.setBool(SPRITE_MAGIC_EXISTS, spriteMagicExists);
            working.setBool(ANY_MAGIC_EXISTS, girlMagicExists || spriteMagicExists);
            return true;
        }
    }
}
