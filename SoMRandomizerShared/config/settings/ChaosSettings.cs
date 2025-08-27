namespace SoMRandomizer.config.settings
{
    /// <summary>
    /// Enumeration of settings and defaults for chaos mode.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class ChaosSettings : RandoSettings
    {
        public const string MODE_KEY = "chaos";
        public const string MODE_NAME = "Chaos";

        public const string PROPERTYNAME_PALETTE_SWAP_TYPE = "chMapColors";

        public const string PROPERTYNAME_INCLUDE_BOY_CHARACTER = "chBoy";
        public const string PROPERTYNAME_INCLUDE_GIRL_CHARACTER = "chGirl";
        public const string PROPERTYNAME_INCLUDE_SPRITE_CHARACTER = "chSprite";

        public const string PROPERTYNAME_PRIORITIZE_HEAL_SPELLS = "chHealSpellsFirst"; 
        public const string PROPERTYNAME_SAFER_EARLY_FLOORS = "chEasyEarlyFloors";

        public const string PROPERTYNAME_NUM_FLOORS = "chFloors";
        public const string PROPERTYNAME_DIFFICULTY = "chDifficulty";
        public const string PROPERTYNAME_NUM_BOSSES = "chBosses";

        public ChaosSettings(CommonSettings commonSettings) : base(commonSettings)
        {
            // default chaos mode settings

            // boolean settings
            setInitial(PROPERTYNAME_INCLUDE_BOY_CHARACTER, true); // include boy
            setInitial(PROPERTYNAME_INCLUDE_GIRL_CHARACTER, true); // include girl
            setInitial(PROPERTYNAME_INCLUDE_SPRITE_CHARACTER, true); // include sprite
            setInitial(PROPERTYNAME_PRIORITIZE_HEAL_SPELLS, true); // almost always get undine first
            setInitial(PROPERTYNAME_SAFER_EARLY_FLOORS, true); // bosses not til later

            // enumerations
            setInitial(PROPERTYNAME_PALETTE_SWAP_TYPE, new string[] { "none", "reasonable", "ridiculous" }, new string[] { "None", "Reasonable", "Ridiculous" }, "reasonable");
            setInitial(PROPERTYNAME_NUM_FLOORS, new string[] { "vfew", "few", "avg", "many", "vmany" }, new string[] { "Very few", "Few", "Average", "Many", "Very many" }, "avg");
            setInitial(PROPERTYNAME_DIFFICULTY, new string[] { "casual", "hard", "reallyhard", "custom" }, new string[] { "Casual", "Hard", "Really hard", "Custom" }, "casual");
            setInitial(PROPERTYNAME_NUM_BOSSES, new string[] { "vfew", "few", "avg", "many", "vmany" }, new string[] { "Very few", "Few", "Average", "Many", "Very many" }, "avg");
        }
    }
}
