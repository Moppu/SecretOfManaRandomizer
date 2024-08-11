namespace SoMRandomizer.config.settings
{
    /// <summary>
    /// Enumeration of settings and defaults for ancient cave mode.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class AncientCaveSettings : RandoSettings
    {
        public const string MODE_KEY = "ancientcave";
        public const string MODE_NAME = "Ancient Cave";

        public const string PROPERTYNAME_INCLUDE_BOY_CHARACTER = "acBoy";
        public const string PROPERTYNAME_INCLUDE_GIRL_CHARACTER = "acGirl";
        public const string PROPERTYNAME_INCLUDE_SPRITE_CHARACTER = "acSprite";

        public const string PROPERTYNAME_RANDOM_MUSIC = "acMusic";
        public const string PROPERTYNAME_DIALOGUE_SOURCE = "acDialogue";
        public const string PROPERTYNAME_BOSS_FREQUENCY = "BossFrequency";
        public const string PROPERTYNAME_PROFANITY_FILTER = "acFilter";

        public const string PROPERTYNAME_LENGTH = "acLength";
        public const string PROPERTYNAME_DIFFICULTY = "acDifficulty";

        public const string PROPERTYNAME_BIOME_TYPES = "acBiomeTypes";

        public const string PROPERTYVALUE_FORESTBIOME = "forest";
        public const string PROPERTYVALUE_ISLANDBIOME = "island";
        public const string PROPERTYVALUE_RUINSBIOME = "ruins";
        public const string PROPERTYVALUE_CAVEBIOME = "cave";
        public const string PROPERTYVALUE_MANAFORTINTBIOME = "manafortinterior";
        public AncientCaveSettings(CommonSettings commonSettings) : base(commonSettings)
        {
            setInitial(PROPERTYNAME_INCLUDE_BOY_CHARACTER, true); // include boy
            setInitial(PROPERTYNAME_INCLUDE_GIRL_CHARACTER, true); // include girl
            setInitial(PROPERTYNAME_INCLUDE_SPRITE_CHARACTER, true); // include sprite
            setInitial(PROPERTYNAME_PROFANITY_FILTER, false);
            setInitial(PROPERTYNAME_RANDOM_MUSIC, true);

            setInitial(PROPERTYNAME_LENGTH, new string[] { "short", "medium", "long" }, new string[] { "Short (8 Floors)", "Medium (16 Floors)", "Long (24 Floors)" }, "medium");
            setInitial(PROPERTYNAME_BOSS_FREQUENCY, new string[] { "every", "everyfew", "final" }, new string[] { "Every floor", "Every few floors", "Final only" }, "everyfew");
            setInitial(PROPERTYNAME_DIALOGUE_SOURCE, new string[] { "mitch", "demetri" }, new string[] { "Mitch Hedberg", "Demetri Martin" }, "mitch");
            setInitial(PROPERTYNAME_DIFFICULTY, new string[] { "casual", "hard", "reallyhard", "custom" }, new string[] { "Casual", "Hard", "Really hard", "Custom" }, "casual");

            setInitial(PROPERTYNAME_BIOME_TYPES, 
                new string[] {
                    PROPERTYVALUE_FORESTBIOME,
                    PROPERTYVALUE_ISLANDBIOME,
                    PROPERTYVALUE_RUINSBIOME,
                    PROPERTYVALUE_CAVEBIOME,
                    PROPERTYVALUE_MANAFORTINTBIOME
                }, 
                new string[] { "Forest", "Island", "Ruins", "Cave", "Manafort Interior" }, 
                new string[] {
                    PROPERTYVALUE_FORESTBIOME,
                    PROPERTYVALUE_ISLANDBIOME,
                    PROPERTYVALUE_RUINSBIOME,
                    PROPERTYVALUE_CAVEBIOME,
                    PROPERTYVALUE_MANAFORTINTBIOME
                });
        }
    }
}
