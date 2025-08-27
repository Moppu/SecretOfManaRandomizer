
namespace SoMRandomizer.processing.ancientcave.mapgen.forest
{
    /// <summary>
    /// Constants for tileset 2, for "forest" floors.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class ForestConstants
    {
        // -------------
        // Layer 1 tiles
        // -------------

        // tall grass
        public static byte L1_TALL_GRASS = 126;

        public static byte L1_TALL_GRASS_U = 110;
        public static byte L1_TALL_GRASS_L = 125;
        public static byte L1_TALL_GRASS_R = 127;
        public static byte L1_TALL_GRASS_B = 142;

        public static byte L1_TALL_GRASS_BL = 141;
        public static byte L1_TALL_GRASS_BR = 143;
        public static byte L1_TALL_GRASS_UL = 109;
        public static byte L1_TALL_GRASS_UR = 111;

        // empty, solid empty, door empty
        public static byte L1_EMPTY = 140;
        public static byte L1_EMPTY_BLOCK = 186;
        public static byte L1_EMPTY_DOOR = 185;

        public static byte L1_CAVE_TOP_LEFT = 145;
        public static byte L1_CAVE_TOP_RIGHT = 146;
        public static byte L1_CAVE_DOOR_LEFT = 161;
        public static byte L1_CAVE_DOOR_RIGHT = 162;

        // bunches of trees
        public static byte L1_TREE_UPPERLEFT = 116;
        public static byte L1_TREE_UPPERRIGHT = 117;
        public static byte L1_TREE_MIDLEFT = 132;
        public static byte L1_TREE_MIDRIGHT = 133;

        // edges of trees
        public static byte L1_TREE_UPPERLEFT_EDGE = 112;
        public static byte L1_TREE_UPPERRIGHT_EDGE = 113;
        public static byte L1_TREE_MIDLEFT_EDGE = 128;
        public static byte L1_TREE_MIDRIGHT_EDGE = 129;
        public static byte L1_TREE_LOWLEFT_EDGE = 114; // matches L1_TREE_UPPERRIGHT
        public static byte L1_TREE_LOWRIGHT_EDGE = 115; // matches L1_TREE_UPPERLEFT

        // slashable bush
        public static byte L1_BUSH = 5;

        public static byte L1_STEPS = 103;

        public static byte L1_LEDGE_LEFT_TOPMOST = 18;
        public static byte L1_LEDGE_LEFT_MIDTOP = 34;
        public static byte L1_LEDGE_LEFT_MIDBOTTOM = 50;
        public static byte L1_LEDGE_LEFT_BOTTOMMOST = 75;

        public static byte L1_LEDGE_RIGHT_TOPMOST = 19;
        public static byte L1_LEDGE_RIGHT_MIDTOP = 35;
        public static byte L1_LEDGE_RIGHT_MIDBOTTOM = 51;
        public static byte L1_LEDGE_RIGHT_BOTTOMMOST = 76;

        // -------------
        // Layer 2 tiles
        // -------------

        // green with no grass
        public static byte L2_GREEN = 187;

        // short grass
        public static byte L2_SHORT_GRASS = 97;
        public static byte L2_SHORT_GRASS_BR = 64;
        public static byte L2_SHORT_GRASS_BL = 65;
        public static byte L2_SHORT_GRASS_UR = 66;
        public static byte L2_SHORT_GRASS_UL = 67;

        // dirt below tall grass
        public static byte L2_TALL_GRASS_DIRT = 180;

        public static byte L2_TALL_GRASS_DIRT_U = 177;
        public static byte L2_TALL_GRASS_DIRT_L = 179;
        public static byte L2_TALL_GRASS_DIRT_R = 181;
        public static byte L2_TALL_GRASS_DIRT_B = 183;

        public static byte L2_TALL_GRASS_DIRT_UL = 148;
        public static byte L2_TALL_GRASS_DIRT_UR = 149;
        public static byte L2_TALL_GRASS_DIRT_BL = 164;
        public static byte L2_TALL_GRASS_DIRT_BR = 165;

        // walk-over flowers
        public static byte L2_FLOWER_A = 80;
        public static byte L2_FLOWER_B = 81;

        // stump below trees
        public static byte L2_STUMP_LEFT = 70;
        public static byte L2_STUMP_RIGHT = 71;

        // waterfall i use as a warper
        public static byte L2_WATERFALL = 124;

        public static bool isTallGrass(byte layer1value)
        {
            return (layer1value >= ForestConstants.L1_TALL_GRASS_UL && layer1value <= ForestConstants.L1_TALL_GRASS_UR)
                || (layer1value >= ForestConstants.L1_TALL_GRASS_L && layer1value <= ForestConstants.L1_TALL_GRASS_R)
                || (layer1value >= ForestConstants.L1_TALL_GRASS_BL && layer1value <= ForestConstants.L1_TALL_GRASS_BR);
        }

        public static bool isGrass(byte layer2value)
        {
            return layer2value == 97 || layer2value == 71 || layer2value == 70;
        }


        public static bool isGrassOrTallGrass(byte layer1value, byte layer2value)
        {
            return isGrass(layer2value) || isTallGrass(layer1value);
        }

        public static bool isntGrass(byte layer1value)
        {
            return layer1value == 187;
        }

        public static bool isMountainTile(byte layer1Value)
        {
            return layer1Value == 18 ||
                layer1Value == 34 ||
                layer1Value == 50 ||
                layer1Value == 75 ||
                layer1Value == 19 ||
                layer1Value == 35 ||
                layer1Value == 51 ||
                layer1Value == 76 ||
                layer1Value == 1 ||
                layer1Value == 49 ||
                layer1Value == 33 ||
                layer1Value == 65 ||
                layer1Value == 4 ||
                layer1Value == 52 ||
                layer1Value == 36 ||
                layer1Value == 68 ||
                layer1Value == 103 ||
                layer1Value == 145 ||
                layer1Value == 146 ||
                layer1Value == 161 ||
                layer1Value == 162;
        }
    }
}
