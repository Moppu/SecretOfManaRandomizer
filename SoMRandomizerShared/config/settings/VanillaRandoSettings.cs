namespace SoMRandomizer.config.settings
{
    /// <summary>
    /// Enumeration of settings and defaults for vanilla rando mode.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class VanillaRandoSettings : RandoSettings
    {
        public const string MODE_KEY = "rando";
        public const string MODE_NAME = "Rando";

        public const string PROPERTYNAME_RANDOMIZE_ENEMIES = "rEnemies";
        public const string PROPERTYNAME_RANDOMIZE_BOSSES = "rBosses";
        public const string PROPERTYNAME_RANDOMIZE_WEAPON_ORBS = "rOrbs";
        public const string PROPERTYNAME_RANDOMIZE_ELEMENTS = "rElements";
        public const string PROPERTYNAME_RANDOMIZE_WEAPONS = "rWeapons";
        public const string PROPERTYNAME_RANDOMIZE_MUSIC = "rMusic";
        public const string PROPERTYNAME_AUTOSAVE = "rAutosave";

        public const string PROPERTYNAME_DIALOGUE_CUTS = "rDialogue";
        public const string PROPERTYNAME_PRESERVE_EARLY_BOSSES = "rPreserve";
        public const string PROPERTYNAME_EXP_MULTIPLIER = "rExp";
        public const string PROPERTYNAME_GOLD_MULTIPLIER = "rGold";
        public const string PROPERTYNAME_SPECIAL_MODE = "rHoliday";

        public const string PROPERTYNAME_ENEMY_SCALING = "rDifficulty";
        public const string PROPERTYNAME_STATUS_AILMENTS = "rStatusAilments";

        public VanillaRandoSettings(CommonSettings commonSettings) : base(commonSettings)
        {
            // default vanilla rando settings

            // boolean settings
            setInitial(PROPERTYNAME_RANDOMIZE_ENEMIES, true); // random enemy swap
            setInitial(PROPERTYNAME_RANDOMIZE_BOSSES, true); // random boss swap
            setInitial(PROPERTYNAME_RANDOMIZE_WEAPON_ORBS, true); // random weapon orb swap
            setInitial(PROPERTYNAME_RANDOMIZE_ELEMENTS, true); // random spell/element swap
            setInitial(PROPERTYNAME_RANDOMIZE_WEAPONS, true); // weapon rando vs vanilla weapons
            setInitial(PROPERTYNAME_RANDOMIZE_MUSIC, true); // random music
            setInitial(PROPERTYNAME_AUTOSAVE, true); // autosave in 4th slot
            setInitial(PROPERTYNAME_DIALOGUE_CUTS, true); // shorten vanilla dialogue/events for faster run
            setInitial(PROPERTYNAME_PRESERVE_EARLY_BOSSES, false); // keep the first few bosses the same

            // enumerations
            setInitial(PROPERTYNAME_EXP_MULTIPLIER, new string[] { "half", "normal", "double", "triple" }, new string[] { "Half", "Normal", "Double", "Triple" }, "double");
            setInitial(PROPERTYNAME_GOLD_MULTIPLIER, new string[] { "half", "normal", "double", "triple" }, new string[] { "Half", "Normal", "Double", "Triple" }, "double");
            setInitial(PROPERTYNAME_SPECIAL_MODE, new string[] { "none", "halloween", "xmas" }, new string[] { "None", "Halloween", "Christmas" }, "none");
            setInitial(PROPERTYNAME_ENEMY_SCALING, new string[] { "20", "40", "60", "80", "100", "125", "150", "175", "200" }, new string[] { "20", "40", "60", "80", "100", "125", "150", "175", "200" }, "100");
            setInitial(PROPERTYNAME_STATUS_AILMENTS, new string[] { "location", "type", "easy", "annoying", "awful" }, new string[] { "Location", "Enemy type", "Random (easy)", "Random (annoying)", "Random (awful)" }, "location");
        }
    }
}
