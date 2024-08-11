using SoMRandomizer.config;
using SoMRandomizer.util;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SoMRandomizer.forms
{
    /// <summary>
    /// Utilities for graphing difficulty values for earlier modes.
    /// May not be particularly accurate.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class DifficultyVisualizer
    {
        public static void visualizePhysicalDamageToEnemies(PictureBox pbox, DifficultySettings difficulty, int numFloors, bool numHits, int numKillsPerFloor, bool boss)
        {
            double scaleFactor = 24.0 / numFloors;
            Bitmap b = new Bitmap(pbox.Width, pbox.Height);
            Graphics g = Graphics.FromImage(b);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            List<int> valuesBoy = DamageCalculations.generatePhysicalDamageToEnemiesCurve(
                numFloors, 0, numKillsPerFloor, 
                difficulty.getGrowthValue(DifficultySettings.ENEMY_DEF),
                difficulty.getGrowthValue(DifficultySettings.ENEMY_EVADE),
                difficulty.getGrowthValue(DifficultySettings.ENEMY_HP),
                difficulty.getGrowthValue(DifficultySettings.ENEMY_EXP),
                difficulty.getGrowthValue(DifficultySettings.MANA_POWER), false, numHits,
                boss? difficulty.getDoubleValue(DifficultySettings.BOSS_STAT_MULTIPLIER) : 1.0,
                boss ? difficulty.getDoubleValue(DifficultySettings.BOSS_HP_MULTIPLIER) : 1.0, scaleFactor
                );
            List<int> valuesGirl = DamageCalculations.generatePhysicalDamageToEnemiesCurve(
                numFloors, 1, numKillsPerFloor,
                difficulty.getGrowthValue(DifficultySettings.ENEMY_DEF),
                difficulty.getGrowthValue(DifficultySettings.ENEMY_EVADE),
                difficulty.getGrowthValue(DifficultySettings.ENEMY_HP),
                difficulty.getGrowthValue(DifficultySettings.ENEMY_EXP),
                difficulty.getGrowthValue(DifficultySettings.MANA_POWER), false, numHits,
                boss ? difficulty.getDoubleValue(DifficultySettings.BOSS_STAT_MULTIPLIER) : 1.0,
                boss ? difficulty.getDoubleValue(DifficultySettings.BOSS_HP_MULTIPLIER) : 1.0, scaleFactor
                );
            List<int> valuesSprite = DamageCalculations.generatePhysicalDamageToEnemiesCurve(
                numFloors, 2, numKillsPerFloor,
                difficulty.getGrowthValue(DifficultySettings.ENEMY_DEF),
                difficulty.getGrowthValue(DifficultySettings.ENEMY_EVADE),
                difficulty.getGrowthValue(DifficultySettings.ENEMY_HP),
                difficulty.getGrowthValue(DifficultySettings.ENEMY_EXP),
                difficulty.getGrowthValue(DifficultySettings.MANA_POWER), false, numHits,
                boss ? difficulty.getDoubleValue(DifficultySettings.BOSS_STAT_MULTIPLIER) : 1.0,
                boss ? difficulty.getDoubleValue(DifficultySettings.BOSS_HP_MULTIPLIER) : 1.0, scaleFactor
                );

            Dictionary<Color, List<int>> allValues = new Dictionary<Color, List<int>>();
            allValues.Add(Color.Blue, valuesBoy);
            allValues.Add(Color.Red, valuesGirl);
            allValues.Add(Color.Green, valuesSprite);
            plot(allValues, g, pbox.Width, pbox.Height);
            pbox.Image = b;
        }

        public static void visualizePhysicalDamageToPlayers(PictureBox pbox, DifficultySettings difficulty, int numFloors, bool numHits, int numKillsPerFloor, int evadeProgression, int armorProgression, bool boss)
        {
            double scaleFactor = 24.0 / numFloors;
            Bitmap b = new Bitmap(pbox.Width, pbox.Height);
            Graphics g = Graphics.FromImage(b);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            List<int> valuesBoy = DamageCalculations.generatePhysicalDamageToPlayersCurve(
                numFloors, 0, numKillsPerFloor,
                difficulty.getGrowthValue(DifficultySettings.ENEMY_STR),
                difficulty.getGrowthValue(DifficultySettings.ENEMY_AGI),
                difficulty.getGrowthValue(DifficultySettings.ENEMY_WEAPON_DAMAGE),
                difficulty.getGrowthValue(DifficultySettings.ENEMY_WEAPON_LEV),
                difficulty.getGrowthValue(DifficultySettings.ENEMY_EXP),
                difficulty.getGrowthValue(DifficultySettings.MANA_POWER), false, numHits, evadeProgression, armorProgression,
                boss ? difficulty.getDoubleValue(DifficultySettings.BOSS_STAT_MULTIPLIER) : 1.0, scaleFactor
                );
            List<int> valuesGirl = DamageCalculations.generatePhysicalDamageToPlayersCurve(
                numFloors, 1, numKillsPerFloor,
                difficulty.getGrowthValue(DifficultySettings.ENEMY_STR),
                difficulty.getGrowthValue(DifficultySettings.ENEMY_AGI),
                difficulty.getGrowthValue(DifficultySettings.ENEMY_WEAPON_DAMAGE),
                difficulty.getGrowthValue(DifficultySettings.ENEMY_WEAPON_LEV),
                difficulty.getGrowthValue(DifficultySettings.ENEMY_EXP),
                difficulty.getGrowthValue(DifficultySettings.MANA_POWER), false, numHits, evadeProgression, armorProgression,
                boss ? difficulty.getDoubleValue(DifficultySettings.BOSS_STAT_MULTIPLIER) : 1.0, scaleFactor
                );
            List<int> valuesSprite = DamageCalculations.generatePhysicalDamageToPlayersCurve(
                numFloors, 2, numKillsPerFloor,
                difficulty.getGrowthValue(DifficultySettings.ENEMY_STR),
                difficulty.getGrowthValue(DifficultySettings.ENEMY_AGI),
                difficulty.getGrowthValue(DifficultySettings.ENEMY_WEAPON_DAMAGE),
                difficulty.getGrowthValue(DifficultySettings.ENEMY_WEAPON_LEV),
                difficulty.getGrowthValue(DifficultySettings.ENEMY_EXP),
                difficulty.getGrowthValue(DifficultySettings.MANA_POWER), false, numHits, evadeProgression, armorProgression,
                boss ? difficulty.getDoubleValue(DifficultySettings.BOSS_STAT_MULTIPLIER) : 1.0, scaleFactor
                );

            Dictionary<Color, List<int>> allValues = new Dictionary<Color, List<int>>();
            allValues.Add(Color.Blue, valuesBoy);
            allValues.Add(Color.Red, valuesGirl);
            allValues.Add(Color.Green, valuesSprite);
            plot(allValues, g, pbox.Width, pbox.Height);
            pbox.Image = b;
        }

        public static void visualizeMagicDamageToEnemies(PictureBox pbox, DifficultySettings difficulty, int numFloors, bool numHits, int numKillsPerFloor, bool boss)
        {
            double scaleFactor = 24.0 / numFloors;
            Bitmap b = new Bitmap(pbox.Width, pbox.Height);
            Graphics g = Graphics.FromImage(b);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            // 2b, 3d
            List<int> valuesLow = DamageCalculations.generateMagicDamageToEnemiesCurve(
                numFloors, 0x2B, numKillsPerFloor,
                difficulty.getGrowthValue(DifficultySettings.ENEMY_MDEF),
                difficulty.getGrowthValue(DifficultySettings.ENEMY_MEVADE),
                difficulty.getGrowthValue(DifficultySettings.ENEMY_HP),
                difficulty.getGrowthValue(DifficultySettings.ENEMY_EXP),
                difficulty.getGrowthValue(DifficultySettings.MANA_POWER), false, numHits,
                boss ? difficulty.getDoubleValue(DifficultySettings.BOSS_STAT_MULTIPLIER) : 1.0,
                boss ? difficulty.getDoubleValue(DifficultySettings.BOSS_HP_MULTIPLIER) : 1.0, scaleFactor
                );
            List<int> valuesHigh = DamageCalculations.generateMagicDamageToEnemiesCurve(
                numFloors, 0x3D, numKillsPerFloor,
                difficulty.getGrowthValue(DifficultySettings.ENEMY_MDEF),
                difficulty.getGrowthValue(DifficultySettings.ENEMY_MEVADE),
                difficulty.getGrowthValue(DifficultySettings.ENEMY_HP),
                difficulty.getGrowthValue(DifficultySettings.ENEMY_EXP),
                difficulty.getGrowthValue(DifficultySettings.MANA_POWER), false, numHits,
                boss ? difficulty.getDoubleValue(DifficultySettings.BOSS_STAT_MULTIPLIER) : 1.0,
                boss ? difficulty.getDoubleValue(DifficultySettings.BOSS_HP_MULTIPLIER) : 1.0, scaleFactor
                );

            Dictionary<Color, List<int>> allValues = new Dictionary<Color, List<int>>();
            allValues.Add(Color.FromArgb(254, 0, 0), valuesLow);
            allValues.Add(Color.FromArgb(255, 0, 0), valuesHigh);
            plot(allValues, g, pbox.Width, pbox.Height);
            pbox.Image = b;
        }

        public static void visualizeMagicDamageToPlayers(PictureBox pbox, DifficultySettings difficulty, int numFloors, bool numHits, int numKillsPerFloor, int evadeProgression, int armorProgression, bool boss)
        {
            double scaleFactor = 24.0 / numFloors;
            Bitmap b = new Bitmap(pbox.Width, pbox.Height);
            Graphics g = Graphics.FromImage(b);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            // 2b, 3d
            List<int> valuesLowBoy = DamageCalculations.generateMagicDamageToPlayersCurve(
                numFloors, 0, 0x2B, numKillsPerFloor,
                difficulty.getGrowthValue(DifficultySettings.ENEMY_INT),
                difficulty.getGrowthValue(DifficultySettings.ENEMY_MAGIC_LEV),
                difficulty.getGrowthValue(DifficultySettings.ENEMY_EXP),
                difficulty.getGrowthValue(DifficultySettings.MANA_POWER), numHits, evadeProgression, armorProgression,
                boss ? difficulty.getDoubleValue(DifficultySettings.BOSS_STAT_MULTIPLIER) : 1.0, scaleFactor
                );
            List<int> valuesHighBoy = DamageCalculations.generateMagicDamageToPlayersCurve(
                numFloors, 0, 0x3D, numKillsPerFloor,
                difficulty.getGrowthValue(DifficultySettings.ENEMY_INT),
                difficulty.getGrowthValue(DifficultySettings.ENEMY_MAGIC_LEV),
                difficulty.getGrowthValue(DifficultySettings.ENEMY_EXP),
                difficulty.getGrowthValue(DifficultySettings.MANA_POWER), numHits, evadeProgression, armorProgression,
                boss ? difficulty.getDoubleValue(DifficultySettings.BOSS_STAT_MULTIPLIER) : 1.0, scaleFactor
                );
            List<int> valuesLowGirl = DamageCalculations.generateMagicDamageToPlayersCurve(
                numFloors, 1, 0x2B, numKillsPerFloor,
                difficulty.getGrowthValue(DifficultySettings.ENEMY_INT),
                difficulty.getGrowthValue(DifficultySettings.ENEMY_MAGIC_LEV),
                difficulty.getGrowthValue(DifficultySettings.ENEMY_EXP),
                difficulty.getGrowthValue(DifficultySettings.MANA_POWER), numHits, evadeProgression, armorProgression,
                boss ? difficulty.getDoubleValue(DifficultySettings.BOSS_STAT_MULTIPLIER) : 1.0, scaleFactor
                );
            List<int> valuesHighGirl = DamageCalculations.generateMagicDamageToPlayersCurve(
                numFloors, 1, 0x3D, numKillsPerFloor,
                difficulty.getGrowthValue(DifficultySettings.ENEMY_INT),
                difficulty.getGrowthValue(DifficultySettings.ENEMY_MAGIC_LEV),
                difficulty.getGrowthValue(DifficultySettings.ENEMY_EXP),
                difficulty.getGrowthValue(DifficultySettings.MANA_POWER), numHits, evadeProgression, armorProgression,
                boss ? difficulty.getDoubleValue(DifficultySettings.BOSS_STAT_MULTIPLIER) : 1.0, scaleFactor
                );
            List<int> valuesLowSprite = DamageCalculations.generateMagicDamageToPlayersCurve(
                numFloors, 2, 0x2B, numKillsPerFloor,
                difficulty.getGrowthValue(DifficultySettings.ENEMY_INT),
                difficulty.getGrowthValue(DifficultySettings.ENEMY_MAGIC_LEV),
                difficulty.getGrowthValue(DifficultySettings.ENEMY_EXP),
                difficulty.getGrowthValue(DifficultySettings.MANA_POWER), numHits, evadeProgression, armorProgression,
                boss ? difficulty.getDoubleValue(DifficultySettings.BOSS_STAT_MULTIPLIER) : 1.0, scaleFactor
                );
            List<int> valuesHighSprite = DamageCalculations.generateMagicDamageToPlayersCurve(
                numFloors, 2, 0x3D, numKillsPerFloor,
                difficulty.getGrowthValue(DifficultySettings.ENEMY_INT),
                difficulty.getGrowthValue(DifficultySettings.ENEMY_MAGIC_LEV),
                difficulty.getGrowthValue(DifficultySettings.ENEMY_EXP),
                difficulty.getGrowthValue(DifficultySettings.MANA_POWER), numHits, evadeProgression, armorProgression,
                boss ? difficulty.getDoubleValue(DifficultySettings.BOSS_STAT_MULTIPLIER) : 1.0, scaleFactor
                );

            Dictionary<Color, List<int>> allValues = new Dictionary<Color, List<int>>();
            allValues.Add(Color.FromArgb(0, 0, 254), valuesLowBoy);
            allValues.Add(Color.FromArgb(0, 0, 255), valuesHighBoy);
            allValues.Add(Color.FromArgb(254, 0, 0), valuesLowGirl);
            allValues.Add(Color.FromArgb(255, 0, 0), valuesHighGirl);
            allValues.Add(Color.FromArgb(0, 254, 0), valuesLowSprite);
            allValues.Add(Color.FromArgb(0, 255, 0), valuesHighSprite);
            plot(allValues, g, pbox.Width, pbox.Height);
            pbox.Image = b;
        }

        private static void plot(Dictionary<Color, List<int>> allValues, Graphics g, int width, int height)
        {
            int min = 0;
            int max = 1;
            int numPoints = 0;
            foreach (List<int> values in allValues.Values)
            {
                foreach (int val in values)
                {
                    if (val > max)
                    {
                        max = val;
                    }
                }
                numPoints = values.Count;
            }

            // max .. 5, 10, 50, 100...
            int graphMax = 5;
            while(graphMax < max)
            {
                if(graphMax.ToString().Contains("5"))
                {
                    graphMax *= 2;
                }
                else
                {
                    graphMax *= 5;
                }
            }

            // five y-axis points plus zero
            int bufferArea = 20;
            int yDrawArea = height - bufferArea * 2;
            int xDrawArea = width - bufferArea * 2;
            // to convert y to screen y
            // height - bufferArea - ((ySrc / (double)graphMax) * yDrawArea)

            for (int axisY = 0; axisY <= graphMax; axisY += graphMax / 5)
            {
                float[] dashValues = new float[] { 5, 5 };
                int drawY = (int)(height - bufferArea - ((axisY / (double)graphMax) * yDrawArea));
                Pen pen = new Pen(Color.Black, 2);
                pen.DashPattern = dashValues;
                g.DrawLine(pen, bufferArea, drawY, width - bufferArea, drawY);
                g.DrawString("" + axisY, new Font(FontFamily.GenericSansSerif, 8), new SolidBrush(Color.Black), 0, drawY);
            }


            for(int floorNum=0; floorNum < numPoints; floorNum++)
            {
                int drawX = (int)(bufferArea + ((floorNum / (double)(numPoints - 1)) * xDrawArea));
                float[] dashValues = new float[] { 3, 3 };
                Pen pen = new Pen(Color.Black, 1);
                pen.DashPattern = dashValues;
                g.DrawLine(pen, drawX, bufferArea, drawX, height - bufferArea);
                g.DrawString("" + (floorNum+1), new Font(FontFamily.GenericSansSerif, 8), new SolidBrush(Color.Black), drawX - 4, height - bufferArea);
            }

            foreach (Color col in allValues.Keys)
            {
                List<int> values = allValues[col];
                for (int floorNum = 0; floorNum < numPoints - 1; floorNum++)
                {
                    int drawX1 = (int)(bufferArea + ((floorNum / (double)(values.Count - 1)) * xDrawArea));
                    int drawX2 = (int)(bufferArea + (((floorNum + 1) / (double)(values.Count - 1)) * xDrawArea));
                    int drawY1 = (int)(height - bufferArea - ((values[floorNum] / (double)graphMax) * yDrawArea));
                    int drawY2 = (int)(height - bufferArea - ((values[floorNum + 1] / (double)graphMax) * yDrawArea));
                    Pen pen = new Pen(col, 1);
                    g.DrawLine(pen, drawX1, drawY1, drawX2, drawY2);
                }
            }
        }
    }
}
