namespace SoMRandomizer.config.settings
{
    /// <summary>
    /// Enumeration of settings and defaults for boss rush mode.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class BossRushSettings : RandoSettings
    {
        public const string MODE_KEY = "bossrush";
        public const string MODE_NAME = "Boss Rush";

        public const string PROPERTYNAME_INCLUDE_BOY_CHARACTER = "brBoy";
        public const string PROPERTYNAME_INCLUDE_GIRL_CHARACTER = "brGirl";
        public const string PROPERTYNAME_INCLUDE_SPRITE_CHARACTER = "brSprite";
        public const string PROPERTYNAME_LIMIT_MP_STEAL = "brLimitMpAbsorb";

        public const string PROPERTYNAME_DIFFICULTY = "brDifficulty";

        public BossRushSettings(CommonSettings commonSettings) : base(commonSettings)
        {
            // default boss rush settings

            // boolean settings
            setInitial(PROPERTYNAME_INCLUDE_BOY_CHARACTER, true); // include boy
            setInitial(PROPERTYNAME_INCLUDE_GIRL_CHARACTER, true); // include girl
            setInitial(PROPERTYNAME_INCLUDE_SPRITE_CHARACTER, true); // include sprite
            setInitial(PROPERTYNAME_LIMIT_MP_STEAL, true); // limit mp stolen by luna spell

            // enumerations
            setInitial(PROPERTYNAME_DIFFICULTY, new string[] { "casual", "hard", "reallyhard", "custom" }, new string[] { "Casual", "Hard", "Really hard", "Custom" }, "casual");
        }
    }
}
