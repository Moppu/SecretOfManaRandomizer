using SoMRandomizer.config.settings;
using SoMRandomizer.logging;
using SoMRandomizer.processing.common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoMRandomizer.processing.openworld.randomization
{
    /// <summary>
    /// Randomly selects weapons for each character that exists in open world.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class StartingWeaponRandomizer
    {
        public const string BOY_START_WEAPON_INDEX = "boyStartWeapon";
        public const string GIRL_START_WEAPON_INDEX = "girlStartWeapon";
        public const string SPRITE_START_WEAPON_INDEX = "spriteStartWeapon";

        public static void setStartingWeapons(RandoSettings settings, RandoContext context)
        {
            bool boyExists = context.workingData.getBool(OpenWorldCharacterSelection.BOY_EXISTS);
            bool girlExists = context.workingData.getBool(OpenWorldCharacterSelection.GIRL_EXISTS);
            bool spriteExists = context.workingData.getBool(OpenWorldCharacterSelection.SPRITE_EXISTS);
            string startingChar = context.workingData.get(OpenWorldCharacterSelection.STARTING_CHARACTER);
            int forceStartWeapon = settings.getInt(OpenWorldSettings.PROPERTYNAME_FORCE_START_WEAPON);
            Dictionary<string, List<string>> plandoSettings = context.plandoSettings;
            Random r = context.randomFunctional;

            int boyStarterWeapon = -1;
            int girlStarterWeapon = -1;
            int spriteStarterWeapon = -1;

            List<int> weaponIndexes = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 }.ToList();
            List<int> plandoWeaponIndexes = new List<int>();
            for (int i = 0; i < 8; i++)
            {
                string weaponName = SomVanillaValues.weaponByteToName(i);
                foreach (string plandoKey in plandoSettings.Keys)
                {
                    if (plandoSettings[plandoKey].Contains(weaponName))
                    {
                        plandoWeaponIndexes.Add(i);
                        break;
                    }
                }
            }

            if (boyExists)
            {
                if (startingChar == "boy" && forceStartWeapon != -1)
                {
                    boyStarterWeapon = forceStartWeapon;
                    if (plandoSettings.ContainsKey(PlandoProperties.KEY_BOY_WEAPON))
                    {
                        boyStarterWeapon = SomVanillaValues.weaponByteFromName(plandoSettings[PlandoProperties.KEY_BOY_WEAPON][0]);
                        Logging.log("Processing plando for boy weapon: " + boyStarterWeapon, "debug");
                    }
                }
                else
                {
                    if (plandoSettings.ContainsKey(PlandoProperties.KEY_BOY_WEAPON))
                    {
                        boyStarterWeapon = SomVanillaValues.weaponByteFromName(plandoSettings[PlandoProperties.KEY_BOY_WEAPON][0]);
                        Logging.log("Processing plando for boy weapon: " + boyStarterWeapon, "debug");
                    }
                    else
                    {
                        boyStarterWeapon = weaponIndexes[r.Next() % weaponIndexes.Count];
                        while ((forceStartWeapon != -1 && boyStarterWeapon == forceStartWeapon) || plandoWeaponIndexes.Contains(boyStarterWeapon))
                        {
                            // reroll
                            boyStarterWeapon = weaponIndexes[r.Next() % weaponIndexes.Count];
                        }
                    }
                }
                weaponIndexes.Remove(boyStarterWeapon);
                Logging.log("boy starts with " + SomVanillaValues.weaponByteToName(boyStarterWeapon), "spoiler");
            }

            if (girlExists)
            {
                if (startingChar == "girl" && forceStartWeapon != -1)
                {
                    girlStarterWeapon = forceStartWeapon;
                    if (plandoSettings.ContainsKey(PlandoProperties.KEY_GIRL_WEAPON))
                    {
                        girlStarterWeapon = SomVanillaValues.weaponByteFromName(plandoSettings[PlandoProperties.KEY_GIRL_WEAPON][0]);
                        Logging.log("Processing plando for girl weapon: " + girlStarterWeapon, "debug");
                    }
                }
                else
                {
                    if (plandoSettings.ContainsKey(PlandoProperties.KEY_GIRL_WEAPON))
                    {
                        girlStarterWeapon = SomVanillaValues.weaponByteFromName(plandoSettings[PlandoProperties.KEY_GIRL_WEAPON][0]);
                        Logging.log("Processing plando for girl weapon: " + girlStarterWeapon, "debug");
                    }
                    else
                    {
                        girlStarterWeapon = weaponIndexes[r.Next() % weaponIndexes.Count];
                        while ((forceStartWeapon != -1 && girlStarterWeapon == forceStartWeapon) || plandoWeaponIndexes.Contains(girlStarterWeapon))
                        {
                            // reroll
                            girlStarterWeapon = weaponIndexes[r.Next() % weaponIndexes.Count];
                        }
                    }
                }
                weaponIndexes.Remove(girlStarterWeapon);
                Logging.log("girl starts with " + SomVanillaValues.weaponByteToName(girlStarterWeapon), "spoiler");
            }

            if (spriteExists)
            {
                if (startingChar == "sprite" && forceStartWeapon != -1)
                {
                    spriteStarterWeapon = forceStartWeapon;
                    if (plandoSettings.ContainsKey(PlandoProperties.KEY_SPRITE_WEAPON))
                    {
                        spriteStarterWeapon = SomVanillaValues.weaponByteFromName(plandoSettings[PlandoProperties.KEY_SPRITE_WEAPON][0]);
                        Logging.log("Processing plando for sprite weapon: " + spriteStarterWeapon, "debug");
                    }
                }
                else
                {
                    if (plandoSettings.ContainsKey(PlandoProperties.KEY_SPRITE_WEAPON))
                    {
                        spriteStarterWeapon = SomVanillaValues.weaponByteFromName(plandoSettings[PlandoProperties.KEY_SPRITE_WEAPON][0]);
                        Logging.log("Processing plando for sprite weapon: " + spriteStarterWeapon, "debug");
                    }
                    else
                    {
                        spriteStarterWeapon = weaponIndexes[r.Next() % weaponIndexes.Count];
                        // shouldn't need to reroll here .. or maybe we do for plando
                        while (plandoWeaponIndexes.Contains(spriteStarterWeapon))
                        {
                            spriteStarterWeapon = weaponIndexes[r.Next() % weaponIndexes.Count];
                        }
                    }
                }
                weaponIndexes.Remove(spriteStarterWeapon);
                Logging.log("sprite starts with " + SomVanillaValues.weaponByteToName(spriteStarterWeapon), "spoiler");
            }

            context.workingData.setInt(BOY_START_WEAPON_INDEX, boyStarterWeapon);
            context.workingData.setInt(GIRL_START_WEAPON_INDEX, girlStarterWeapon);
            context.workingData.setInt(SPRITE_START_WEAPON_INDEX, spriteStarterWeapon);
        }
    }
}
