using SoMRandomizer.config;
using System;
using System.Collections.Generic;

namespace SoMRandomizer.util
{
    /// <summary>
    /// Damage calculation utilities used by the customizable settings UI for the older procgen modes.
    /// May not be particularly accurate.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class DamageCalculations
    {
        public static void calculatePhysicalDamage(
            int sourceStr,
            int sourceAgi,
            int sourceWeaponHitrate,
            int sourceWeaponDamage,
            int weaponCharge,
            int manaPower,
            int destDef,
            int destEvade,
            out int damage,
            out int criticalDamage
            )
        {
            // boy level 1:
            // str = 15
            // agi = 15
            // weapon hit=75%
            // weapon atk=3
            // rabite:
            // def = 0
            // evade = 0
            int hit = sourceAgi + sourceWeaponHitrate; // 90
            int atk = sourceStr + sourceWeaponDamage; // 18
            int force = (int)(atk * (1 + (weaponCharge + manaPower) / 4.0)); // 18

            int power = (hit * force) / 100; // 16

            int defpower = (int)(destEvade * destDef / 80.0); // 0

            int normalPower = power - defpower;
            if (normalPower <= 0)
            {
                normalPower = 1;
            }
            int critPower = power * 2 - defpower;
            if (critPower <= 0)
            {
                critPower = 1;
            }
            damage = normalPower;
            criticalDamage = critPower;
        }

        public static void calculateMagicDamage(
            int sourceInt,
            int spellHitRate,
            int spellLevel,
            int spellDamage,
            int destMdef,
            int destMev,
            out int damage
            )
        {
            int hit = sourceInt / 4 + spellHitRate;
            int force = (int)((sourceInt + spellDamage) * (1 + spellLevel / 2.0));
            int power = (hit * force) / 100;
            int defpower = (destMdef * destMev) / 80;
            int dmg = power - defpower;
            if(dmg <= 0)
            {
                dmg = 1;
            }
            damage = dmg;
        }

        // starting at level 1
        private static int[] expValues = new int[]
        {
            0,       16,      47,      105,     204,     363,     602,     945,     1418,    2049,
            2870,    3914,    5218,    6819,    8759,    11080,   13827,   17049,   20794,   25115,
            30065,   35701,   42081,   49265,   57316,   66298,   76278,   87324,   99507,   112899,
            127575,  143611,  161086,  180080,  200675,  222956,  247009,  272921,  300783,  330686,
            362724,  396992,  433588,  472611,  514162,  558343,  605260,  655018,  707726,  763494,
            822433,  884658,  950283,  1019425, 1092204, 1168739, 1249153, 1333570, 1422116, 1514918, 
            1612106, 1713810, 1820163, 1931299, 2047354, 2168465, 2294772, 2426416, 2563540, 2706287,
            2854804, 3009239, 3169740, 3336459, 3509548, 3689162, 3875456, 4068587, 4268715, 4476000,
            4690605, 4912693, 5142430, 5379983, 5625521, 5879214, 6141234, 6411754, 6690950, 6978998,
            7276076, 7582364, 7898043, 8223296, 8558308, 8903264, 9258352, 9623761, 9999682, 9999999
        };
        public static List<int> generatePhysicalDamageToEnemiesCurve(int numFloors, int playerCharNum, int estEnemiesKilledPerFloor,
            IntGrowthValue enemyDef, IntGrowthValue enemyEvade,
            IntGrowthValue enemyHp,
            IntGrowthValue enemyExp,
            IntGrowthValue manaPower, bool critDamage, bool divideByTotalHp, double bossStatMultiplier, double bossHpMultiplier,
            double floorScalingFactor)
        {
            List<int> points = new List<int>();
            // stats:
            // boy str, agi 15-99
            // girl str 10-80, agi 10-85
            // sprite str 7-75, agi 12-75
            // exp 2=16, 3=47, 4=105...
            int[] strMin = new int[] { 15, 10, 7 };
            int[] strMax = new int[] { 99, 80, 75 };
            int[] agiMin = new int[] { 15, 10, 12 };
            int[] agiMax = new int[] { 99, 85, 75 };

            int playerLev = 1;
            int playerExp = 0;
            for(int i=0; i < numFloors; i++)
            {
                int totalExpGainedThisFloor = (int)(enemyExp.baseValue + Math.Pow(i, enemyExp.exponent) * enemyExp.growthValue * floorScalingFactor) * estEnemiesKilledPerFloor;
                playerExp += totalExpGainedThisFloor;
                int lev = 1;
                for(lev=1; lev <= 99; lev++)
                {
                    int expReq = expValues[lev - 1];
                    if(expReq > playerExp)
                    {
                        break;
                    }
                }
                playerLev = lev;
                int playerStrRange = strMax[playerCharNum] - strMin[playerCharNum];
                double levProgress = (lev - 1) / 98.0;
                int playerStr = (int)(strMin[playerCharNum] + levProgress * playerStrRange);
                int playerAgiRange = agiMax[playerCharNum] - agiMin[playerCharNum];
                int playerAgi = (int)(agiMin[playerCharNum] + levProgress * playerAgiRange);

                int enemyDefThisFloor = (int)(enemyDef.baseValue + Math.Pow(i, enemyDef.exponent) * enemyDef.growthValue * floorScalingFactor);
                enemyDefThisFloor = (int)(enemyDefThisFloor * bossStatMultiplier);
                int enemyEvadeThisFloor = (int)(enemyEvade.baseValue + Math.Pow(i, enemyEvade.exponent) * enemyEvade.growthValue * floorScalingFactor);
                enemyDefThisFloor = (int)(enemyDefThisFloor * bossStatMultiplier);
                int enemyHpThisFloor = (int)(enemyHp.baseValue + Math.Pow(i, enemyHp.exponent) * enemyHp.growthValue * floorScalingFactor);
                enemyHpThisFloor = (int)(enemyHpThisFloor * bossHpMultiplier);
                int manaPowerThisFloor = (int)(manaPower.baseValue + Math.Pow(i, manaPower.exponent) * manaPower.growthValue * floorScalingFactor);
                if (manaPowerThisFloor > 8)
                {
                    manaPowerThisFloor = 8;
                }

                int damageOut;
                int critDamageOut;

                int weaponHit = 75;
                // a weapon level every ~3 floors
                double estWeaponLev = i / 3.0;
                if(estWeaponLev > 9)
                {
                    estWeaponLev = 9;
                }
                int weaponDamage = (int)Math.Round(5 + Math.Pow(estWeaponLev + 1, 1.25) * 8);
                calculatePhysicalDamage(playerStr, playerAgi, weaponHit, weaponDamage, 0, manaPowerThisFloor, enemyDefThisFloor, enemyEvadeThisFloor, out damageOut, out critDamageOut);
                
                if (divideByTotalHp)
                {
                    damageOut = (int)Math.Ceiling(enemyHpThisFloor / (double)damageOut);
                    critDamageOut = (int)Math.Ceiling(enemyHpThisFloor / (double)critDamageOut);
                }

                points.Add(critDamage ? critDamageOut : damageOut);

            }


            return points;
        }

        public static List<int> generatePhysicalDamageToPlayersCurve(int numFloors, int playerCharNum, int estEnemiesKilledPerFloor,
            IntGrowthValue enemyStr, IntGrowthValue enemyAgi,
            IntGrowthValue enemyWeaponPower, IntGrowthValue enemyWeaponLevel,
            IntGrowthValue enemyExp,
            IntGrowthValue manaPower, bool critDamage, bool divideByTotalHp,
            int evadeProgression, int armorProgression, double bossStatMultiplier,
            double floorScalingFactor)
        {
            List<int> points = new List<int>();
            // stats:
            // boy str, agi 15-99
            // girl str 10-80, agi 10-85
            // sprite str 7-75, agi 12-75
            // exp 2=16, 3=47, 4=105...
            int[] conMin = new int[] { 13, 12, 10 };
            int[] conMax = new int[] { 99, 75, 50 };
            int[] hpMin = new int[] { 50, 45, 40 };
            int[] hpMax = new int[] { 999, 800, 800 };

            int playerLev = 1;
            int playerExp = 0;
            for (int i = 0; i < numFloors; i++)
            {
                int totalExpGainedThisFloor = (int)(enemyExp.baseValue + Math.Pow(i, enemyExp.exponent) * enemyExp.growthValue * floorScalingFactor) * estEnemiesKilledPerFloor;
                playerExp += totalExpGainedThisFloor;
                int lev = 1;
                for (lev = 1; lev <= 99; lev++)
                {
                    int expReq = expValues[lev - 1];
                    if (expReq > playerExp)
                    {
                        break;
                    }
                }
                playerLev = lev;
                int playerConRange = conMax[playerCharNum] - conMin[playerCharNum];
                double levProgress = (lev - 1) / 98.0;
                int playerCon = (int)(conMin[playerCharNum] + levProgress * playerConRange);
                int playerHpRange = hpMax[playerCharNum] - hpMin[playerCharNum];
                int playerHp = (int)(hpMin[playerCharNum] + levProgress * playerHpRange);

                int enemyStrThisFloor = (int)(enemyStr.baseValue + Math.Pow(i, enemyStr.exponent) * enemyStr.growthValue * floorScalingFactor);
                enemyStrThisFloor = (int)(enemyStrThisFloor * bossStatMultiplier);
                int enemyAgiThisFloor = (int)(enemyAgi.baseValue + Math.Pow(i, enemyAgi.exponent) * enemyAgi.growthValue * floorScalingFactor);
                enemyAgiThisFloor = (int)(enemyAgiThisFloor * bossStatMultiplier);
                int enemyWeaponLevelThisFloor = (int)(enemyWeaponLevel.baseValue + Math.Pow(i, enemyWeaponLevel.exponent) * enemyWeaponLevel.growthValue * floorScalingFactor);
                enemyWeaponLevelThisFloor = (int)(enemyWeaponLevelThisFloor * bossStatMultiplier);
                if (enemyWeaponLevelThisFloor > 8)
                {
                    enemyWeaponLevelThisFloor = 8;
                }
                int enemyWeaponDamageThisFloor = (int)(enemyWeaponPower.baseValue + Math.Pow(i, enemyWeaponPower.exponent) * enemyWeaponPower.growthValue * floorScalingFactor);
                enemyWeaponDamageThisFloor = (int)(enemyWeaponDamageThisFloor * bossStatMultiplier);
                int manaPowerThisFloor = (int)(manaPower.baseValue + Math.Pow(i, manaPower.exponent) * manaPower.growthValue * floorScalingFactor);
                if (manaPowerThisFloor > 8)
                {
                    manaPowerThisFloor = 8;
                }

                int armorEstimate = i * armorProgression;
                int playerDef = playerCon + armorEstimate;

                int damageOut;
                int critDamageOut;

                int weaponHit = 99;
                // 37 base because you start with a chest piece
                int playerEvade = 37 + i * evadeProgression;
                if(playerEvade > 75)
                {
                    playerEvade = 75;
                }

                calculatePhysicalDamage(enemyStrThisFloor, enemyAgiThisFloor, weaponHit, enemyWeaponDamageThisFloor, enemyWeaponLevelThisFloor, manaPowerThisFloor, playerDef, playerEvade, out damageOut, out critDamageOut);

                if(divideByTotalHp)
                {
                    damageOut = (int)Math.Ceiling(playerHp / (double)damageOut);
                    critDamageOut = (int)Math.Ceiling(playerHp / (double)critDamageOut);
                }
                points.Add(critDamage ? critDamageOut : damageOut);

            }


            return points;
        }

        public static List<int> generateMagicDamageToEnemiesCurve(int numFloors, int spellPower, int estEnemiesKilledPerFloor,
            IntGrowthValue enemyMdef, IntGrowthValue enemyMevade,
            IntGrowthValue enemyHp,
            IntGrowthValue enemyExp,
            IntGrowthValue manaPower, bool critDamage, bool divideByTotalHp, double bossStatMultiplier, double bossHpMultiplier,
            double floorScalingFactor)
        {
            List<int> points = new List<int>();
            // stats:
            // boy str, agi 15-99
            // girl str 10-80, agi 10-85
            // sprite str 7-75, agi 12-75
            // exp 2=16, 3=47, 4=105...
            // spell power 0x2B, 0x3D

            int intWisMin = 15;
            int intWisMax = 99;

            int playerLev = 1;
            int playerExp = 0;
            for (int i = 0; i < numFloors; i++)
            {
                int totalExpGainedThisFloor = (int)(enemyExp.baseValue + Math.Pow(i, enemyExp.exponent) * enemyExp.growthValue * floorScalingFactor) * estEnemiesKilledPerFloor;
                playerExp += totalExpGainedThisFloor;
                int lev = 1;
                for (lev = 1; lev <= 99; lev++)
                {
                    int expReq = expValues[lev - 1];
                    if (expReq > playerExp)
                    {
                        break;
                    }
                }
                playerLev = lev;

                int playerIntWisRange = intWisMax - intWisMin;
                double levProgress = (lev - 1) / 98.0;
                int playerIntWis = (int)(intWisMin + levProgress * playerIntWisRange);
                int enemyMdefThisFloor = (int)(enemyMdef.baseValue + Math.Pow(i, enemyMdef.exponent) * enemyMdef.growthValue * floorScalingFactor);
                enemyMdefThisFloor = (int)(enemyMdefThisFloor * bossStatMultiplier);
                int enemyMevadeThisFloor = (int)(enemyMevade.baseValue + Math.Pow(i, enemyMevade.exponent) * enemyMevade.growthValue * floorScalingFactor);
                enemyMevadeThisFloor = (int)(enemyMevadeThisFloor * bossStatMultiplier);
                int enemyHpThisFloor = (int)(enemyHp.baseValue + Math.Pow(i, enemyHp.exponent) * enemyHp.growthValue * floorScalingFactor);
                enemyHpThisFloor = (int)(enemyHpThisFloor * bossHpMultiplier);
                int manaPowerThisFloor = (int)(manaPower.baseValue + Math.Pow(i, manaPower.exponent) * manaPower.growthValue * floorScalingFactor);
                if(manaPowerThisFloor > 8)
                {
                    manaPowerThisFloor = 8;
                }
                int damageOut;
                calculateMagicDamage(playerIntWis, 75, manaPowerThisFloor, spellPower, enemyMdefThisFloor, enemyMevadeThisFloor, out damageOut);
                if (divideByTotalHp)
                {
                    damageOut = (int)Math.Ceiling(enemyHpThisFloor / (double)damageOut);
                }

                points.Add(damageOut);
                
            }


            return points;
        }


        public static List<int> generateMagicDamageToPlayersCurve(int numFloors, int playerCharNum, int spellDamage, int estEnemiesKilledPerFloor,
            IntGrowthValue enemyInt,
            IntGrowthValue enemyMagicLevel,
            IntGrowthValue enemyExp,
            IntGrowthValue manaPower, bool divideByTotalHp,
            int evadeProgression, int armorProgression, double bossStatMultiplier,
            double floorScalingFactor)
        {
            List<int> points = new List<int>();
            // stats:
            // boy str, agi 15-99
            // girl str 10-80, agi 10-85
            // sprite str 7-75, agi 12-75
            // exp 2=16, 3=47, 4=105...
            int[] wisMin = new int[] { 5, 15, 5 };
            int[] wisMax = new int[] { 75, 99, 50 };
            int[] hpMin = new int[] { 50, 45, 40 };
            int[] hpMax = new int[] { 999, 800, 800 };

            int playerLev = 1;
            int playerExp = 0;
            for (int i = 0; i < numFloors; i++)
            {
                int totalExpGainedThisFloor = (int)(enemyExp.baseValue + Math.Pow(i, enemyExp.exponent) * enemyExp.growthValue * floorScalingFactor) * estEnemiesKilledPerFloor;
                playerExp += totalExpGainedThisFloor;
                int lev = 1;
                for (lev = 1; lev <= 99; lev++)
                {
                    int expReq = expValues[lev - 1];
                    if (expReq > playerExp)
                    {
                        break;
                    }
                }
                playerLev = lev;
                int playerWisRange = wisMax[playerCharNum] - wisMin[playerCharNum];
                double levProgress = (lev - 1) / 98.0;
                int playerWis = (int)(wisMin[playerCharNum] + levProgress * playerWisRange);
                int playerHpRange = hpMax[playerCharNum] - hpMin[playerCharNum];
                int playerHp = (int)(hpMin[playerCharNum] + levProgress * playerHpRange);

                int enemyIntThisFloor = (int)(enemyInt.baseValue + Math.Pow(i, enemyInt.exponent) * enemyInt.growthValue * floorScalingFactor);
                enemyIntThisFloor = (int)(enemyIntThisFloor * bossStatMultiplier);
                int enemyMagicLevelThisFloor = (int)(enemyMagicLevel.baseValue + Math.Pow(i, enemyMagicLevel.exponent) * enemyMagicLevel.growthValue * floorScalingFactor);
                enemyMagicLevelThisFloor = (int)(enemyMagicLevelThisFloor * bossStatMultiplier);
                if (enemyMagicLevelThisFloor > 8)
                {
                    enemyMagicLevelThisFloor = 8;
                }
                int manaPowerThisFloor = (int)(manaPower.baseValue + Math.Pow(i, manaPower.exponent) * manaPower.growthValue * floorScalingFactor);
                if (manaPowerThisFloor > 8)
                {
                    manaPowerThisFloor = 8;
                }

                int armorEstimate = i * armorProgression;
                int playerMdef = playerWis + armorEstimate;

                int damageOut;
                int playerMevade = i * evadeProgression;
                if (playerMevade > 75)
                {
                    playerMevade = 75;
                }

                calculateMagicDamage(enemyIntThisFloor, 75, enemyMagicLevelThisFloor, spellDamage, playerMdef, playerMevade, out damageOut);

                if (divideByTotalHp)
                {
                    damageOut = (int)Math.Ceiling(playerHp / (double)damageOut);
                }
                points.Add(damageOut);

            }


            return points;
        }
    }
}
