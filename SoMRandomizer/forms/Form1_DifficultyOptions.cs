using SoMRandomizer.config;
using SoMRandomizer.config.settings;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SoMRandomizer.forms
{
    // parts of Form1 related to difficulty selection for the older modes.
    public partial class Form1
    {
        private void initDifficultyPaneComponents()
        {
            visOptions["Est. kills per floor"] = 4;
            visOptions["Evade progression"] = 30;
            visOptions["Armor progression"] = 5;

            difficulties[AncientCaveSettings.MODE_KEY] = new Dictionary<string, DifficultySettings>();
            difficulties[BossRushSettings.MODE_KEY] = new Dictionary<string, DifficultySettings>();
            difficulties[ChaosSettings.MODE_KEY] = new Dictionary<string, DifficultySettings>();
            // difficulty for other (newer) modes is determined separately, this class is a bit of a relic

            DifficultySettings casualDifficulty = new DifficultySettings();
            casualDifficulty.readFromResourceFile("difficulty_casual.properties", null);
            difficulties[AncientCaveSettings.MODE_KEY]["Casual"] = casualDifficulty;
            DifficultySettings hardDifficulty = new DifficultySettings();
            hardDifficulty.readFromResourceFile("difficulty_hard.properties", null);
            difficulties[AncientCaveSettings.MODE_KEY]["Hard"] = hardDifficulty;
            DifficultySettings reallyhardDifficulty = new DifficultySettings();
            reallyhardDifficulty.readFromResourceFile("difficulty_reallyhard.properties", null);
            difficulties[AncientCaveSettings.MODE_KEY]["Really hard"] = reallyhardDifficulty;

            DifficultySettings casualDifficultyBr = new DifficultySettings();
            casualDifficultyBr.readFromResourceFile("difficulty_bossrush_casual.properties", null);
            difficulties[BossRushSettings.MODE_KEY]["Casual"] = casualDifficultyBr;
            DifficultySettings hardDifficultyBr = new DifficultySettings();
            hardDifficultyBr.readFromResourceFile("difficulty_bossrush_hard.properties", null);
            difficulties[BossRushSettings.MODE_KEY]["Hard"] = hardDifficultyBr;
            DifficultySettings reallyhardDifficultyBr = new DifficultySettings();
            reallyhardDifficultyBr.readFromResourceFile("difficulty_bossrush_reallyhard.properties", null);
            difficulties[BossRushSettings.MODE_KEY]["Really hard"] = reallyhardDifficultyBr;

            DifficultySettings casualDifficultyChaos = new DifficultySettings();
            casualDifficultyChaos.readFromResourceFile("difficulty_chaos_casual.properties", null);
            difficulties[ChaosSettings.MODE_KEY]["Casual"] = casualDifficultyChaos;
            DifficultySettings hardDifficultyChaos = new DifficultySettings();
            hardDifficultyChaos.readFromResourceFile("difficulty_chaos_hard.properties", null);
            difficulties[ChaosSettings.MODE_KEY]["Hard"] = hardDifficultyChaos;
            DifficultySettings reallyhardDifficultyChaos = new DifficultySettings();
            reallyhardDifficultyChaos.readFromResourceFile("difficulty_chaos_reallyhard.properties", null);
            difficulties[ChaosSettings.MODE_KEY]["Really hard"] = reallyhardDifficultyChaos;

            customDifficulty[AncientCaveSettings.MODE_KEY] = new DifficultySettings(casualDifficulty);
            customDifficulty[BossRushSettings.MODE_KEY] = new DifficultySettings(casualDifficultyBr);
            customDifficulty[ChaosSettings.MODE_KEY] = new DifficultySettings(casualDifficultyChaos);

            Dictionary<string, string> dispStrings = DifficultySettings.displayStrings;
            foreach (string dispString in dispStrings.Keys)
            {
                comboBox5.Items.Add(dispString);
            }
            t.SetToolTip(numericUpDown1, "Value of the stat on the first floor.");
            t.SetToolTip(numericUpDown2, "Linear growth rate of the stat per floor.");
            t.SetToolTip(numericUpDown4, "Exponential growth rate of the stat per floor.");
            t.SetToolTip(button5, "Save your difficulty settings to a reloadable file.");
            t.SetToolTip(button6, "Load difficulty settings from a previously-saved file.\n" + "Any missing values will default to \"Casual\" values.");
            t.SetToolTip(comboBox7, "Some options for how the graph below is displayed.\n" +
                "These are not part of the difficulty setting and only affect the display.\n" +
                "Options include:\n\n" +
                "- Est. kills per floor: used to model the amount of\n" +
                "experience gained per floor to estimate player stats.\n" +
                "If you plan to grind, raise this number.  If you plan to\n" +
                "blow through floors faster, lower this number.\n\n" +
                "- Evade progression: player evade is only gained through\n" +
                "equipping any piece of armor on a slot; with all three slots\n" +
                "filled your evade will be 75.  This models the evade gain per\n" +
                "floor based on equipment found/bought.  Values above 75 are\n" +
                "clipped down to 75.\n\n" +
                "- Armor progression: models the amount of defense gained per\n" +
                "floor from buying and finding armor.");
            t.SetToolTip(comboBox8, "Enemy stat progression level.");
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox5.SelectedIndex < 0)
            {
                return;
            }
            string dispString = (string)comboBox5.SelectedItem;
            bool isGrowth = DifficultySettings.isGrowthValue(dispString);
            if (isGrowth)
            {
                label13.Visible = true;
                label16.Visible = true;
                numericUpDown2.Visible = true;
                numericUpDown4.Visible = true;
                label12.Text = "Base value";
            }
            else
            {
                label13.Visible = false;
                label16.Visible = false;
                numericUpDown2.Visible = false;
                numericUpDown4.Visible = false;
                label12.Text = "Value";
            }

            string romType = getRomType();
            DifficultySettings diff = customDifficulty[romType];
            if (difficulties.ContainsKey(romType) && difficulties[romType].ContainsKey(selectedDifficulty))
            {
                diff = difficulties[romType][selectedDifficulty];
            }
            if (isGrowth)
            {
                IntGrowthValue igv = diff.getGrowthValue(dispString);
                numericUpDown1.Value = (decimal)igv.baseValue;
                numericUpDown2.Value = (decimal)igv.growthValue;
                numericUpDown4.Value = (decimal)igv.exponent;
            }
            else
            {
                double val = diff.getDoubleValue(dispString);
                numericUpDown1.Value = (decimal)val;
            }
            comboBox6_SelectedIndexChanged(null, null);
        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (selectedDifficulty == null)
            {
                return;
            }
            if (comboBox3.SelectedIndex < 0)
            {
                return;
            }

            string romType = getRomType();
            DifficultySettings diff = customDifficulty[romType];
            if (difficulties.ContainsKey(romType) && difficulties[romType].ContainsKey(selectedDifficulty))
            {
                diff = difficulties[romType][selectedDifficulty];
            }
            int numFloors = (comboBox3.SelectedIndex + 1) * 8;
            if (tabControl1.SelectedTab.Text == "Chaos")
            {
                numFloors = trackBar1.Value;
            }

            switch (comboBox6.SelectedIndex)
            {
                case 0:
                    DifficultyVisualizer.visualizePhysicalDamageToEnemies(pictureBox2, diff, numFloors, false, visOptions["Est. kills per floor"], false);
                    break;
                case 1:
                    DifficultyVisualizer.visualizePhysicalDamageToPlayers(pictureBox2, diff, numFloors, false, visOptions["Est. kills per floor"], visOptions["Evade progression"], visOptions["Armor progression"], false);
                    break;
                case 2:
                    DifficultyVisualizer.visualizePhysicalDamageToPlayers(pictureBox2, diff, numFloors, true, visOptions["Est. kills per floor"], visOptions["Evade progression"], visOptions["Armor progression"], false);
                    break;
                case 3:
                    DifficultyVisualizer.visualizePhysicalDamageToEnemies(pictureBox2, diff, numFloors, true, visOptions["Est. kills per floor"], false);
                    break;
                case 4:
                    DifficultyVisualizer.visualizeMagicDamageToEnemies(pictureBox2, diff, numFloors, false, visOptions["Est. kills per floor"], false);
                    break;
                case 5:
                    DifficultyVisualizer.visualizeMagicDamageToPlayers(pictureBox2, diff, numFloors, false, visOptions["Est. kills per floor"], visOptions["Evade progression"], visOptions["Armor progression"], false);
                    break;
                case 6:
                    DifficultyVisualizer.visualizeMagicDamageToPlayers(pictureBox2, diff, numFloors, true, visOptions["Est. kills per floor"], visOptions["Evade progression"], visOptions["Armor progression"], false);
                    break;
                case 7:
                    DifficultyVisualizer.visualizeMagicDamageToEnemies(pictureBox2, diff, numFloors, true, visOptions["Est. kills per floor"], false);
                    break;
                case 8:
                    DifficultyVisualizer.visualizePhysicalDamageToEnemies(pictureBox2, diff, numFloors, false, visOptions["Est. kills per floor"], true);
                    break;
                case 9:
                    DifficultyVisualizer.visualizePhysicalDamageToPlayers(pictureBox2, diff, numFloors, false, visOptions["Est. kills per floor"], visOptions["Evade progression"], visOptions["Armor progression"], true);
                    break;
                case 10:
                    DifficultyVisualizer.visualizePhysicalDamageToPlayers(pictureBox2, diff, numFloors, true, visOptions["Est. kills per floor"], visOptions["Evade progression"], visOptions["Armor progression"], true);
                    break;
                case 11:
                    DifficultyVisualizer.visualizePhysicalDamageToEnemies(pictureBox2, diff, numFloors, true, visOptions["Est. kills per floor"], true);
                    break;
                case 12:
                    DifficultyVisualizer.visualizeMagicDamageToEnemies(pictureBox2, diff, numFloors, false, visOptions["Est. kills per floor"], true);
                    break;
                case 13:
                    DifficultyVisualizer.visualizeMagicDamageToPlayers(pictureBox2, diff, numFloors, false, visOptions["Est. kills per floor"], visOptions["Evade progression"], visOptions["Armor progression"], true);
                    break;
                case 14:
                    DifficultyVisualizer.visualizeMagicDamageToPlayers(pictureBox2, diff, numFloors, true, visOptions["Est. kills per floor"], visOptions["Evade progression"], visOptions["Armor progression"], true);
                    break;
                case 15:
                    DifficultyVisualizer.visualizeMagicDamageToEnemies(pictureBox2, diff, numFloors, true, visOptions["Est. kills per floor"], true);
                    break;
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (comboBox5.SelectedIndex < 0)
            {
                return;
            }

            string romType = getRomType();
            string selectedDifficulty = "";
            switch (romType)
            {
                case AncientCaveSettings.MODE_KEY:
                    selectedDifficulty = (string)comboBox1.SelectedItem;
                    break;
                case BossRushSettings.MODE_KEY:
                    selectedDifficulty = (string)comboBox8.SelectedItem;
                    break;
                case ChaosSettings.MODE_KEY:
                    selectedDifficulty = (string)comboBox9.SelectedItem;
                    break;
                case VanillaRandoSettings.MODE_KEY:
                    // unused by mode
                    selectedDifficulty = "casual";
                    break;
                case OpenWorldSettings.MODE_KEY:
                    // unused by mode
                    selectedDifficulty = "casual";
                    break;
            }

            if (selectedDifficulty != "Custom")
            {
                return;
            }

            string dispString = (string)comboBox5.SelectedItem;
            bool isGrowth = DifficultySettings.isGrowthValue(dispString);

            if (isGrowth)
            {
                double baseValue = (double)numericUpDown1.Value;
                double growValue = (double)numericUpDown2.Value;
                double exp = (double)numericUpDown4.Value;
                IntGrowthValue igv = new IntGrowthValue();
                igv.baseValue = baseValue;
                igv.growthValue = growValue;
                igv.exponent = exp;
                customDifficulty[romType].setGrowthValue(dispString, igv);
            }
            else
            {
                double value = (double)numericUpDown1.Value;
                customDifficulty[romType].setDoubleValue(dispString, value);
            }
            // force graph redraw
            comboBox6_SelectedIndexChanged(null, null);
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            numericUpDown1_ValueChanged(null, null);
            // force graph redraw
            comboBox6_SelectedIndexChanged(null, null);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedDifficulty = (string)comboBox1.SelectedItem;
            bool customSelected = (string)comboBox1.SelectedItem == "Custom";
            numericUpDown1.Enabled = customSelected;
            numericUpDown2.Enabled = customSelected;
            numericUpDown4.Enabled = customSelected;
            button6.Enabled = customSelected;
            refreshWidth();
            comboBox5_SelectedIndexChanged(null, null);
        }

        private void refreshWidth()
        {
            if (tabControl1.SelectedTab == null)
            {
                return;
            }
            bool customSelected = false;
            if (tabControl1.SelectedTab.Text == "Ancient Cave")
            {
                customSelected = (string)comboBox1.SelectedItem == "Custom";
            }
            else if (tabControl1.SelectedTab.Text == "Boss Rush")
            {
                customSelected = (string)comboBox8.SelectedItem == "Custom";
            }
            else if (tabControl1.SelectedTab.Text == "Chaos")
            {
                customSelected = (string)comboBox9.SelectedItem == "Custom";
            }


            if (difficultyPopout || customSelected)
            {
                ClientSize = new Size(difficultyWidth, ClientSize.Height);
            }
            if (!customSelected && !difficultyPopout)
            {
                ClientSize = new Size(noDifficultyWidth, ClientSize.Height);
            }
        }
        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            string visOption = (string)comboBox7.SelectedItem;
            if (visOption == null)
            {
                return;
            }
            visOptions[visOption] = (int)numericUpDown3.Value;
            // force graph redraw
            comboBox6_SelectedIndexChanged(null, null);
        }

        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            string visOption = (string)comboBox7.SelectedItem;
            if (visOption == null)
            {
                return;
            }
            numericUpDown3.Value = visOptions[visOption];
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox5_SelectedIndexChanged(null, null);
        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            numericUpDown1_ValueChanged(null, null);
            // force graph redraw
            comboBox6_SelectedIndexChanged(null, null);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = "Properties File (*.properties)|*.properties";
            DialogResult d = sf.ShowDialog();
            string romType = getRomType();
            if (d == DialogResult.OK)
            {
                string filename = sf.FileName;
                try
                {
                    StreamWriter fileOut = new StreamWriter(filename);
                    DifficultySettings diff = customDifficulty[romType];
                    if (difficulties[romType].ContainsKey(selectedDifficulty))
                    {
                        diff = difficulties[romType][selectedDifficulty];
                    }
                    Dictionary<string, string> dispStrings = DifficultySettings.displayStrings;
                    foreach (string dispString in dispStrings.Keys)
                    {
                        string keyString = dispStrings[dispString];
                        if (DifficultySettings.isGrowthValue(dispString))
                        {
                            IntGrowthValue igv = diff.getGrowthValue(dispString);
                            fileOut.WriteLine(keyString + "_base=" + igv.baseValue);
                            fileOut.WriteLine(keyString + "_growth=" + igv.growthValue);
                            fileOut.WriteLine(keyString + "_exp=" + igv.exponent);
                        }
                        else
                        {
                            double dv = diff.getDoubleValue(dispString);
                            fileOut.WriteLine(keyString + "=" + dv);
                        }
                        fileOut.WriteLine();
                    }
                    fileOut.Close();
                }
                catch (Exception ee)
                {
                    MessageBox.Show("Unable to write to file.");
                }
            }

        }

        private void button6_Click(object sender, EventArgs e)
        {
            OpenFileDialog sf = new OpenFileDialog();
            sf.Filter = "Properties File (*.properties)|*.properties";
            sf.Multiselect = false;
            DialogResult d = sf.ShowDialog();
            string romType = getRomType();
            if (d == DialogResult.OK)
            {
                string filename = sf.FileName;
                try
                {
                    DifficultySettings newCustom = new DifficultySettings();
                    newCustom.readFromFile(filename, difficulties[romType]["Casual"]);
                    customDifficulty[romType] = newCustom;
                    comboBox5_SelectedIndexChanged(null, null);
                }
                catch (Exception ee)
                {
                    MessageBox.Show("Unable to read from file.  Verify all properties exist!");
                }
            }
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            difficultyPopout = !difficultyPopout;
            if (difficultyPopout)
            {
                linkLabel2.LinkColor = Color.Blue;
            }
            else
            {
                linkLabel2.LinkColor = Color.Red;
            }
            refreshWidth();
        }

        private void comboBox8_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedDifficulty = (string)comboBox8.SelectedItem;
            bool customSelected = (string)comboBox8.SelectedItem == "Custom";
            numericUpDown1.Enabled = customSelected;
            numericUpDown2.Enabled = customSelected;
            numericUpDown4.Enabled = customSelected;
            button6.Enabled = customSelected;
            refreshWidth();
            comboBox5_SelectedIndexChanged(null, null);
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            int val = trackBar1.Value;
            int newv = (int)Math.Round(val / 20.0) * 20;

            if (trackBar1.Value != newv)
            {
                trackBar1.Value = newv;
            }
            comboBox5_SelectedIndexChanged(null, null);
        }

        private void comboBox9_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedDifficulty = (string)comboBox9.SelectedItem;
            bool customSelected = (string)comboBox9.SelectedItem == "Custom";
            numericUpDown1.Enabled = customSelected;
            numericUpDown2.Enabled = customSelected;
            numericUpDown4.Enabled = customSelected;
            button6.Enabled = customSelected;
            refreshWidth();
            comboBox5_SelectedIndexChanged(null, null);
        }
    }
}
