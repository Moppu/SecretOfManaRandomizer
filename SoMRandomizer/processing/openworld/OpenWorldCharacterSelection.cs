using SoMRandomizer.config.settings;
using SoMRandomizer.logging;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.hacks.openworld;
using System;
using System.Collections.Generic;

namespace SoMRandomizer.processing.openworld
{
    /// <summary>
    /// Processes starting/included characters and randomizations for open world, and sets some convenience properties in the settings that other hacks use.
    /// </summary>
    public class OpenWorldCharacterSelection : RandoProcessor
    {
        // intermediary settings passed between hacks/other code that aren't part of the initial inputs
        public static string STARTING_CHARACTER = "startingCharacter";
        public static string BOY_IN_LOGIC = "findBoy";
        public static string GIRL_IN_LOGIC = "findGirl";
        public static string SPRITE_IN_LOGIC = "findSprite";
        public static string START_WITH_BOY = "startWithBoy";
        public static string START_WITH_GIRL = "startWithGirl";
        public static string START_WITH_SPRITE = "startWithSprite";
        public static string BOY_EXISTS = "boyExists";
        public static string GIRL_EXISTS = "girlExists";
        public static string SPRITE_EXISTS = "spriteExists";
        public static string START_SOLO = "startSolo";
        public static string FOUND_CHARS_GET_YOUR_LEVEL = "foundCharsGetYourLevel";

        protected override string getName()
        {
            return "Open world character selection";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            Random r = context.randomFunctional;
            StringValueSettings working = context.workingData;
            string startingChar = settings.get(OpenWorldSettings.PROPERTYNAME_STARTING_CHAR);
            //working.set(STARTING_CHARACTER, settings.get(OpenWorldSettings.PROPERTYNAME_STARTING_CHAR));
            if (startingChar == "random")
            {
                int startingCharVal = r.Next() % 3;
                switch (startingCharVal)
                {
                    case 0:
                        startingChar = "boy";
                        break;
                    case 1:
                        startingChar = "girl";
                        break;
                    case 2:
                        startingChar = "sprite";
                        break;
                }
            }

            bool boyInLogic = false;
            bool girlInLogic = true;
            bool spriteInLogic = true;
            bool startSolo = true;
            bool startWithBoy = startingChar == "boy";
            bool startWithGirl = startingChar == "girl";
            bool startWithSprite = startingChar == "sprite";

            // process character settings
            string otherPlayersProp = settings.get(OpenWorldSettings.PROPERTYNAME_START_WITH_GIRL_AND_SPRITE);
            bool foundCharactersGetYourLevel = otherPlayersProp.Contains("CL");
            if (otherPlayersProp.Contains("none"))
            {
                boyInLogic = false;
                girlInLogic = false;
                spriteInLogic = false;
                startSolo = true;
            }
            else if (otherPlayersProp.Contains("startboth"))
            {
                boyInLogic = false;
                girlInLogic = false;
                spriteInLogic = false;
                startWithBoy = true;
                startWithGirl = true;
                startWithSprite = true;
                startSolo = false;
            }
            else if (otherPlayersProp.Contains("start1"))
            {
                boyInLogic = false;
                girlInLogic = false;
                spriteInLogic = false;
                startSolo = false;
                bool which = (r.Next() & 2) == 0;
                bool findOther = (otherPlayersProp.Contains("find1"));

                if (startWithBoy)
                {
                    startWithGirl = which;
                    startWithSprite = !which;
                    if (findOther)
                    {
                        girlInLogic = !which;
                        spriteInLogic = which;
                    }
                }
                else if (startWithGirl)
                {
                    startWithBoy = which;
                    startWithSprite = !which;
                    if (findOther)
                    {
                        boyInLogic = !which;
                        spriteInLogic = which;
                    }
                }
                else if (startWithSprite)
                {
                    startWithBoy = which;
                    startWithGirl = !which;
                    if (findOther)
                    {
                        boyInLogic = !which;
                        girlInLogic = which;
                    }
                }
            }
            else if (otherPlayersProp.Contains("find1"))
            {
                boyInLogic = false;
                girlInLogic = false;
                spriteInLogic = false;
                startSolo = true;
                bool which = (r.Next() & 2) == 0;

                if (startWithBoy)
                {
                    girlInLogic = which;
                    spriteInLogic = !which;
                }
                else if (startWithGirl)
                {
                    boyInLogic = which;
                    spriteInLogic = !which;
                }
                else if (startWithSprite)
                {
                    boyInLogic = which;
                    girlInLogic = !which;
                }
            }
            else if (otherPlayersProp.Contains("findboth"))
            {
                boyInLogic = !startWithBoy;
                girlInLogic = !startWithGirl;
                spriteInLogic = !startWithSprite;
                startSolo = true;
            }
            //}

            bool boyExists = startWithBoy || boyInLogic;
            bool girlExists = startWithGirl || girlInLogic;
            bool spriteExists = startWithSprite || spriteInLogic;

            // tweak character settings based on plando
            if (context.plandoSettings.ContainsKey("(NON-EXISTENT)"))
            {
                List<string> nonExistent = context.plandoSettings["(NON-EXISTENT)"];
                if (nonExistent.Contains("Boy"))
                {
                    boyExists = false;
                    startWithBoy = false;
                }
                if (nonExistent.Contains("Girl"))
                {
                    girlExists = false;
                    startWithGirl = false;
                }
                if (nonExistent.Contains("Sprite"))
                {
                    spriteExists = false;
                    startWithSprite = false;
                }
                if (nonExistent.Contains("Boy") && startingChar == "boy")
                {
                    // change starting char
                    if (girlExists)
                    {
                        startingChar = "girl";
                        girlInLogic = false;
                        Logging.log("Had to change starting character from boy to girl due to plando");
                    }
                    else if (spriteExists)
                    {
                        startingChar = "sprite";
                        spriteInLogic = false;
                        Logging.log("Had to change starting character from boy to sprite due to plando");
                    }
                    else
                    {
                        throw new Exception("Plando - no characters exist! Couldn't reassign boy as starting character.");
                    }
                }
                if (nonExistent.Contains("Girl") && startingChar == "girl")
                {
                    // change starting char
                    if (boyExists)
                    {
                        startingChar = "boy";
                        boyInLogic = false;
                        Logging.log("Had to change starting character from girl to boy due to plando");
                    }
                    else if (spriteExists)
                    {
                        startingChar = "sprite";
                        spriteInLogic = false;
                        Logging.log("Had to change starting character from girl to sprite due to plando");
                    }
                    else
                    {
                        throw new Exception("Plando - no characters exist! Couldn't reassign girl as starting character.");
                    }
                }
                if (nonExistent.Contains("Sprite") && startingChar == "sprite")
                {
                    // change starting char
                    if (boyExists)
                    {
                        startingChar = "boy";
                        boyInLogic = false;
                        Logging.log("Had to change starting character from sprite to boy due to plando");
                    }
                    else if (girlExists)
                    {
                        startingChar = "girl";
                        girlInLogic = false;
                        Logging.log("Had to change starting character from sprite to girl due to plando");
                    }
                    else
                    {
                        throw new Exception("Plando - no characters exist! Couldn't reassign sprite as starting character.");
                    }
                }
            }
            if (!boyExists && !girlExists && !spriteExists)
            {
                throw new Exception("Plando - no characters exist!");
            }

            Logging.log("Starting char = " + startingChar, "spoiler");
            Logging.log("Start with boy? " + startWithBoy + " // find boy? " + boyInLogic + " // boy exists? " + boyExists, "spoiler");
            Logging.log("Start with girl? " + startWithGirl + " // find girl? " + girlInLogic + " // girl exists? " + girlExists, "spoiler");
            Logging.log("Start with sprite? " + startWithSprite + " // find sprite? " + spriteInLogic + " // sprite exists? " + spriteExists, "spoiler");

            // output to working data for use by other hacks (including the one below)
            working.set(STARTING_CHARACTER, startingChar);
            working.setBool(START_WITH_BOY, startWithBoy);
            working.setBool(START_WITH_GIRL, startWithGirl);
            working.setBool(START_WITH_SPRITE, startWithSprite);
            working.setBool(BOY_IN_LOGIC, boyInLogic);
            working.setBool(GIRL_IN_LOGIC, girlInLogic);
            working.setBool(SPRITE_IN_LOGIC, spriteInLogic);
            working.setBool(BOY_EXISTS, boyExists);
            working.setBool(GIRL_EXISTS, girlExists);
            working.setBool(SPRITE_EXISTS, spriteExists);
            working.setBool(START_SOLO, startSolo);
            working.setBool(FOUND_CHARS_GET_YOUR_LEVEL, foundCharactersGetYourLevel);

            if (startingChar != "boy")
            {
                new StartingCharacterRandomizer().add(origRom, outRom, seed, settings, context);
            }

            return true;
        }
    }
}
