using SoMRandomizer.config;
using SoMRandomizer.config.settings;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SoMRandomizer.forms
{
    // parts of MainForm related to difficulty selection for the older modes.
    public partial class MainForm
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
                cmbDifficultyValue.Items.Add(dispString);
            }
            t.SetToolTip(nudDifficultyBaseValue, "Value of the stat on the first floor.");
            t.SetToolTip(nudDifficultyLinearGrowth, "Linear growth rate of the stat per floor.");
            t.SetToolTip(nudDifficultyExponent, "Exponential growth rate of the stat per floor.");
            t.SetToolTip(btnDifficultySave, "Save your difficulty settings to a reloadable file.");
            t.SetToolTip(btnDifficultyLoad, "Load difficulty settings from a previously-saved file.\n" + "Any missing values will default to \"Casual\" values.");
            t.SetToolTip(cmbVisualizationOption, "Some options for how the graph below is displayed.\n" +
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
            t.SetToolTip(cmbBossRushDifficulty, "Enemy stat progression level.");
        }

        private void cmbDifficultyValue_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbDifficultyValue.SelectedIndex < 0)
            {
                return;
            }
            string dispString = (string)cmbDifficultyValue.SelectedItem;
            bool isGrowth = DifficultySettings.isGrowthValue(dispString);
            if (isGrowth)
            {
                label13.Visible = true;
                label16.Visible = true;
                nudDifficultyLinearGrowth.Visible = true;
                nudDifficultyExponent.Visible = true;
                label12.Text = "Base value";
            }
            else
            {
                label13.Visible = false;
                label16.Visible = false;
                nudDifficultyLinearGrowth.Visible = false;
                nudDifficultyExponent.Visible = false;
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
                nudDifficultyBaseValue.Value = (decimal)igv.baseValue;
                nudDifficultyLinearGrowth.Value = (decimal)igv.growthValue;
                nudDifficultyExponent.Value = (decimal)igv.exponent;
            }
            else
            {
                double val = diff.getDoubleValue(dispString);
                nudDifficultyBaseValue.Value = (decimal)val;
            }
            cmbDifficultyVisualize_SelectedIndexChanged(null, null);
        }

        private void cmbDifficultyVisualize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (selectedDifficulty == null)
            {
                return;
            }
            if (cmbACLength.SelectedIndex < 0)
            {
                return;
            }

            string romType = getRomType();
            DifficultySettings diff = customDifficulty[romType];
            if (difficulties.ContainsKey(romType) && difficulties[romType].ContainsKey(selectedDifficulty))
            {
                diff = difficulties[romType][selectedDifficulty];
            }
            int numFloors = (cmbACLength.SelectedIndex + 1) * 8;
            if (tabMode.SelectedTab.Text == "Chaos")
            {
                numFloors = trbChaosFloors.Value;
            }

            switch (cmbDifficultyVisualize.SelectedIndex)
            {
                case 0:
                    DifficultyVisualizer.visualizePhysicalDamageToEnemies(picDifficultyGraph, diff, numFloors, false, visOptions["Est. kills per floor"], false);
                    break;
                case 1:
                    DifficultyVisualizer.visualizePhysicalDamageToPlayers(picDifficultyGraph, diff, numFloors, false, visOptions["Est. kills per floor"], visOptions["Evade progression"], visOptions["Armor progression"], false);
                    break;
                case 2:
                    DifficultyVisualizer.visualizePhysicalDamageToPlayers(picDifficultyGraph, diff, numFloors, true, visOptions["Est. kills per floor"], visOptions["Evade progression"], visOptions["Armor progression"], false);
                    break;
                case 3:
                    DifficultyVisualizer.visualizePhysicalDamageToEnemies(picDifficultyGraph, diff, numFloors, true, visOptions["Est. kills per floor"], false);
                    break;
                case 4:
                    DifficultyVisualizer.visualizeMagicDamageToEnemies(picDifficultyGraph, diff, numFloors, false, visOptions["Est. kills per floor"], false);
                    break;
                case 5:
                    DifficultyVisualizer.visualizeMagicDamageToPlayers(picDifficultyGraph, diff, numFloors, false, visOptions["Est. kills per floor"], visOptions["Evade progression"], visOptions["Armor progression"], false);
                    break;
                case 6:
                    DifficultyVisualizer.visualizeMagicDamageToPlayers(picDifficultyGraph, diff, numFloors, true, visOptions["Est. kills per floor"], visOptions["Evade progression"], visOptions["Armor progression"], false);
                    break;
                case 7:
                    DifficultyVisualizer.visualizeMagicDamageToEnemies(picDifficultyGraph, diff, numFloors, true, visOptions["Est. kills per floor"], false);
                    break;
                case 8:
                    DifficultyVisualizer.visualizePhysicalDamageToEnemies(picDifficultyGraph, diff, numFloors, false, visOptions["Est. kills per floor"], true);
                    break;
                case 9:
                    DifficultyVisualizer.visualizePhysicalDamageToPlayers(picDifficultyGraph, diff, numFloors, false, visOptions["Est. kills per floor"], visOptions["Evade progression"], visOptions["Armor progression"], true);
                    break;
                case 10:
                    DifficultyVisualizer.visualizePhysicalDamageToPlayers(picDifficultyGraph, diff, numFloors, true, visOptions["Est. kills per floor"], visOptions["Evade progression"], visOptions["Armor progression"], true);
                    break;
                case 11:
                    DifficultyVisualizer.visualizePhysicalDamageToEnemies(picDifficultyGraph, diff, numFloors, true, visOptions["Est. kills per floor"], true);
                    break;
                case 12:
                    DifficultyVisualizer.visualizeMagicDamageToEnemies(picDifficultyGraph, diff, numFloors, false, visOptions["Est. kills per floor"], true);
                    break;
                case 13:
                    DifficultyVisualizer.visualizeMagicDamageToPlayers(picDifficultyGraph, diff, numFloors, false, visOptions["Est. kills per floor"], visOptions["Evade progression"], visOptions["Armor progression"], true);
                    break;
                case 14:
                    DifficultyVisualizer.visualizeMagicDamageToPlayers(picDifficultyGraph, diff, numFloors, true, visOptions["Est. kills per floor"], visOptions["Evade progression"], visOptions["Armor progression"], true);
                    break;
                case 15:
                    DifficultyVisualizer.visualizeMagicDamageToEnemies(picDifficultyGraph, diff, numFloors, true, visOptions["Est. kills per floor"], true);
                    break;
            }
        }

        private void nudDifficultyBaseValue_ValueChanged(object sender, EventArgs e)
        {
            if (cmbDifficultyValue.SelectedIndex < 0)
            {
                return;
            }

            string romType = getRomType();
            string selectedDifficulty = "";
            switch (romType)
            {
                case AncientCaveSettings.MODE_KEY:
                    selectedDifficulty = (string)cmbACDifficulty.SelectedItem;
                    break;
                case BossRushSettings.MODE_KEY:
                    selectedDifficulty = (string)cmbBossRushDifficulty.SelectedItem;
                    break;
                case ChaosSettings.MODE_KEY:
                    selectedDifficulty = (string)cmbChaosDifficulty.SelectedItem;
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

            string dispString = (string)cmbDifficultyValue.SelectedItem;
            bool isGrowth = DifficultySettings.isGrowthValue(dispString);

            if (isGrowth)
            {
                double baseValue = (double)nudDifficultyBaseValue.Value;
                double growValue = (double)nudDifficultyLinearGrowth.Value;
                double exp = (double)nudDifficultyExponent.Value;
                IntGrowthValue igv = new IntGrowthValue();
                igv.baseValue = baseValue;
                igv.growthValue = growValue;
                igv.exponent = exp;
                customDifficulty[romType].setGrowthValue(dispString, igv);
            }
            else
            {
                double value = (double)nudDifficultyBaseValue.Value;
                customDifficulty[romType].setDoubleValue(dispString, value);
            }
            // force graph redraw
            cmbDifficultyVisualize_SelectedIndexChanged(null, null);
        }

        private void nudDifficultyLinearGrowth_ValueChanged(object sender, EventArgs e)
        {
            nudDifficultyBaseValue_ValueChanged(null, null);
            // force graph redraw
            cmbDifficultyVisualize_SelectedIndexChanged(null, null);
        }

        private void cmbACDifficulty_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedDifficulty = (string)cmbACDifficulty.SelectedItem;
            bool customSelected = (string)cmbACDifficulty.SelectedItem == "Custom";
            nudDifficultyBaseValue.Enabled = customSelected;
            nudDifficultyLinearGrowth.Enabled = customSelected;
            nudDifficultyExponent.Enabled = customSelected;
            btnDifficultyLoad.Enabled = customSelected;
            refreshWidth();
            cmbDifficultyValue_SelectedIndexChanged(null, null);
        }

        private void refreshWidth()
        {
            if (tabMode.SelectedTab == null)
            {
                return;
            }
            bool customSelected = false;
            if (tabMode.SelectedTab.Text == "Ancient Cave")
            {
                customSelected = (string)cmbACDifficulty.SelectedItem == "Custom";
            }
            else if (tabMode.SelectedTab.Text == "Boss Rush")
            {
                customSelected = (string)cmbBossRushDifficulty.SelectedItem == "Custom";
            }
            else if (tabMode.SelectedTab.Text == "Chaos")
            {
                customSelected = (string)cmbChaosDifficulty.SelectedItem == "Custom";
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
        private void nudVisualizationOptionValue_ValueChanged(object sender, EventArgs e)
        {
            string visOption = (string)cmbVisualizationOption.SelectedItem;
            if (visOption == null)
            {
                return;
            }
            visOptions[visOption] = (int)nudVisualizationOptionValue.Value;
            // force graph redraw
            cmbDifficultyVisualize_SelectedIndexChanged(null, null);
        }

        private void cmbVisualizationOption_SelectedIndexChanged(object sender, EventArgs e)
        {
            string visOption = (string)cmbVisualizationOption.SelectedItem;
            if (visOption == null)
            {
                return;
            }
            nudVisualizationOptionValue.Value = visOptions[visOption];
        }

        private void cmbACLength_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbDifficultyValue_SelectedIndexChanged(null, null);
        }

        private void nudDifficultyExponent_ValueChanged(object sender, EventArgs e)
        {
            nudDifficultyBaseValue_ValueChanged(null, null);
            // force graph redraw
            cmbDifficultyVisualize_SelectedIndexChanged(null, null);
        }

        private void btnDifficultySave_Click(object sender, EventArgs e)
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

        private void btnDifficultyLoad_Click(object sender, EventArgs e)
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
                    cmbDifficultyValue_SelectedIndexChanged(null, null);
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
                llbDifficultyDetails.LinkColor = Color.Blue;
            }
            else
            {
                llbDifficultyDetails.LinkColor = Color.Red;
            }
            refreshWidth();
        }

        private void cmbBossRushDifficulty_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedDifficulty = (string)cmbBossRushDifficulty.SelectedItem;
            bool customSelected = (string)cmbBossRushDifficulty.SelectedItem == "Custom";
            nudDifficultyBaseValue.Enabled = customSelected;
            nudDifficultyLinearGrowth.Enabled = customSelected;
            nudDifficultyExponent.Enabled = customSelected;
            btnDifficultyLoad.Enabled = customSelected;
            refreshWidth();
            cmbDifficultyValue_SelectedIndexChanged(null, null);
        }

        private void trbChaosFloors_Scroll(object sender, EventArgs e)
        {
            int val = trbChaosFloors.Value;
            int newv = (int)Math.Round(val / 20.0) * 20;

            if (trbChaosFloors.Value != newv)
            {
                trbChaosFloors.Value = newv;
            }
            cmbDifficultyValue_SelectedIndexChanged(null, null);
        }

        private void cmbChaosDifficulty_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedDifficulty = (string)cmbChaosDifficulty.SelectedItem;
            bool customSelected = (string)cmbChaosDifficulty.SelectedItem == "Custom";
            nudDifficultyBaseValue.Enabled = customSelected;
            nudDifficultyLinearGrowth.Enabled = customSelected;
            nudDifficultyExponent.Enabled = customSelected;
            btnDifficultyLoad.Enabled = customSelected;
            refreshWidth();
            cmbDifficultyValue_SelectedIndexChanged(null, null);
        }
    }
}
