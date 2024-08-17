using SoMRandomizer.config.ui;
using SoMRandomizer.processing.common;
using SoMRandomizer.util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static SoMRandomizer.processing.common.VanillaBossMaps;
using static SoMRandomizer.processing.openworld.PlandoProperties;

namespace SoMRandomizer.forms
{
    /// <summary>
    /// Form used to select plando options for open world.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public partial class PlandoForm : Form
    {
        public event EventHandler PlandoSettingsChanged;
        ToolTip t = new ToolTip();
        bool suspendUpdates = false;

        public PlandoForm()
        {
            InitializeComponent();

            t.AutoPopDelay = 32767;
            t.SetToolTip(checkBox1, "So you can share your plando without it being obvious what you changed.");
            t.SetToolTip(checkBox3, "Don't run the checks making sure the ROM can be completed.\n\n" +
                "TO BE USED SPARINGLY!  Can easily create an uncompletable ROM.\n\n" +
                "Should only be used if you're sure the plando settings ALONE ensure a completable ROM,\n" + 
                "and that my checks are breaking somehow.\n\n" +
                "... Or, if you like, enable just this option and gamble on how far the generated settings actually allow you to get.");
            t.SetToolTip(pictureBox1, "The orb outside near the Sylphid temple.\n\n" +
                "This only matters if \"Flammie drum in logic\" is enabled, since\n\n" +
                "otherwise you can just fly to either side of it.\n\n");
            t.SetToolTip(pictureBox2, "Use Plandomizer to set specific open world settings that are\nusually determined randomly.\n\n" + 
                "Anything not specified here will be placed by the\nusual rando logic as best as possible, with your\nchoices placed first.\n\n" +
                "You do not need to specify everything here! Only the\nthings you want to override.\n\n" +
                "If you choose impossible settings, the seed generation will\nfail and the log should give you some idea what you\nneed to change.");
            t.SetToolTip(pictureBox3, 
                "Fire Palace 2 and 3 share the same event flag to unlock, so if\n\n" +
                "you set 3 to None, 2 will also effectively be None.");
            t.SetToolTip(pictureBox11, "Note that these settings override \"Force starting weapon\" under open world misc if\nspecified for the starting character.");
            t.SetToolTip(pictureBox7, "Only applicable if boss elements are shuffled.\n\nNo change: randomize as normal\n\nDefault: force vanilla element\n\nElementless: force plain element");
            t.SetToolTip(pictureBox5, "Note that there are some restrictions based on map, and also\nwhether a boss has already been placed.\n\nThese may change in the future.\n\n" + 
                "Also note, this currently only works if bosses are set to \"Random\" and not \"Swap\" or \"Vanilla\".\n\n" + 
                "Doom's Wall's map has special restrictions and is not part of \"Oops all.\"");

            button1.Click += Button1_Click;
            button2.Click += Button2_Click;
            FormClosing += MyForm_FormClosing;
            locationButtons.Add(button1);
            prizeButtons.Add(button2);
            allLocations.Add("");
            allPrizes.Add("");
            MaximizeBox = false;
            tabControl1.Enabled = false;

            // button 3, then 9-17 are element orb selection
            button3.Click += Button3_Click;
            button9.Click += Button3_Click;
            button10.Click += Button3_Click;
            button11.Click += Button3_Click;
            button12.Click += Button3_Click;
            button13.Click += Button3_Click;
            button14.Click += Button3_Click;
            button15.Click += Button3_Click;
            button16.Click += Button3_Click;
            button17.Click += Button3_Click;
            button18.Click += Button3_Click;
            button19.Click += Button3_Click;
            button20.Click += Button3_Click;
            button21.Click += Button3_Click;

            orbButtonPropertyNames[KEY_EARTH_PALACE_ORB_ELEMENT] = button3;
            orbButtonPropertyNames[KEY_MATANGO_ORB_ELEMENT] = button9;
            orbButtonPropertyNames[KEY_LUNA_PALACE_ORB_ELEMENT] = button10;
            orbButtonPropertyNames[KEY_GRAND_PALACE_ORB_1_ELEMENT] = button11;
            orbButtonPropertyNames[KEY_GRAND_PALACE_ORB_2_ELEMENT] = button12;
            orbButtonPropertyNames[KEY_GRAND_PALACE_ORB_3_ELEMENT] = button13;
            orbButtonPropertyNames[KEY_GRAND_PALACE_ORB_4_ELEMENT] = button14;
            orbButtonPropertyNames[KEY_GRAND_PALACE_ORB_5_ELEMENT] = button15;
            orbButtonPropertyNames[KEY_GRAND_PALACE_ORB_6_ELEMENT] = button16;
            orbButtonPropertyNames[KEY_GRAND_PALACE_ORB_7_ELEMENT] = button17;
            orbButtonPropertyNames[KEY_UPPER_LAND_ORB_ELEMENT] = button18;
            orbButtonPropertyNames[KEY_FIRE_PALACE_ORB_1_ELEMENT] = button21;
            orbButtonPropertyNames[KEY_FIRE_PALACE_ORB_2_ELEMENT] = button20;
            orbButtonPropertyNames[KEY_FIRE_PALACE_ORB_3_ELEMENT] = button19;
            foreach (string key in orbButtonPropertyNames.Keys)
            {
                orbButtonPropertyNamesInv[orbButtonPropertyNames[key]] = key;
            }

            // button 4,5,6,7,8 are weapons for boy/girl/sprite/p2/p3
            button4.Click += Button4_Click;
            button5.Click += Button4_Click;
            button6.Click += Button4_Click;
            button7.Click += Button4_Click;
            button8.Click += Button4_Click;
            weaponButtonPropertyNames[KEY_BOY_WEAPON] = button4;
            weaponButtonPropertyNames[KEY_GIRL_WEAPON] = button5;
            weaponButtonPropertyNames[KEY_SPRITE_WEAPON] = button6;
            weaponButtonPropertyNames[KEY_P2_WEAPON] = button7;
            weaponButtonPropertyNames[KEY_P3_WEAPON] = button8;
            foreach (string key in weaponButtonPropertyNames.Keys)
            {
                weaponButtonPropertyNamesInv[weaponButtonPropertyNames[key]] = key;
            }

            bossButtonPropertyNames[KEY_BOSS_MANTISANT] = button23;
            bossButtonPropertyNames[KEY_BOSS_TROPICALLO] = button25;
            bossButtonPropertyNames[KEY_BOSS_SPIKEY] = button31;
            bossButtonPropertyNames[KEY_BOSS_TONPOLE] = button27;
            bossButtonPropertyNames[KEY_BOSS_FIREGIGAS] = button33;
            bossButtonPropertyNames[KEY_BOSS_KILROY] = button61;
            bossButtonPropertyNames[KEY_BOSS_WALLFACE] = button45;
            bossButtonPropertyNames[KEY_BOSS_JABBERWOCKY] = button29;
            bossButtonPropertyNames[KEY_BOSS_SPRINGBEAK] = button35;
            bossButtonPropertyNames[KEY_BOSS_GREATVIPER] = button37;
            bossButtonPropertyNames[KEY_BOSS_BOREALFACE] = button39;
            bossButtonPropertyNames[KEY_BOSS_FROSTGIGAS] = button41;
            bossButtonPropertyNames[KEY_BOSS_MINOTAUR] = button43;
            bossButtonPropertyNames[KEY_BOSS_DOOMSWALL] = button47;
            bossButtonPropertyNames[KEY_BOSS_VAMPIRE] = button49;
            bossButtonPropertyNames[KEY_BOSS_METALMANTIS] = button51;
            bossButtonPropertyNames[KEY_BOSS_MECHRIDER2] = button53;
            bossButtonPropertyNames[KEY_BOSS_BLUESPIKE] = button55;
            bossButtonPropertyNames[KEY_BOSS_GORGONBULL] = button57;
            bossButtonPropertyNames[KEY_BOSS_HYDRA] = button59;
            bossButtonPropertyNames[KEY_BOSS_KETTLEKIN] = button65;
            bossButtonPropertyNames[KEY_BOSS_SNAPDRAGON] = button69;
            bossButtonPropertyNames[KEY_BOSS_WATERMELON] = button67;
            bossButtonPropertyNames[KEY_BOSS_HEXAS] = button71;
            bossButtonPropertyNames[KEY_BOSS_MECHRIDER3] = button73;
            bossButtonPropertyNames[KEY_BOSS_DRAGONWORM] = button75;
            bossButtonPropertyNames[KEY_BOSS_SNOWDRAGON] = button77;
            bossButtonPropertyNames[KEY_BOSS_AXEBEAK] = button79;
            bossButtonPropertyNames[KEY_BOSS_REDDRAGON] = button81;
            bossButtonPropertyNames[KEY_BOSS_THUNDERGIGAS] = button83;
            bossButtonPropertyNames[KEY_BOSS_BLUEDRAGON] = button85;
            bossButtonPropertyNames[KEY_BOSS_BUFFY] = button87;
            bossButtonPropertyNames[KEY_BOSS_DARKLICH] = button89;
            foreach (string key in bossButtonPropertyNames.Keys)
            {
                bossButtonPropertyNamesInv[bossButtonPropertyNames[key]] = key;
                bossButtonPropertyNames[key].Click += BossButton_Click;
            }

            bossElementButtonPropertyNames[KEY_BOSS_ELEMENT_MANTISANT] = button156;
            bossElementButtonPropertyNames[KEY_BOSS_ELEMENT_TROPICALLO] = button154;
            bossElementButtonPropertyNames[KEY_BOSS_ELEMENT_SPIKEY] = button148;
            bossElementButtonPropertyNames[KEY_BOSS_ELEMENT_TONPOLE] = button152;
            bossElementButtonPropertyNames[KEY_BOSS_ELEMENT_BITING_LIZARD] = button116;
            bossElementButtonPropertyNames[KEY_BOSS_ELEMENT_FIREGIGAS] = button146;
            bossElementButtonPropertyNames[KEY_BOSS_ELEMENT_KILROY] = button118;
            bossElementButtonPropertyNames[KEY_BOSS_ELEMENT_WALLFACE] = button134;
            bossElementButtonPropertyNames[KEY_BOSS_ELEMENT_JABBERWOCKY] = button150;
            bossElementButtonPropertyNames[KEY_BOSS_ELEMENT_SPRINGBEAK] = button144;
            bossElementButtonPropertyNames[KEY_BOSS_ELEMENT_GREATVIPER] = button142;
            bossElementButtonPropertyNames[KEY_BOSS_ELEMENT_BOREALFACE] = button140;
            bossElementButtonPropertyNames[KEY_BOSS_ELEMENT_FROSTGIGAS] = button138;
            bossElementButtonPropertyNames[KEY_BOSS_ELEMENT_MINOTAUR] = button136;
            bossElementButtonPropertyNames[KEY_BOSS_ELEMENT_DOOMSWALL] = button132;
            bossElementButtonPropertyNames[KEY_BOSS_ELEMENT_VAMPIRE] = button130;
            bossElementButtonPropertyNames[KEY_BOSS_ELEMENT_METALMANTIS] = button128;
            bossElementButtonPropertyNames[KEY_BOSS_ELEMENT_MECHRIDER2] = button126;
            bossElementButtonPropertyNames[KEY_BOSS_ELEMENT_BLUESPIKE] = button124;
            bossElementButtonPropertyNames[KEY_BOSS_ELEMENT_GORGONBULL] = button122;
            bossElementButtonPropertyNames[KEY_BOSS_ELEMENT_HYDRA] = button120;
            bossElementButtonPropertyNames[KEY_BOSS_ELEMENT_KETTLEKIN] = button114;
            bossElementButtonPropertyNames[KEY_BOSS_ELEMENT_SNAPDRAGON] = button110;
            bossElementButtonPropertyNames[KEY_BOSS_ELEMENT_WATERMELON] = button112;
            bossElementButtonPropertyNames[KEY_BOSS_ELEMENT_HEXAS] = button108;
            bossElementButtonPropertyNames[KEY_BOSS_ELEMENT_MECHRIDER3] = button106;
            bossElementButtonPropertyNames[KEY_BOSS_ELEMENT_DRAGONWORM] = button104;
            bossElementButtonPropertyNames[KEY_BOSS_ELEMENT_SNOWDRAGON] = button102;
            bossElementButtonPropertyNames[KEY_BOSS_ELEMENT_AXEBEAK] = button100;
            bossElementButtonPropertyNames[KEY_BOSS_ELEMENT_REDDRAGON] = button98;
            bossElementButtonPropertyNames[KEY_BOSS_ELEMENT_THUNDERGIGAS] = button96;
            bossElementButtonPropertyNames[KEY_BOSS_ELEMENT_BLUEDRAGON] = button94;
            bossElementButtonPropertyNames[KEY_BOSS_ELEMENT_BUFFY] = button92;
            bossElementButtonPropertyNames[KEY_BOSS_ELEMENT_DARKLICH] = button90;
            foreach (string key in bossElementButtonPropertyNames.Keys)
            {
                bossElementButtonPropertyNamesInv[bossElementButtonPropertyNames[key]] = key;
                bossElementButtonPropertyNames[key].Click += BossElementButton_Click;
            }

            button22.Click += OopsAllBossButton_Click;
        }

        int locations = 0;
        List<Button> locationButtons = new List<Button>();
        List<Button> prizeButtons = new List<Button>();

        List<string> allLocations = new List<string>();
        List<string> allPrizes = new List<string>();

        Dictionary<string, Button> orbButtonPropertyNames = new Dictionary<string, Button>();
        Dictionary<Button, string> orbButtonPropertyNamesInv = new Dictionary<Button, string>();
        Dictionary<string, string> orbElements = new Dictionary<string, string>();

        Dictionary<string, Button> weaponButtonPropertyNames = new Dictionary<string, Button>();
        Dictionary<Button, string> weaponButtonPropertyNamesInv = new Dictionary<Button, string>();
        Dictionary<string, string> weaponSettings = new Dictionary<string, string>();

        Dictionary<string, Button> bossButtonPropertyNames = new Dictionary<string, Button>();
        Dictionary<Button, string> bossButtonPropertyNamesInv = new Dictionary<Button, string>();
        Dictionary<string, string> bossSettings = new Dictionary<string, string>();

        Dictionary<string, Button> bossElementButtonPropertyNames = new Dictionary<string, Button>();
        Dictionary<Button, string> bossElementButtonPropertyNamesInv = new Dictionary<Button, string>();
        Dictionary<string, string> bossElementSettings = new Dictionary<string, string>();

        // due to hardcoded positions in vanilla game; may change later
        string[] singleInstanceBosses = new string[] {
            VALUE_BOSS_SPIKEY,
            VALUE_BOSS_BLUESPIKE,
            VALUE_BOSS_TROPICALLO,
            VALUE_BOSS_BOREALFACE,
            VALUE_BOSS_AXEBEAK,
            VALUE_BOSS_SPRINGBEAK,
            VALUE_BOSS_WALLFACE,
            VALUE_BOSS_DOOMSWALL
        };

        // due to using layer 2 for rendering
        string[] mapRestrictedBosses = new string[] {
            VALUE_BOSS_SNOWDRAGON,
            VALUE_BOSS_REDDRAGON,
            VALUE_BOSS_BLUEDRAGON,
            VALUE_BOSS_WALLFACE,
            VALUE_BOSS_DOOMSWALL,
            VALUE_BOSS_DARKLICH,
            VALUE_BOSS_WATERMELON
        };

        string[] mapRestrictedLocations = new string[] {
            KEY_BOSS_SNOWDRAGON,
            KEY_BOSS_REDDRAGON,
            KEY_BOSS_BLUEDRAGON,
            KEY_BOSS_WALLFACE,
            KEY_BOSS_DOOMSWALL,
            KEY_BOSS_DARKLICH,
            KEY_BOSS_WATERMELON
        };

        string[] allBossNameValues = new string[] {
            VALUE_BOSS_MANTISANT,
            VALUE_BOSS_TROPICALLO,
            VALUE_BOSS_SPIKEY,
            VALUE_BOSS_TONPOLE,
            VALUE_BOSS_FIREGIGAS,
            VALUE_BOSS_KILROY,
            VALUE_BOSS_WALLFACE,
            VALUE_BOSS_JABBERWOCKY,
            VALUE_BOSS_SPRINGBEAK,
            VALUE_BOSS_GREATVIPER,
            VALUE_BOSS_BOREALFACE,
            VALUE_BOSS_FROSTGIGAS,
            VALUE_BOSS_MINOTAUR,
            VALUE_BOSS_DOOMSWALL,
            VALUE_BOSS_VAMPIRE,
            VALUE_BOSS_METALMANTIS,
            VALUE_BOSS_MECHRIDER2,
            VALUE_BOSS_BLUESPIKE,
            VALUE_BOSS_GORGONBULL,
            VALUE_BOSS_HYDRA,
            VALUE_BOSS_KETTLEKIN,
            VALUE_BOSS_SNAPDRAGON,
            VALUE_BOSS_WATERMELON,
            VALUE_BOSS_HEXAS,
            VALUE_BOSS_MECHRIDER3,
            VALUE_BOSS_DRAGONWORM,
            VALUE_BOSS_SNOWDRAGON,
            VALUE_BOSS_AXEBEAK,
            VALUE_BOSS_REDDRAGON,
            VALUE_BOSS_THUNDERGIGAS,
            VALUE_BOSS_BLUEDRAGON,
            VALUE_BOSS_BUFFY,
            VALUE_BOSS_DARKLICH
        };
        bool bypassValidation = false;

        Dictionary<string, VanillaBossMap> bossmaps = new Dictionary<string, VanillaBossMap>
        {
            { KEY_BOSS_MANTISANT, VanillaBossMaps.MANTISANT },
            { KEY_BOSS_TROPICALLO, VanillaBossMaps.TROPICALLO },
            { KEY_BOSS_SPIKEY, VanillaBossMaps.SPIKEY },
            { KEY_BOSS_TONPOLE, VanillaBossMaps.TONPOLE },
            { KEY_BOSS_FIREGIGAS, VanillaBossMaps.FIREGIGAS },
            { KEY_BOSS_KILROY, VanillaBossMaps.KILROY },
            { KEY_BOSS_WALLFACE, VanillaBossMaps.WALLFACE },
            { KEY_BOSS_JABBERWOCKY, VanillaBossMaps.JABBER },
            { KEY_BOSS_SPRINGBEAK, VanillaBossMaps.SPRINGBEAK },
            { KEY_BOSS_GREATVIPER, VanillaBossMaps.GREATVIPER },
            { KEY_BOSS_BOREALFACE, VanillaBossMaps.BOREAL },
            { KEY_BOSS_FROSTGIGAS, VanillaBossMaps.FROSTGIGAS },
            { KEY_BOSS_MINOTAUR, VanillaBossMaps.MINOTAUR },
            { KEY_BOSS_DOOMSWALL, VanillaBossMaps.DOOMSWALL },
            { KEY_BOSS_VAMPIRE, VanillaBossMaps.VAMPIRE },
            { KEY_BOSS_METALMANTIS, VanillaBossMaps.METALMANTIS },
            { KEY_BOSS_MECHRIDER2, VanillaBossMaps.MECHRIDER2 },
            { KEY_BOSS_BLUESPIKE, VanillaBossMaps.BLUESPIKE },
            { KEY_BOSS_GORGONBULL, VanillaBossMaps.GORGON },
            { KEY_BOSS_HYDRA, VanillaBossMaps.HYDRA },
            { KEY_BOSS_KETTLEKIN, VanillaBossMaps.KETTLEKIN },
            { KEY_BOSS_SNAPDRAGON, VanillaBossMaps.SNAPDRAGON },
            { KEY_BOSS_WATERMELON, VanillaBossMaps.AEGRTOJNFSDGHOPALNIAPLEIAN },
            { KEY_BOSS_HEXAS, VanillaBossMaps.HEXAS },
            { KEY_BOSS_MECHRIDER3, VanillaBossMaps.MECHRIDER3 },
            { KEY_BOSS_DRAGONWORM, VanillaBossMaps.DRAGONWORM },
            { KEY_BOSS_SNOWDRAGON, VanillaBossMaps.SNOWDRAGON },
            { KEY_BOSS_AXEBEAK, VanillaBossMaps.AXEBEAK },
            { KEY_BOSS_REDDRAGON, VanillaBossMaps.REDDRAGON },
            { KEY_BOSS_THUNDERGIGAS, VanillaBossMaps.THUNDERGIGAS },
            { KEY_BOSS_BLUEDRAGON, VanillaBossMaps.BLUEDRAGON },
            { KEY_BOSS_BUFFY, VanillaBossMaps.BUFFY },
            { KEY_BOSS_DARKLICH, VanillaBossMaps.DARKLICH },
        };

        Dictionary<VanillaBossMap, byte> bossMapOriginalIds = new Dictionary<VanillaBossMap, byte>
        {
            { VanillaBossMaps.MANTISANT, 0x57 },
            { VanillaBossMaps.TROPICALLO, 0x59 },
            { VanillaBossMaps.SPIKEY, 0x5B },
            { VanillaBossMaps.TONPOLE, 0x71 },
            { VanillaBossMaps.FIREGIGAS, 0x74 },
            { VanillaBossMaps.KILROY, 0x65 },
            { VanillaBossMaps.WALLFACE, 0x58 },
            { VanillaBossMaps.JABBER, 0x5C },
            { VanillaBossMaps.SPRINGBEAK, 0x5D },
            { VanillaBossMaps.GREATVIPER, 0x69 },
            { VanillaBossMaps.BOREAL, 0x68 },
            { VanillaBossMaps.FROSTGIGAS, 0x5E },
            { VanillaBossMaps.MINOTAUR, 0x5A },
            { VanillaBossMaps.DOOMSWALL, 0x61 },
            { VanillaBossMaps.VAMPIRE, 0x62 },
            { VanillaBossMaps.METALMANTIS, 0x63 },
            { VanillaBossMaps.MECHRIDER2, 0x64 },
            { VanillaBossMaps.BLUESPIKE, 0x6B },
            { VanillaBossMaps.GORGON, 0x66 },
            { VanillaBossMaps.HYDRA, 0x6D },
            { VanillaBossMaps.KETTLEKIN, 0x70 },
            { VanillaBossMaps.SNAPDRAGON, 0x5F },
            { VanillaBossMaps.AEGRTOJNFSDGHOPALNIAPLEIAN, 0x6E },
            { VanillaBossMaps.HEXAS, 0x6F },
            { VanillaBossMaps.MECHRIDER3, 0x72 },
            { VanillaBossMaps.DRAGONWORM, 0x7B },
            { VanillaBossMaps.SNOWDRAGON, 0x73 },
            { VanillaBossMaps.AXEBEAK, 0x76 },
            { VanillaBossMaps.REDDRAGON, 0x75 },
            { VanillaBossMaps.THUNDERGIGAS, 0x7D },
            { VanillaBossMaps.BLUEDRAGON, 0x77 },
            { VanillaBossMaps.BUFFY, 0x78 },
            { VanillaBossMaps.DARKLICH, 0x79 },
        };

        private void entriesChanged()
        {
            PlandoSettingsChanged?.Invoke(this, null);
        }

        private void MyForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            e.Cancel = true; // this cancels the close event.
        }

        Dictionary<ToolStripMenuItem, Button> locationButtonLookup = new Dictionary<ToolStripMenuItem, Button>();

        Dictionary<ToolStripMenuItem, Button> orbButtonLookup = new Dictionary<ToolStripMenuItem, Button>();
        Dictionary<ToolStripMenuItem, Button> weaponButtonLookup = new Dictionary<ToolStripMenuItem, Button>();
        Dictionary<ToolStripMenuItem, Button> bossButtonLookup = new Dictionary<ToolStripMenuItem, Button>();
        Dictionary<ToolStripMenuItem, Button> bossElementButtonLookup = new Dictionary<ToolStripMenuItem, Button>();
        private void makeLocationSubmenu(ToolStripMenuItem category, string subLabel, Button button)
        {
            ToolStripMenuItem item = new ToolStripMenuItem(subLabel);
            category.DropDownItems.Add(item);
            locationButtonLookup[item] = button;
            item.Click += Item_Click_Location;
        }

        private void Item_Click_Location(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            string text = item.Text;
            Button locationButton = locationButtonLookup[item];
            int buttonIndex = locationButtons.IndexOf(locationButton);
            if (text == "(Remove)")
            {
                if (allPrizes.Count > buttonIndex && allPrizes[buttonIndex] == "")
                {
                    locationButton.Text = "Add";
                }
                else
                {
                    if (buttonIndex < locations)
                    {
                        locations--;
                        if (locationButtons.Count > 1)
                        {
                            panel1.Controls.Remove(locationButton);
                            locationButtons.RemoveAt(buttonIndex);
                        }
                        else
                        {
                            locationButtons[0].Text = "Add";
                        }
                        if (prizeButtons.Count > buttonIndex)
                        {
                            if (prizeButtons.Count > 1)
                            {
                                panel1.Controls.Remove(prizeButtons[buttonIndex]);
                                prizeButtons.RemoveAt(buttonIndex);
                            }
                            else
                            {
                                prizeButtons[0].Visible = false;
                                prizeButtons[0].Text = "Set";
                            }
                        }
                        // remove from allPrizes, allLocations
                        if (allPrizes.Count > buttonIndex)
                        {
                            allPrizes.RemoveAt(buttonIndex);
                        }
                        if (allLocations.Count > buttonIndex)
                        {
                            allLocations.RemoveAt(buttonIndex);
                        }
                        // move buttons below it up
                        for (int i = buttonIndex; i < prizeButtons.Count; i++)
                        {
                            prizeButtons[i].Location = new Point(prizeButtons[i].Location.X, prizeButtons[i].Location.Y - 30);
                        }
                        for (int i = buttonIndex; i < locationButtons.Count; i++)
                        {
                            locationButtons[i].Location = new Point(locationButtons[i].Location.X, locationButtons[i].Location.Y - 30);
                        }
                        // if location buttons empty, add back the default one
                    }
                }
            }
            else
            {

                if (!allLocations.Contains(text) || text.StartsWith("("))
                {
                    locationButtonLookup[item].Text = text;
                    if (allLocations[buttonIndex] == "")
                    {
                        // show right-side button
                        prizeButtons[locations].Visible = true;
                        locations++;
                    }
                    allLocations[buttonIndex] = text;
                }
            }
            entriesChanged();
        }

        Dictionary<ToolStripMenuItem, Button> prizeButtonLookup = new Dictionary<ToolStripMenuItem, Button>();
        private void makePrizeSubmenu(ToolStripMenuItem category, string subLabel, Button button)
        {
            ToolStripMenuItem item = new ToolStripMenuItem(subLabel);
            category.DropDownItems.Add(item);
            prizeButtonLookup[item] = button;
            item.Click += Item_Click_Prize;
        }

        private void Item_Click_Prize(object sender, EventArgs e)
        {
            // handle trying to set a prize via button click.
            // check whether inventory allows it based on the other buttons.
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            Dictionary<string, int> prizeLimit = new Dictionary<string, int>();
            prizeLimit[VALUE_PRIZE_ANY_CHARACTER] = 2;
            prizeLimit[VALUE_PRIZE_ANY_WEAPON] = 8;
            prizeLimit[VALUE_PRIZE_CUTTING_WEAPON] = 2;
            prizeLimit[VALUE_PRIZE_ANY_SPELLS] = 8;
            prizeLimit[VALUE_PRIZE_ANY_SEED] = 8;
            prizeLimit[VALUE_PRIZE_ANY_WEAPON_ORB] = 64;
            prizeLimit[VALUE_PRIZE_GLOVE_ORB] = 8;
            prizeLimit[VALUE_PRIZE_SWORD_ORB] = 8;
            prizeLimit[VALUE_PRIZE_AXE_ORB] = 8;
            prizeLimit[VALUE_PRIZE_SPEAR_ORB] = 8;
            prizeLimit[VALUE_PRIZE_WHIP_ORB] = 8;
            prizeLimit[VALUE_PRIZE_BOW_ORB] = 8;
            prizeLimit[VALUE_PRIZE_BOOMERANG_ORB] = 8;
            prizeLimit[VALUE_PRIZE_JAVELIN_ORB] = 8;
            prizeLimit[VALUE_PRIZE_MONEY] = 999;
            prizeLimit[VALUE_PRIZE_NOTHING] = 999;
            int thisIndex = prizeButtons.IndexOf(prizeButtonLookup[item]);
            // mapping of individual members to "any"s containing them
            Dictionary<string[], string> anyPrizeMappings = new Dictionary<string[], string>();
            // characters
            anyPrizeMappings[new string[] { VALUE_PRIZE_BOY, VALUE_PRIZE_GIRL, VALUE_PRIZE_SPRITE, VALUE_PRIZE_OTHER_CHARACTER_1, VALUE_PRIZE_OTHER_CHARACTER_2 }] = VALUE_PRIZE_ANY_CHARACTER;
            // weapons
            anyPrizeMappings[new string[] { VALUE_PRIZE_GLOVE, VALUE_PRIZE_SWORD, VALUE_PRIZE_AXE, VALUE_PRIZE_SPEAR, VALUE_PRIZE_WHIP, VALUE_PRIZE_BOW, VALUE_PRIZE_BOOMERANG, VALUE_PRIZE_JAVELIN }] = VALUE_PRIZE_ANY_WEAPON;
            // spells
            anyPrizeMappings[new string[] { VALUE_PRIZE_UNDINE, VALUE_PRIZE_GNOME, VALUE_PRIZE_SYLPHID, VALUE_PRIZE_SALAMANDO, VALUE_PRIZE_LUMINA, VALUE_PRIZE_SHADE, VALUE_PRIZE_LUNA, VALUE_PRIZE_DRYAD }] = VALUE_PRIZE_ANY_SPELLS;
            // seeds
            anyPrizeMappings[new string[] { VALUE_PRIZE_WATER_SEED, VALUE_PRIZE_EARTH_SEED, VALUE_PRIZE_WIND_SEED, VALUE_PRIZE_FIRE_SEED, VALUE_PRIZE_LIGHT_SEED, VALUE_PRIZE_DARK_SEED, VALUE_PRIZE_MOON_SEED, VALUE_PRIZE_DRYAD_SEED }] = VALUE_PRIZE_ANY_SEED;
            // weapon orbs
            anyPrizeMappings[new string[] { VALUE_PRIZE_GLOVE_ORB, VALUE_PRIZE_SWORD_ORB, VALUE_PRIZE_AXE_ORB, VALUE_PRIZE_SPEAR_ORB, VALUE_PRIZE_WHIP_ORB, VALUE_PRIZE_BOW_ORB, VALUE_PRIZE_BOOMERANG_ORB, VALUE_PRIZE_JAVELIN_ORB }] = VALUE_PRIZE_ANY_WEAPON_ORB;
            for (int i=0; i < allPrizes.Count; i++)
            {
                // note these are display text (no -), while prizeLimit is property text (with -)
                string prize = allPrizes[i];
                // we're replacing this one, so don't count what's at it now
                if (i != thisIndex)
                {
                    string propertyPrize = displayToProperty(prize, VALUE_PREFIX);
                    // multi-value "any" type property
                    if (prizeLimit.ContainsKey(propertyPrize))
                    {
                        prizeLimit[propertyPrize]--;
                        if(prizeLimit[propertyPrize] == 0)
                        {
                            foreach(string[] key in anyPrizeMappings.Keys)
                            {
                                if(anyPrizeMappings[key] == propertyPrize)
                                {
                                    // don't allow any more of this type's individual members
                                    foreach (string individual in key)
                                    {
                                        prizeLimit[individual] = 0;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        // single item
                        prizeLimit[propertyPrize] = 0;
                    }
                    foreach (string[] key in anyPrizeMappings.Keys)
                    {
                        // if item is a member of any "any" group, reduce that group's count by one
                        if (key.Contains(propertyPrize))
                        {
                            prizeLimit[anyPrizeMappings[key]] = prizeLimit[anyPrizeMappings[key]] - 1;
                            // if we ran out of "any"s, don't allow any other singles for the group either
                            if (prizeLimit[anyPrizeMappings[key]] == 0)
                            {
                                foreach (string individual in key)
                                {
                                    prizeLimit[individual] = 0;
                                }
                            }
                        }
                    }
                }
            }

            // check whether still allowed to add the item that we were trying to add
            int maxThisItem = 1;

            string itemProperty = displayToProperty(item.Text, VALUE_PREFIX);
            if (prizeLimit.ContainsKey(itemProperty))
            {
                maxThisItem = prizeLimit[itemProperty];
            }
            
            // if prizeLimit decided its remaining count was 0, don't add it
            if(maxThisItem > 0)
            {
                prizeButtonLookup[item].Text = item.Text;
                allPrizes[thisIndex] = item.Text;
                if (thisIndex == allLocations.Count - 1)
                {
                    Button lastLocButton = locationButtons[locations - 1];
                    Button newLocButton = new Button();
                    newLocButton.Location = new Point(lastLocButton.Location.X, lastLocButton.Location.Y + 30);
                    newLocButton.Text = "Add";
                    newLocButton.Click += Button1_Click;
                    newLocButton.Anchor = button1.Anchor;
                    newLocButton.AutoSize = true;
                    panel1.Controls.Add(newLocButton);
                    locationButtons.Add(newLocButton);
                    allLocations.Add("");

                    Button lastPrizeButton = prizeButtons[locations - 1];
                    Button newPrizeButton = new Button();
                    newPrizeButton.Location = new Point(lastPrizeButton.Location.X, lastPrizeButton.Location.Y + 30);
                    newPrizeButton.Text = "Set";
                    newPrizeButton.Visible = false;
                    newPrizeButton.Click += Button2_Click;
                    newPrizeButton.Anchor = button2.Anchor;
                    newPrizeButton.AutoSize = true;
                    prizeButtons.Add(newPrizeButton);
                    panel1.Controls.Add(newPrizeButton);
                    allPrizes.Add("");

                }

                entriesChanged();
            }
        }

        public string getPlandoSettingsString()
        {
            string allSettings = "";
            if(checkBox2.Checked)
            {
                // plandoSettings=...
                bool obfuscate = checkBox1.Checked;
                for(int i=0; i < allLocations.Count && i < allPrizes.Count; i++)
                {
                    string thisLoc = allLocations[i];
                    string thisPrize = allPrizes[i];
                    if(thisLoc != "" && thisPrize != "")
                    {
                        thisLoc = displayToProperty(thisLoc, KEY_PREFIX_PRIZE);
                        thisPrize = thisPrize.Replace(' ', '-');
                        allSettings += thisLoc + ":" + thisPrize + ";";
                    }
                }
                foreach(string key in orbElements.Keys)
                {
                    string orbProperty = key;
                    string orbValue = orbElements[key];
                    orbValue = displayToProperty(orbValue, VALUE_PREFIX);
                    allSettings += orbProperty + ":" + orbValue + ";";
                }
                foreach (string key in weaponSettings.Keys)
                {
                    string weaponProperty = key;
                    string weaponValue = weaponSettings[key];
                    weaponValue = displayToProperty(weaponValue, VALUE_PREFIX);
                    allSettings += weaponProperty + ":" + weaponValue + ";";
                }
                foreach (string key in bossSettings.Keys)
                {
                    string bossProperty = key;
                    string bossValue = bossSettings[key];
                    bossValue = displayToProperty(bossValue, VALUE_PREFIX);
                    allSettings += bossProperty + ":" + bossValue + ";";
                }
                foreach (string key in bossElementSettings.Keys)
                {
                    string bossElementProperty = key;
                    string bossElementValue = bossElementSettings[key];
                    bossElementValue = displayToProperty(bossElementValue, VALUE_PREFIX);
                    allSettings += bossElementProperty + ":" + bossElementValue + ";";
                }
                if (bypassValidation)
                {
                    allSettings += KEY_BYPASS_VALIDATION + ":yes;";
                }
                if (allSettings != "")
                {
                    if (obfuscate)
                    {
                        allSettings = "#" + DataUtil.Base64Encode(allSettings);
                    }
                }
            }
            return allSettings;
        }

        public void setPlandoSettingsString(string settings)
        {
            suspendUpdates = true;
            // from options mgr
            if (settings == "")
            {
                checkBox2.Checked = false;
            }
            else
            {
                // everything back to initial state first
                foreach(Button locationButton in locationButtons)
                {
                    panel1.Controls.Remove(locationButton);
                }
                locationButtons.Clear();
                foreach (Button prizeButton in prizeButtons)
                {
                    panel1.Controls.Remove(prizeButton);
                }
                prizeButtons.Clear();
                foreach (Button orbButton in orbButtonPropertyNames.Values)
                {
                    orbButton.Text = VALUE_ORBELEMENT_NO_CHANGE;
                }
                orbElements.Clear();
                foreach (Button weaponButton in weaponButtonPropertyNames.Values)
                {
                    weaponButton.Text = VALUE_WEAPON_NO_CHANGE;
                }
                weaponSettings.Clear();
                
                locationButtons.Clear();
                allLocations.Clear();
                allPrizes.Clear();
                bypassValidation = false;

                checkBox2.Checked = true;
                string plandoSettings = settings;
                checkBox1.Checked = false;
                if (plandoSettings.StartsWith("#"))
                {
                    plandoSettings = DataUtil.Base64Decode(plandoSettings.Substring(1));
                    checkBox1.Checked = true;
                }
                locations = 0;
                int baseYPosition = 21;
                // setting:value;setting:value...
                string[] individualSettings = plandoSettings.Split(new char[] { ';' });
                foreach(string individualSetting in individualSettings)
                {
                    if(individualSetting.Length > 0)
                    {
                        string[] keyValue = individualSetting.Split(new char[] { ':' });
                        if(keyValue.Length == 2)
                        {
                            string kvKey = keyValue[0];
                            string kvValue = keyValue[1];
                            if(kvKey.StartsWith(KEY_PREFIX_PRIZE))
                            {
                                string locationName = propertyToDisplay(kvKey);
                                string prizeName = propertyToDisplay(kvValue);

                                Button newLocButton = new Button();
                                newLocButton.Location = new Point(23, baseYPosition + 30 * locations);
                                newLocButton.Text = locationName;
                                newLocButton.Click += Button1_Click;
                                newLocButton.Anchor = button1.Anchor;
                                newLocButton.AutoSize = true;
                                panel1.Controls.Add(newLocButton);
                                locationButtons.Add(newLocButton);
                                allLocations.Add(locationName);

                                Button newPrizeButton = new Button();
                                newPrizeButton.Location = new Point(264, baseYPosition + 30 * locations);
                                newPrizeButton.Text = prizeName;
                                newPrizeButton.Visible = true;
                                newPrizeButton.Click += Button2_Click;
                                newPrizeButton.Anchor = button2.Anchor;
                                newPrizeButton.AutoSize = true;
                                prizeButtons.Add(newPrizeButton);
                                panel1.Controls.Add(newPrizeButton);
                                allPrizes.Add(prizeName);

                                locations++;
                            }
                            else if(kvKey.StartsWith(KEY_PREFIX_NONPRIZE))
                            {
                                string keyName = propertyToDisplay(kvKey);
                                string valueName = propertyToDisplay(kvValue);
                                if (orbButtonPropertyNames.ContainsKey(keyName))
                                {
                                    orbButtonPropertyNames[keyName].Text = valueName;
                                    orbElements[keyName] = valueName;
                                }
                                if (weaponButtonPropertyNames.ContainsKey(keyName))
                                {
                                    weaponButtonPropertyNames[keyName].Text = valueName;
                                    weaponSettings[keyName] = valueName;
                                }
                                if(bossButtonPropertyNames.ContainsKey(keyName))
                                {
                                    bossButtonPropertyNames[keyName].Text = valueName;
                                    bossSettings[keyName] = valueName;
                                }
                                if (bossElementButtonPropertyNames.ContainsKey(keyName))
                                {
                                    bossElementButtonPropertyNames[keyName].Text = valueName;
                                    bossElementSettings[keyName] = valueName;
                                }
                                if (keyName == KEY_BYPASS_VALIDATION)
                                {
                                    bypassValidation = valueName == "yes";
                                    checkBox3.Checked = bypassValidation;
                                }
                            }
                            else
                            {
                                suspendUpdates = false;
                                throw new OptionsException(true, "Unrecognized plando key: " + kvKey);
                            }
                        }
                    }
                }
                Button addLocButton = new Button();
                addLocButton.Location = new Point(23, baseYPosition + 30 * locations);
                addLocButton.Text = "Add";
                addLocButton.Click += Button1_Click;
                addLocButton.Anchor = button1.Anchor;
                addLocButton.AutoSize = true;
                panel1.Controls.Add(addLocButton);
                locationButtons.Add(addLocButton);
                allLocations.Add("");

                Button addPrizeButton = new Button();
                addPrizeButton.Location = new Point(264, baseYPosition + 30 * locations);
                addPrizeButton.Text = "Set";
                addPrizeButton.Visible = false;
                addPrizeButton.Click += Button2_Click;
                addPrizeButton.Anchor = button2.Anchor;
                addPrizeButton.AutoSize = true;
                prizeButtons.Add(addPrizeButton);
                panel1.Controls.Add(addPrizeButton);
                allPrizes.Add("");
            }


            suspendUpdates = false;
            entriesChanged();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            // location
            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();

            Button button = (Button)sender;

            ToolStripMenuItem pandoraCategory = new ToolStripMenuItem("Pandora Region");
            makeLocationSubmenu(pandoraCategory, propertyToDisplay(KEY_LOCATION_POTOS_CHEST), button);
            makeLocationSubmenu(pandoraCategory, propertyToDisplay(KEY_LOCATION_MANTIS_ANT), button);
            makeLocationSubmenu(pandoraCategory, propertyToDisplay(KEY_LOCATION_PANDORA_GIRL), button);
            makeLocationSubmenu(pandoraCategory, propertyToDisplay(KEY_LOCATION_PANDORA_CHEST_1), button);
            makeLocationSubmenu(pandoraCategory, propertyToDisplay(KEY_LOCATION_PANDORA_CHEST_2), button);
            makeLocationSubmenu(pandoraCategory, propertyToDisplay(KEY_LOCATION_PANDORA_CHEST_3), button);
            makeLocationSubmenu(pandoraCategory, propertyToDisplay(KEY_LOCATION_PANDORA_CHEST_4), button);
            makeLocationSubmenu(pandoraCategory, propertyToDisplay(KEY_LOCATION_PANDORA_CHEST_5), button);
            makeLocationSubmenu(pandoraCategory, propertyToDisplay(KEY_LOCATION_PANDORA_CHEST_6), button);
            makeLocationSubmenu(pandoraCategory, propertyToDisplay(KEY_LOCATION_LUKA_1), button);
            makeLocationSubmenu(pandoraCategory, propertyToDisplay(KEY_LOCATION_LUKA_2), button);
            makeLocationSubmenu(pandoraCategory, propertyToDisplay(KEY_LOCATION_UNDINE_1), button);
            makeLocationSubmenu(pandoraCategory, propertyToDisplay(KEY_LOCATION_UNDINE_2), button);
            makeLocationSubmenu(pandoraCategory, propertyToDisplay(KEY_LOCATION_ELINEE_CHEST_1), button);
            makeLocationSubmenu(pandoraCategory, propertyToDisplay(KEY_LOCATION_ELINEE_CHEST_2), button);
            makeLocationSubmenu(pandoraCategory, propertyToDisplay(KEY_LOCATION_SWORD_PEDESTAL), button);
            makeLocationSubmenu(pandoraCategory, propertyToDisplay(KEY_LOCATION_ANY_PANDORA), button);

            ToolStripMenuItem navelCategory = new ToolStripMenuItem("Gaia's Navel");
            makeLocationSubmenu(navelCategory, propertyToDisplay(KEY_LOCATION_MAGIC_ROPE_CHEST), button);
            makeLocationSubmenu(navelCategory, propertyToDisplay(KEY_LOCATION_WATTS), button);
            makeLocationSubmenu(navelCategory, propertyToDisplay(KEY_LOCATION_DWARF_ELDER), button);
            makeLocationSubmenu(navelCategory, propertyToDisplay(KEY_LOCATION_TROPICALLO_1), button);
            makeLocationSubmenu(navelCategory, propertyToDisplay(KEY_LOCATION_TROPICALLO_2), button);
            makeLocationSubmenu(navelCategory, propertyToDisplay(KEY_LOCATION_GNOME_1), button);
            makeLocationSubmenu(navelCategory, propertyToDisplay(KEY_LOCATION_GNOME_2), button);
            makeLocationSubmenu(navelCategory, propertyToDisplay(KEY_LOCATION_KILROY), button);
            makeLocationSubmenu(navelCategory, propertyToDisplay(KEY_LOCATION_KILROY_CHEST), button);
            makeLocationSubmenu(navelCategory, propertyToDisplay(KEY_LOCATION_ANY_GAIAS_NAVEL), button);

            ToolStripMenuItem upperLandCategory = new ToolStripMenuItem("Upper Land");
            makeLocationSubmenu(upperLandCategory, propertyToDisplay(KEY_LOCATION_MOOGLE_TOWN_CHEST_1), button);
            makeLocationSubmenu(upperLandCategory, propertyToDisplay(KEY_LOCATION_MOOGLE_TOWN_CHEST_2), button);
            makeLocationSubmenu(upperLandCategory, propertyToDisplay(KEY_LOCATION_SYLPHID_1), button);
            makeLocationSubmenu(upperLandCategory, propertyToDisplay(KEY_LOCATION_SYLPHID_2), button);
            makeLocationSubmenu(upperLandCategory, propertyToDisplay(KEY_LOCATION_MATANGO_INN_CHEST), button);
            makeLocationSubmenu(upperLandCategory, propertyToDisplay(KEY_LOCATION_MATANGO_CAVE_FLAMMIE), button);
            makeLocationSubmenu(upperLandCategory, propertyToDisplay(KEY_LOCATION_ANY_UPPER_LAND), button);

            ToolStripMenuItem empireCategory = new ToolStripMenuItem("Empire");
            makeLocationSubmenu(empireCategory, propertyToDisplay(KEY_LOCATION_SOUTHTOWN_MARA), button);
            makeLocationSubmenu(empireCategory, propertyToDisplay(KEY_LOCATION_NTR_WEST_CHEST), button);
            makeLocationSubmenu(empireCategory, propertyToDisplay(KEY_LOCATION_NTR_EAST_CHEST), button);
            makeLocationSubmenu(empireCategory, propertyToDisplay(KEY_LOCATION_NTR_INNER_CHEST), button);
            makeLocationSubmenu(empireCategory, propertyToDisplay(KEY_LOCATION_NTR_WALL), button);
            makeLocationSubmenu(empireCategory, propertyToDisplay(KEY_LOCATION_NTR_VAMPIRE), button);
            makeLocationSubmenu(empireCategory, propertyToDisplay(KEY_LOCATION_NTC_EAST_CHEST_1), button);
            makeLocationSubmenu(empireCategory, propertyToDisplay(KEY_LOCATION_NTC_EAST_CHEST_2), button);
            makeLocationSubmenu(empireCategory, propertyToDisplay(KEY_LOCATION_NTC_METAL_MANTIS), button);
            makeLocationSubmenu(empireCategory, propertyToDisplay(KEY_LOCATION_NTC_INNER_CHEST), button);
            makeLocationSubmenu(empireCategory, propertyToDisplay(KEY_LOCATION_NTC_MECH_RIDER), button);
            makeLocationSubmenu(empireCategory, propertyToDisplay(KEY_LOCATION_GOLD_TOWER_CHEST_1), button);
            makeLocationSubmenu(empireCategory, propertyToDisplay(KEY_LOCATION_GOLD_TOWER_CHEST_2), button);
            makeLocationSubmenu(empireCategory, propertyToDisplay(KEY_LOCATION_LUMINA_1), button);
            makeLocationSubmenu(empireCategory, propertyToDisplay(KEY_LOCATION_LUMINA_2), button);
            makeLocationSubmenu(empireCategory, propertyToDisplay(KEY_LOCATION_ANY_EMPIRE), button);

            ToolStripMenuItem iceCountryCategory = new ToolStripMenuItem("Ice Country");
            makeLocationSubmenu(iceCountryCategory, propertyToDisplay(KEY_LOCATION_SALAMANDO_STOVE), button);
            makeLocationSubmenu(iceCountryCategory, propertyToDisplay(KEY_LOCATION_TRIPLE_TONPOLE), button);
            makeLocationSubmenu(iceCountryCategory, propertyToDisplay(KEY_LOCATION_ICE_CASTLE_CHEST), button);
            makeLocationSubmenu(iceCountryCategory, propertyToDisplay(KEY_LOCATION_FROST_GIGAS), button);
            makeLocationSubmenu(iceCountryCategory, propertyToDisplay(KEY_LOCATION_ANY_ICE_COUNTRY), button);

            ToolStripMenuItem desertCategory = new ToolStripMenuItem("Kakkara");
            makeLocationSubmenu(desertCategory, propertyToDisplay(KEY_LOCATION_SEA_HARE_TAIL_GIFT), button);
            makeLocationSubmenu(desertCategory, propertyToDisplay(KEY_LOCATION_FIRE_PALACE_CHEST_1), button);
            makeLocationSubmenu(desertCategory, propertyToDisplay(KEY_LOCATION_FIRE_PALACE_CHEST_2), button);
            makeLocationSubmenu(desertCategory, propertyToDisplay(KEY_LOCATION_FIRE_PALACE_CHEST_3), button);
            makeLocationSubmenu(desertCategory, propertyToDisplay(KEY_LOCATION_FIRE_PALACE_SALAMANDO), button);
            makeLocationSubmenu(desertCategory, propertyToDisplay(KEY_LOCATION_LUNA_1), button);
            makeLocationSubmenu(desertCategory, propertyToDisplay(KEY_LOCATION_LUNA_2), button);
            makeLocationSubmenu(desertCategory, propertyToDisplay(KEY_LOCATION_ANY_DESERT), button);

            ToolStripMenuItem mountainsCategory = new ToolStripMenuItem("Mountains");
            makeLocationSubmenu(mountainsCategory, propertyToDisplay(KEY_LOCATION_POD_GATED_CHEST), button);
            makeLocationSubmenu(mountainsCategory, propertyToDisplay(KEY_LOCATION_POD_HALLWAY_CHEST), button);
            makeLocationSubmenu(mountainsCategory, propertyToDisplay(KEY_LOCATION_SHADE_1), button);
            makeLocationSubmenu(mountainsCategory, propertyToDisplay(KEY_LOCATION_SHADE_2), button);
            makeLocationSubmenu(mountainsCategory, propertyToDisplay(KEY_LOCATION_DOPPEL), button);
            makeLocationSubmenu(mountainsCategory, propertyToDisplay(KEY_LOCATION_ANY_MOUNTAINS), button);

            ToolStripMenuItem lostContinentCategory = new ToolStripMenuItem("Lost Continent");
            makeLocationSubmenu(lostContinentCategory, propertyToDisplay(KEY_LOCATION_WATERMELON), button);
            makeLocationSubmenu(lostContinentCategory, propertyToDisplay(KEY_LOCATION_DRYAD_1), button);
            makeLocationSubmenu(lostContinentCategory, propertyToDisplay(KEY_LOCATION_DRYAD_2), button);
            makeLocationSubmenu(lostContinentCategory, propertyToDisplay(KEY_LOCATION_HEXAS), button);
            makeLocationSubmenu(lostContinentCategory, propertyToDisplay(KEY_LOCATION_SPELL_ORB_AREA_CHEST), button);
            makeLocationSubmenu(lostContinentCategory, propertyToDisplay(KEY_LOCATION_UNDERSEA_AREA_CHEST), button);
            makeLocationSubmenu(lostContinentCategory, propertyToDisplay(KEY_LOCATION_HYDRA), button);
            makeLocationSubmenu(lostContinentCategory, propertyToDisplay(KEY_LOCATION_KETTLE_KIN), button);
            makeLocationSubmenu(lostContinentCategory, propertyToDisplay(KEY_LOCATION_MECH_RIDER_3), button);
            makeLocationSubmenu(lostContinentCategory, propertyToDisplay(KEY_LOCATION_ANY_LOST_CONTINENT), button);

            ToolStripMenuItem pureLandsCategory = new ToolStripMenuItem("Pure Lands");
            makeLocationSubmenu(pureLandsCategory, propertyToDisplay(KEY_LOCATION_DRAGON_WORM), button);
            makeLocationSubmenu(pureLandsCategory, propertyToDisplay(KEY_LOCATION_SNOW_DRAGON), button);
            makeLocationSubmenu(pureLandsCategory, propertyToDisplay(KEY_LOCATION_AXE_BEAK), button);
            makeLocationSubmenu(pureLandsCategory, propertyToDisplay(KEY_LOCATION_RED_DRAGON), button);
            makeLocationSubmenu(pureLandsCategory, propertyToDisplay(KEY_LOCATION_THUNDER_GIGAS), button);
            makeLocationSubmenu(pureLandsCategory, propertyToDisplay(KEY_LOCATION_BLUE_DRAGON), button);
            makeLocationSubmenu(pureLandsCategory, propertyToDisplay(KEY_LOCATION_MANA_TREE), button);
            makeLocationSubmenu(pureLandsCategory, propertyToDisplay(KEY_LOCATION_ANY_PURE_LANDS), button);

            ToolStripMenuItem miscCategory = new ToolStripMenuItem("Misc");
            makeLocationSubmenu(miscCategory, propertyToDisplay(KEY_LOCATION_TURTLE_ISLAND), button);
            makeLocationSubmenu(miscCategory, propertyToDisplay(KEY_LOCATION_LIGHTHOUSE), button);
            makeLocationSubmenu(miscCategory, propertyToDisplay(KEY_LOCATION_TASNICA_MINIBOSS), button);
            makeLocationSubmenu(miscCategory, propertyToDisplay(KEY_LOCATION_BUFFY), button);
            makeLocationSubmenu(miscCategory, propertyToDisplay(KEY_LOCATION_DREAD_SLIME), button);
            makeLocationSubmenu(miscCategory, propertyToDisplay(KEY_LOCATION_ANY_MANA_SEED_PEDESTAL), button);
            makeLocationSubmenu(miscCategory, propertyToDisplay(KEY_LOCATION_ANY_BOSS), button);

            ToolStripMenuItem startWithCategory = new ToolStripMenuItem(propertyToDisplay(KEY_LOCATION_START_WITH));
            startWithCategory.Click += Item_Click_Location;
            locationButtonLookup[startWithCategory] = button;

            ToolStripMenuItem nonExistentCategory = new ToolStripMenuItem(propertyToDisplay(KEY_LOCATION_NON_EXISTENT));
            nonExistentCategory.Click += Item_Click_Location;
            locationButtonLookup[nonExistentCategory] = button;

            ToolStripMenuItem removeCategory = new ToolStripMenuItem("(Remove)");
            removeCategory.Click += Item_Click_Location;
            locationButtonLookup[removeCategory] = button;

            contextMenuStrip.Items.Add(pandoraCategory);
            contextMenuStrip.Items.Add(navelCategory);
            contextMenuStrip.Items.Add(upperLandCategory);
            contextMenuStrip.Items.Add(empireCategory);
            contextMenuStrip.Items.Add(iceCountryCategory);
            contextMenuStrip.Items.Add(desertCategory);
            contextMenuStrip.Items.Add(mountainsCategory);
            contextMenuStrip.Items.Add(lostContinentCategory);
            contextMenuStrip.Items.Add(pureLandsCategory);
            contextMenuStrip.Items.Add(miscCategory);
            contextMenuStrip.Items.Add(startWithCategory);
            contextMenuStrip.Items.Add(nonExistentCategory);
            contextMenuStrip.Items.Add(removeCategory);

            Point ptLowerLeft = new Point(0, button.Height);
            ptLowerLeft = button.PointToScreen(ptLowerLeft);
            contextMenuStrip.Show(ptLowerLeft);
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            // prize
            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
            Button button = (Button)sender;
            // filter some stuff out for "start with" .. OpenWorldStuffRandomizer only handles some stuff currently
            bool filterForStartsWith = false;
            bool filterForChest = false;
            for (int i=0; i < prizeButtons.Count; i++)
            {
                if(prizeButtons[i] == button)
                {
                    if(allLocations.Count > i)
                    {
                        if(allLocations[i] == "(Start with)")
                        {
                            filterForStartsWith = true;
                        }
                        if (allLocations[i].Contains("Chest"))
                        {
                            filterForChest = true;
                        }
                    }
                }
            }

            if (!filterForStartsWith) // separate option for this
            {
                ToolStripMenuItem charactersCategory = new ToolStripMenuItem("Characters");
                makePrizeSubmenu(charactersCategory, propertyToDisplay(VALUE_PRIZE_BOY), button);
                makePrizeSubmenu(charactersCategory, propertyToDisplay(VALUE_PRIZE_GIRL), button);
                makePrizeSubmenu(charactersCategory, propertyToDisplay(VALUE_PRIZE_SPRITE), button);
                makePrizeSubmenu(charactersCategory, propertyToDisplay(VALUE_PRIZE_OTHER_CHARACTER_1), button);
                makePrizeSubmenu(charactersCategory, propertyToDisplay(VALUE_PRIZE_OTHER_CHARACTER_2), button);
                makePrizeSubmenu(charactersCategory, propertyToDisplay(VALUE_PRIZE_ANY_CHARACTER), button);
                contextMenuStrip.Items.Add(charactersCategory);
            }

            ToolStripMenuItem weaponsCategory = new ToolStripMenuItem("Weapons");
            makePrizeSubmenu(weaponsCategory, propertyToDisplay(VALUE_PRIZE_GLOVE), button);
            makePrizeSubmenu(weaponsCategory, propertyToDisplay(VALUE_PRIZE_SWORD), button);
            makePrizeSubmenu(weaponsCategory, propertyToDisplay(VALUE_PRIZE_AXE), button);
            makePrizeSubmenu(weaponsCategory, propertyToDisplay(VALUE_PRIZE_SPEAR), button);
            makePrizeSubmenu(weaponsCategory, propertyToDisplay(VALUE_PRIZE_WHIP), button);
            makePrizeSubmenu(weaponsCategory, propertyToDisplay(VALUE_PRIZE_BOW), button);
            makePrizeSubmenu(weaponsCategory, propertyToDisplay(VALUE_PRIZE_BOOMERANG), button);
            makePrizeSubmenu(weaponsCategory, propertyToDisplay(VALUE_PRIZE_JAVELIN), button);
            makePrizeSubmenu(weaponsCategory, propertyToDisplay(VALUE_PRIZE_ANY_WEAPON), button);
            makePrizeSubmenu(weaponsCategory, propertyToDisplay(VALUE_PRIZE_CUTTING_WEAPON), button);
            contextMenuStrip.Items.Add(weaponsCategory);

            ToolStripMenuItem spellsCategory = new ToolStripMenuItem("Spells");
            makePrizeSubmenu(spellsCategory, propertyToDisplay(VALUE_PRIZE_UNDINE), button);
            makePrizeSubmenu(spellsCategory, propertyToDisplay(VALUE_PRIZE_GNOME), button);
            makePrizeSubmenu(spellsCategory, propertyToDisplay(VALUE_PRIZE_SYLPHID), button);
            makePrizeSubmenu(spellsCategory, propertyToDisplay(VALUE_PRIZE_SALAMANDO), button);
            makePrizeSubmenu(spellsCategory, propertyToDisplay(VALUE_PRIZE_LUMINA), button);
            makePrizeSubmenu(spellsCategory, propertyToDisplay(VALUE_PRIZE_SHADE), button);
            makePrizeSubmenu(spellsCategory, propertyToDisplay(VALUE_PRIZE_LUNA), button);
            makePrizeSubmenu(spellsCategory, propertyToDisplay(VALUE_PRIZE_DRYAD), button);
            makePrizeSubmenu(spellsCategory, propertyToDisplay(VALUE_PRIZE_ANY_SPELLS), button);
            contextMenuStrip.Items.Add(spellsCategory);

            ToolStripMenuItem seedsCategory = new ToolStripMenuItem("Seeds");
            makePrizeSubmenu(seedsCategory, propertyToDisplay(VALUE_PRIZE_WATER_SEED), button);
            makePrizeSubmenu(seedsCategory, propertyToDisplay(VALUE_PRIZE_EARTH_SEED), button);
            makePrizeSubmenu(seedsCategory, propertyToDisplay(VALUE_PRIZE_WIND_SEED), button);
            makePrizeSubmenu(seedsCategory, propertyToDisplay(VALUE_PRIZE_FIRE_SEED), button);
            makePrizeSubmenu(seedsCategory, propertyToDisplay(VALUE_PRIZE_LIGHT_SEED), button);
            makePrizeSubmenu(seedsCategory, propertyToDisplay(VALUE_PRIZE_DARK_SEED), button);
            makePrizeSubmenu(seedsCategory, propertyToDisplay(VALUE_PRIZE_MOON_SEED), button);
            makePrizeSubmenu(seedsCategory, propertyToDisplay(VALUE_PRIZE_DRYAD_SEED), button);
            makePrizeSubmenu(seedsCategory, propertyToDisplay(VALUE_PRIZE_ANY_SEED), button);
            contextMenuStrip.Items.Add(seedsCategory);

            ToolStripMenuItem orbsCategory = new ToolStripMenuItem("Weapon orbs");
            makePrizeSubmenu(orbsCategory, propertyToDisplay(VALUE_PRIZE_GLOVE_ORB), button);
            makePrizeSubmenu(orbsCategory, propertyToDisplay(VALUE_PRIZE_SWORD_ORB), button);
            makePrizeSubmenu(orbsCategory, propertyToDisplay(VALUE_PRIZE_AXE_ORB), button);
            makePrizeSubmenu(orbsCategory, propertyToDisplay(VALUE_PRIZE_SPEAR_ORB), button);
            makePrizeSubmenu(orbsCategory, propertyToDisplay(VALUE_PRIZE_WHIP_ORB), button);
            makePrizeSubmenu(orbsCategory, propertyToDisplay(VALUE_PRIZE_BOW_ORB), button);
            makePrizeSubmenu(orbsCategory, propertyToDisplay(VALUE_PRIZE_BOOMERANG_ORB), button);
            makePrizeSubmenu(orbsCategory, propertyToDisplay(VALUE_PRIZE_JAVELIN_ORB), button);
            makePrizeSubmenu(orbsCategory, propertyToDisplay(VALUE_PRIZE_ANY_WEAPON_ORB), button);
            contextMenuStrip.Items.Add(orbsCategory);

            ToolStripMenuItem miscCategory = new ToolStripMenuItem("Misc");
            makePrizeSubmenu(miscCategory, propertyToDisplay(VALUE_PRIZE_MOOGLE_BELT), button);
            makePrizeSubmenu(miscCategory, propertyToDisplay(VALUE_PRIZE_MIDGE_MALLET), button);
            makePrizeSubmenu(miscCategory, propertyToDisplay(VALUE_PRIZE_SEA_HARE_TAIL), button);
            makePrizeSubmenu(miscCategory, propertyToDisplay(VALUE_PRIZE_GOLD_KEY), button);
            if (!filterForStartsWith)
            {
                makePrizeSubmenu(miscCategory, propertyToDisplay(VALUE_PRIZE_FLAMMIE_DRUM), button); // starts with flammie drum is just flammie drum not in logic
                makePrizeSubmenu(miscCategory, propertyToDisplay(VALUE_PRIZE_MONEY), button); // separate option for this
                if (!filterForChest) // chests can't have "nothing" since it has no event flag to make them disappear after opening
                {
                    makePrizeSubmenu(miscCategory, propertyToDisplay(VALUE_PRIZE_NOTHING), button); // we always start with nothing.
                }
            }
            contextMenuStrip.Items.Add(miscCategory);

            Point ptLowerLeft = new Point(0, button.Height);
            ptLowerLeft = button.PointToScreen(ptLowerLeft);
            contextMenuStrip.Show(ptLowerLeft);

        }

        private void Button3_Click(object sender, EventArgs e)
        {
            // orb element
            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
            Button button = (Button)sender;

            ToolStripMenuItem noChangeItem = new ToolStripMenuItem(VALUE_ORBELEMENT_NO_CHANGE);
            ToolStripMenuItem noneItem = new ToolStripMenuItem(VALUE_ORBELEMENT_NONE);
            ToolStripMenuItem undineItem = new ToolStripMenuItem(VALUE_ORBELEMENT_UNDINE);
            ToolStripMenuItem gnomeItem = new ToolStripMenuItem(VALUE_ORBELEMENT_GNOME);
            ToolStripMenuItem sylphidItem = new ToolStripMenuItem(VALUE_ORBELEMENT_SYLPHID);
            ToolStripMenuItem salaItem = new ToolStripMenuItem(VALUE_ORBELEMENT_SALAMANDO);
            ToolStripMenuItem shadeItem = new ToolStripMenuItem(VALUE_ORBELEMENT_SHADE);
            ToolStripMenuItem luminaItem = new ToolStripMenuItem(VALUE_ORBELEMENT_LUMINA);
            ToolStripMenuItem lunaItem = new ToolStripMenuItem(VALUE_ORBELEMENT_LUNA);
            ToolStripMenuItem dryadItem = new ToolStripMenuItem(VALUE_ORBELEMENT_DRYAD);

            contextMenuStrip.Items.Add(noChangeItem);
            contextMenuStrip.Items.Add(noneItem);
            contextMenuStrip.Items.Add(undineItem);
            contextMenuStrip.Items.Add(gnomeItem);
            contextMenuStrip.Items.Add(sylphidItem);
            contextMenuStrip.Items.Add(salaItem);
            contextMenuStrip.Items.Add(shadeItem);
            contextMenuStrip.Items.Add(luminaItem);
            contextMenuStrip.Items.Add(lunaItem);
            contextMenuStrip.Items.Add(dryadItem);

            noChangeItem.Click += Item_Click_OrbElement;
            noneItem.Click += Item_Click_OrbElement;
            undineItem.Click += Item_Click_OrbElement;
            gnomeItem.Click += Item_Click_OrbElement;
            sylphidItem.Click += Item_Click_OrbElement;
            salaItem.Click += Item_Click_OrbElement;
            shadeItem.Click += Item_Click_OrbElement;
            luminaItem.Click += Item_Click_OrbElement;
            lunaItem.Click += Item_Click_OrbElement;
            dryadItem.Click += Item_Click_OrbElement;

            orbButtonLookup[noChangeItem] = button;
            orbButtonLookup[noneItem] = button;
            orbButtonLookup[undineItem] = button;
            orbButtonLookup[gnomeItem] = button;
            orbButtonLookup[sylphidItem] = button;
            orbButtonLookup[salaItem] = button;
            orbButtonLookup[shadeItem] = button;
            orbButtonLookup[luminaItem] = button;
            orbButtonLookup[lunaItem] = button;
            orbButtonLookup[dryadItem] = button;

            Point ptLowerLeft = new Point(0, button.Height);
            ptLowerLeft = button.PointToScreen(ptLowerLeft);
            contextMenuStrip.Show(ptLowerLeft);

        }

        private void BossElementButton_Click(object sender, EventArgs e)
        {
            // boss element
            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
            Button button = (Button)sender;

            // randomize or don't randomize as-is
            ToolStripMenuItem noChangeItem = new ToolStripMenuItem(VALUE_BOSSELEMENT_NO_CHANGE);
            // force vanilla
            ToolStripMenuItem defaultItem = new ToolStripMenuItem(VALUE_BOSSELEMENT_DEFAULT);
            // force elementless
            ToolStripMenuItem noneItem = new ToolStripMenuItem(VALUE_BOSSELEMENT_ELEMENTLESS);
            ToolStripMenuItem undineItem = new ToolStripMenuItem(VALUE_BOSSELEMENT_UNDINE);
            ToolStripMenuItem gnomeItem = new ToolStripMenuItem(VALUE_BOSSELEMENT_GNOME);
            ToolStripMenuItem sylphidItem = new ToolStripMenuItem(VALUE_BOSSELEMENT_SYLPHID);
            ToolStripMenuItem salaItem = new ToolStripMenuItem(VALUE_BOSSELEMENT_SALAMANDO);
            ToolStripMenuItem shadeItem = new ToolStripMenuItem(VALUE_BOSSELEMENT_SHADE);
            ToolStripMenuItem luminaItem = new ToolStripMenuItem(VALUE_BOSSELEMENT_LUMINA);
            ToolStripMenuItem lunaItem = new ToolStripMenuItem(VALUE_BOSSELEMENT_LUNA);
            ToolStripMenuItem dryadItem = new ToolStripMenuItem(VALUE_BOSSELEMENT_DRYAD);

            contextMenuStrip.Items.Add(noChangeItem);
            contextMenuStrip.Items.Add(defaultItem);
            contextMenuStrip.Items.Add(noneItem);
            contextMenuStrip.Items.Add(undineItem);
            contextMenuStrip.Items.Add(gnomeItem);
            contextMenuStrip.Items.Add(sylphidItem);
            contextMenuStrip.Items.Add(salaItem);
            contextMenuStrip.Items.Add(shadeItem);
            contextMenuStrip.Items.Add(luminaItem);
            contextMenuStrip.Items.Add(lunaItem);
            contextMenuStrip.Items.Add(dryadItem);

            noChangeItem.Click += Item_Click_BossElement;
            noneItem.Click += Item_Click_BossElement;
            undineItem.Click += Item_Click_BossElement;
            gnomeItem.Click += Item_Click_BossElement;
            sylphidItem.Click += Item_Click_BossElement;
            salaItem.Click += Item_Click_BossElement;
            shadeItem.Click += Item_Click_BossElement;
            luminaItem.Click += Item_Click_BossElement;
            lunaItem.Click += Item_Click_BossElement;
            dryadItem.Click += Item_Click_BossElement;

            bossElementButtonLookup[noChangeItem] = button;
            bossElementButtonLookup[defaultItem] = button;
            bossElementButtonLookup[noneItem] = button;
            bossElementButtonLookup[undineItem] = button;
            bossElementButtonLookup[gnomeItem] = button;
            bossElementButtonLookup[sylphidItem] = button;
            bossElementButtonLookup[salaItem] = button;
            bossElementButtonLookup[shadeItem] = button;
            bossElementButtonLookup[luminaItem] = button;
            bossElementButtonLookup[lunaItem] = button;
            bossElementButtonLookup[dryadItem] = button;

            Point ptLowerLeft = new Point(0, button.Height);
            ptLowerLeft = button.PointToScreen(ptLowerLeft);
            contextMenuStrip.Show(ptLowerLeft);

        }

        private void Item_Click_BossElement(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            Button button = bossElementButtonLookup[item];
            button.Text = item.Text;
            if (item.Text == VALUE_BOSSELEMENT_NO_CHANGE)
            {
                bossElementSettings.Remove(bossElementButtonPropertyNamesInv[button]);
            }
            else
            {
                bossElementSettings[bossElementButtonPropertyNamesInv[button]] = item.Text;
            }
            entriesChanged();
        }

        private void Item_Click_OrbElement(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            Button button = orbButtonLookup[item];
            button.Text = item.Text;
            if(item.Text == VALUE_ORBELEMENT_NO_CHANGE)
            {
                orbElements.Remove(orbButtonPropertyNamesInv[button]);
            }
            else
            {
                orbElements[orbButtonPropertyNamesInv[button]] = item.Text;
            }
            entriesChanged();
        }

        private void OopsAllBossButton_Click(object sender, EventArgs e)
        {
            // boss element
            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
            Button button = (Button)sender;

            foreach (string bossName in allBossNameValues)
            {
                bool valid = true;
                if (singleInstanceBosses.Contains(bossName))
                {
                    valid = false;
                }
                if (mapRestrictedBosses.Contains(bossName))
                {
                    valid = false;
                }
                foreach(VanillaBossMap map in bossmaps.Values)
                {
                    // doom's wall is REALLY restrictive atm, so special-case it
                    if (map != VanillaBossMaps.DOOMSWALL)
                    {
                        if (BOSS_IDS_BY_PLANDO_VALUE.ContainsKey(bossName))
                        {
                            if (!map.allSupportedBosses.Contains(BOSS_IDS_BY_PLANDO_VALUE[bossName]) && bossMapOriginalIds[map] != BOSS_IDS_BY_PLANDO_VALUE[bossName])
                            {
                                valid = false;
                            }
                        }
                    }
                }
                if (valid)
                {
                    string bossDisplayName = propertyToDisplay(bossName);
                    ToolStripMenuItem bossItem = new ToolStripMenuItem(bossDisplayName);
                    contextMenuStrip.Items.Add(bossItem);
                    bossItem.Click += Item_Click_OopsAllBoss;
                }
            }

            Point ptLowerLeft = new Point(0, button.Height);
            ptLowerLeft = button.PointToScreen(ptLowerLeft);
            contextMenuStrip.Show(ptLowerLeft);

        }

        private void BossButton_Click(object sender, EventArgs e)
        {
            // boss element
            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
            Button button = (Button)sender;

            ToolStripMenuItem _bossItem = new ToolStripMenuItem("(No change)");
            contextMenuStrip.Items.Add(_bossItem);
            _bossItem.Click += Item_Click_Boss;
            bossButtonLookup[_bossItem] = button;
            string bossPropertyName = bossButtonPropertyNamesInv[button];

            foreach (string bossName in allBossNameValues)
            {
                bool valid = true;
                if(singleInstanceBosses.Contains(bossName) && bossSettings.ContainsValue(bossName))
                {
                    valid = false;
                }
                if(!mapRestrictedLocations.Contains(bossButtonPropertyNamesInv[button]) && mapRestrictedBosses.Contains(bossName))
                {
                    valid = false;
                }
                if (bossmaps.ContainsKey(bossPropertyName))
                {
                    if (BOSS_IDS_BY_PLANDO_VALUE.ContainsKey(bossName))
                    {
                        VanillaBossMap map = bossmaps[bossPropertyName];
                        if (!map.allSupportedBosses.Contains(BOSS_IDS_BY_PLANDO_VALUE[bossName]) && bossMapOriginalIds[map] != BOSS_IDS_BY_PLANDO_VALUE[bossName])
                        {
                            valid = false;
                        }
                    }
                }
                if (valid)
                {
                    string bossDisplayName = propertyToDisplay(bossName);
                    ToolStripMenuItem bossItem = new ToolStripMenuItem(bossName);
                    contextMenuStrip.Items.Add(bossItem);
                    bossItem.Click += Item_Click_Boss;
                    bossButtonLookup[bossItem] = button;
                }
            }
           
            Point ptLowerLeft = new Point(0, button.Height);
            ptLowerLeft = button.PointToScreen(ptLowerLeft);
            contextMenuStrip.Show(ptLowerLeft);

        }

        private void Item_Click_OopsAllBoss(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            foreach (Button button in bossButtonPropertyNames.Values)
            {
                if (bossButtonPropertyNamesInv[button] != KEY_BOSS_DOOMSWALL)
                {
                    button.Text = item.Text;
                    bossSettings[bossButtonPropertyNamesInv[button]] = item.Text;
                }
            }
            entriesChanged();
        }

        private void Item_Click_Boss(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            Button button = bossButtonLookup[item];
            button.Text = item.Text;
            if (item.Text == "(No change)")
            {
                bossSettings.Remove(bossButtonPropertyNamesInv[button]);
            }
            else
            {
                bossSettings[bossButtonPropertyNamesInv[button]] = item.Text;
            }
            entriesChanged();
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            // weapons
            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
            Button button = (Button)sender;

            ToolStripMenuItem noChangeItem = new ToolStripMenuItem(VALUE_WEAPON_NO_CHANGE);
            ToolStripMenuItem gloveItem = new ToolStripMenuItem(VALUE_WEAPON_GLOVE);
            ToolStripMenuItem swordItem = new ToolStripMenuItem(VALUE_WEAPON_SWORD);
            ToolStripMenuItem axeItem = new ToolStripMenuItem(VALUE_WEAPON_AXE);
            ToolStripMenuItem spearItem = new ToolStripMenuItem(VALUE_WEAPON_SPEAR);
            ToolStripMenuItem whipItem = new ToolStripMenuItem(VALUE_WEAPON_WHIP);
            ToolStripMenuItem bowItem = new ToolStripMenuItem(VALUE_WEAPON_BOW);
            ToolStripMenuItem boomerangItem = new ToolStripMenuItem(VALUE_WEAPON_BOOMERANG);
            ToolStripMenuItem javelinItem = new ToolStripMenuItem(VALUE_WEAPON_JAVELIN);

            contextMenuStrip.Items.Add(noChangeItem);
            contextMenuStrip.Items.Add(gloveItem);
            contextMenuStrip.Items.Add(swordItem);
            contextMenuStrip.Items.Add(axeItem);
            contextMenuStrip.Items.Add(spearItem);
            contextMenuStrip.Items.Add(whipItem);
            contextMenuStrip.Items.Add(bowItem);
            contextMenuStrip.Items.Add(boomerangItem);
            contextMenuStrip.Items.Add(javelinItem);

            noChangeItem.Click += Item_Click_Weapon;
            gloveItem.Click += Item_Click_Weapon;
            swordItem.Click += Item_Click_Weapon;
            axeItem.Click += Item_Click_Weapon;
            spearItem.Click += Item_Click_Weapon;
            whipItem.Click += Item_Click_Weapon;
            bowItem.Click += Item_Click_Weapon;
            boomerangItem.Click += Item_Click_Weapon;
            javelinItem.Click += Item_Click_Weapon;

            weaponButtonLookup[noChangeItem] = button;
            weaponButtonLookup[gloveItem] = button;
            weaponButtonLookup[swordItem] = button;
            weaponButtonLookup[axeItem] = button;
            weaponButtonLookup[spearItem] = button;
            weaponButtonLookup[whipItem] = button;
            weaponButtonLookup[bowItem] = button;
            weaponButtonLookup[boomerangItem] = button;
            weaponButtonLookup[javelinItem] = button;

            Point ptLowerLeft = new Point(0, button.Height);
            ptLowerLeft = button.PointToScreen(ptLowerLeft);
            contextMenuStrip.Show(ptLowerLeft);

        }

        private void Item_Click_Weapon(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            Button button = weaponButtonLookup[item];
            if (item.Text == VALUE_WEAPON_NO_CHANGE)
            {
                button.Text = item.Text;
                weaponSettings.Remove(weaponButtonPropertyNamesInv[button]);
            }
            else
            {
                if (!weaponSettings.Values.Contains(item.Text))
                {
                    button.Text = item.Text;
                    weaponSettings[weaponButtonPropertyNamesInv[button]] = item.Text;
                }
            }
            entriesChanged();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            entriesChanged();
            tabControl1.Enabled = checkBox2.Checked;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (!suspendUpdates)
            {
                entriesChanged();
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (!suspendUpdates)
            {
                bypassValidation = checkBox3.Checked;
                entriesChanged();
            }
        }
    }
}
