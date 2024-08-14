using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using SoMRandomizer.config.ui;
using SoMRandomizer.config.settings;
using SoMRandomizer.processing.ancientcave;
using SoMRandomizer.processing.bossrush;
using SoMRandomizer.processing.chaos;
using SoMRandomizer.processing.vanillarando;
using SoMRandomizer.processing.openworld;
using SoMRandomizer.processing.common;
using SoMRandomizer.config;
using SoMRandomizer.util;
using SoMRandomizer.help;
using SoMRandomizer.processing.common.structure;

namespace SoMRandomizer.forms
{
    /// note that for organization's sake, this class is split into multiple files for different features of the form.
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public partial class MainForm : Form, UiOptionsManager.OptionsListener
    {
        // generators to call out to to make rando roms
        private AncientCaveGenerator ancientCaveGenerator = new AncientCaveGenerator();
        private BossRushGenerator bossRushGenerator = new BossRushGenerator();
        private ChaosGenerator chaosGenerator = new ChaosGenerator();
        private VanillaRandoGenerator vanillaRandoGenerator = new VanillaRandoGenerator();
        private OpenWorldGenerator openWorldGenerator = new OpenWorldGenerator();
        private Dictionary<string, RomGenerator> generatorsByRomType = new Dictionary<string, RomGenerator>();

        // open world forms
        private PresetsForm openWorldPresetsForm;
        private PlandoForm plandoForm;

        // components that get disabled while we're generating
        private List<Control> enableComponents = new List<Control>();

        // difficulty stuff for older modes
        private Dictionary<string, Dictionary<string, DifficultySettings>> difficulties = new Dictionary<string, Dictionary<string, DifficultySettings>>();
        private Dictionary<string, DifficultySettings> customDifficulty = new Dictionary<string, DifficultySettings>();
        private Dictionary<string, int> visOptions = new Dictionary<string, int>();
        private string selectedDifficulty = "Casual";
        bool difficultyPopout = false;

        // character palette form
        private CharacterDesigner characterDesigner = new CharacterDesigner();

        // width of the form for when we are and are not showing the difficulty adjustment thing for the older modes
        const int noDifficultyWidth = 510;
        const int difficultyWidth = 1068;

        // true if currently making a rom
        private bool generating = false;

        // state booleans for updating properties display
        private bool handleOptionChanges = true;
        private bool handleIndividualChanges = true;

        // options forms & handlers
        private UiOptionsManager optionsManager = new UiOptionsManager();
        private OptionsFormManager propertyManager;
        OptionsCategoryForm generalOptionsCategory;
        OptionsCategoryForm openWorldOptionsCategory;

        // settings for each mode, and common settings
        private CommonSettings commonSettings;
        private VanillaRandoSettings vanillaRandoSettings;
        private OpenWorldSettings openWorldSettings;
        private AncientCaveSettings ancientCaveSettings;
        private BossRushSettings bossRushSettings;
        private ChaosSettings chaosSettings;

        // settings mapped by mode
        private Dictionary<string, RandoSettings> settingsByRomType = new Dictionary<string, RandoSettings>();

        public void optionsChanged()
        {
            handleOptionChanges = false;
            if (handleIndividualChanges)
            {
                txtOptions.Text = optionsManager.getOptionsString();
                clearOptionsError();
            }
            handleOptionChanges = true;
        }
        ToolTip t = new ToolTip();
        ToolTip tError = new ToolTip();

        public MainForm()
        {
            InitializeComponent();

            commonSettings = new CommonSettings();

            vanillaRandoSettings = new VanillaRandoSettings(commonSettings);
            openWorldSettings = new OpenWorldSettings(commonSettings);
            ancientCaveSettings = new AncientCaveSettings(commonSettings);
            bossRushSettings = new BossRushSettings(commonSettings);
            chaosSettings = new ChaosSettings(commonSettings);

            settingsByRomType[VanillaRandoSettings.MODE_KEY] = vanillaRandoSettings;
            settingsByRomType[OpenWorldSettings.MODE_KEY] = openWorldSettings;
            settingsByRomType[AncientCaveSettings.MODE_KEY] = ancientCaveSettings;
            settingsByRomType[BossRushSettings.MODE_KEY] = bossRushSettings;
            settingsByRomType[ChaosSettings.MODE_KEY] = chaosSettings;

            generatorsByRomType[VanillaRandoSettings.MODE_KEY] = vanillaRandoGenerator;
            generatorsByRomType[OpenWorldSettings.MODE_KEY] = openWorldGenerator;
            generatorsByRomType[AncientCaveSettings.MODE_KEY] = ancientCaveGenerator;
            generatorsByRomType[BossRushSettings.MODE_KEY] = bossRushGenerator;
            generatorsByRomType[ChaosSettings.MODE_KEY] = chaosGenerator;

            // populate your vanilla SoM ROM if you put one before
            string prevInputRom = AppConfig.APPCONFIG.getStringProperty("PreviousInputRom");
            if(prevInputRom != null)
            {
                txtInputROM.Text = prevInputRom;
            }

            // copy filename, and process with seed
            txtOutputROM.Text = txtInputROM.Text;
            processOutputFilename();

            lblVanillaNoRandoHint.Text = "Uncheck all randomization options to play the vanilla game" + Environment.NewLine + "with the selected hacks enabled.  Seed will be ignored.";
            optionsManager.addOptionsListener(this);
            propertyManager = new OptionsFormManager(optionsManager);

            // set up all the options controls
            initCommonComponents();
            initVanillaRandoComponents();
            initOpenWorldComponents();
            initAncientCaveComponents();
            initBossRushComponents();
            initChaosComponents();
            initDifficultyPaneComponents();
            
            tError.InitialDelay = 5; // show almost immediately
            tError.AutoPopDelay = 32767; // last forever
            t.AutoPopDelay = 32767;

            txtSeed.TextChanged += txtSeed_TextChanged;
            randomizeSeed();

            characterDesigner.Icon = Icon;

            cklACBiomeTypes.ItemCheck += cklACBiomeTypes_ItemCheck;
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 100;
            timer.Tick += Timer_Tick;
            timer.Start();

            ClientSize = new Size(510, ClientSize.Height);
            tabMode.SelectedIndexChanged += tabMode_SelectedIndexChanged;

            // disable some of the common components when we're running rom generation
            enableComponents.Add(btnBrowseInput); // browse
            enableComponents.Add(btnBrowseOutput); // browse
            enableComponents.Add(btnWriteROM); // generate
            enableComponents.Add(btnRandomSeed); // randomize seed
            enableComponents.Add(txtInputROM); // input rom
            enableComponents.Add(txtOutputROM); // output rom
            enableComponents.Add(txtSeed); // seed
            enableComponents.Add(txtOptions); // options
            enableComponents.Add(chkAppendSeedToFilename); // append seed to filename
            enableComponents.Add(btnGeneralOptions); // general options

            t.SetToolTip(btnOptionsCopy, "Copy options string to the clipboard.");
            t.SetToolTip(btnOptionsPaste, "Paste options string from the clipboard and replace current selections.");

            makeOpenWorldPresets();

            // character selection for ancient cave mode
            FormUtil.RequireAtLeastOneCheckbox(new CheckBox[] { chkACIncludeBoyChar, chkACIncludeGirlChar, chkACIncludeSpriteChar }.ToList());
            // character selection for boss rush mode
            FormUtil.RequireAtLeastOneCheckbox(new CheckBox[] { chkBossRushIncludeSpriteChar, chkBossRushIncludeGirlChar, chkBossRushIncludeBoyChar }.ToList());
            // character selection for chaos mode
            FormUtil.RequireAtLeastOneCheckbox(new CheckBox[] { chkChaosIncludeSpriteChar, chkChaosIncludeGirlChar, chkChaosIncludeBoyChar }.ToList());
            FormClosing += MainForm_FormClosing;

            Task.Run(updateCheck);
            optionsChanged();
        }

        private async Task updateCheck()
        {
            if(await UpdateCheck.checkForNewVersion())
            {
                BeginInvoke(new Action(newVersionAvailable));
            }
        }

        private void newVersionAvailable()
        {
            llbNewVersion.Visible = true;
            t.SetToolTip(llbNewVersion, "Click to open the Mana Rando GitHub to download the latest version of the randomizer.");
        }

        public string getInputRom()
        {
            return txtInputROM.Text;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // save the input rom for next time when we close the window 
            AppConfig.APPCONFIG.setConfigProperty("PreviousInputRom", txtInputROM.Text);
        }

        private void initCheckbox(CheckBox cb, string propertyName, StringValueSettings settings)
        {
            // associate a property with a checkbox
            enableComponents.Add(cb);
            optionsManager.addCheckbox(cb, propertyName, settings);
        }

        private void initTrackbar(TrackBar tb, string propertyName, StringValueSettings settings)
        {
            // associate a property with a trackbar
            enableComponents.Add(tb);
            optionsManager.addTrackbar(tb, propertyName, settings);
        }

        private void initCheckedListbox(CheckedListBox clb, string propertyName, StringValueSettings settings)
        {
            // associate a property with a checked listbox, which is that multi-checkbox thing we use for the ancient cave biome settings
            enableComponents.Add(clb);
            optionsManager.addCheckedListBox(clb, propertyName, settings);
        }

        private void initCombobox(ComboBox cb, string propertyName, StringValueSettings settings)
        {
            // associate a property with a dropdown combobox
            enableComponents.Add(cb);
            optionsManager.addComboBox(cb, propertyName, settings);
        }

        private void initTabControl(TabControl tabs, string propertyName, StringValueSettings settings)
        {
            // associate a property with a tabbed control
            enableComponents.Add(tabs);
            optionsManager.addTabControl(tabs, propertyName, settings);
        }

        private void initRadioButtons(RadioButton[] rbs, string propertyName, StringValueSettings settings)
        {
            // associate a property with a radio button group
            optionsManager.addRadioButtonGroup(rbs, propertyName, settings);
            foreach (RadioButton rb in rbs)
            {
                enableComponents.Add(rb);
            }
        }

        private void clearOptionsError()
        {
            txtOptions.BackColor = txtSeed.BackColor;
            t.SetToolTip(txtOptions, "Paste options to quickly select randomizer settings, or copy to share them.\nTyping in here is allowed, but not recommended.");
        }
        
        private void txtOptions_TextChanged(object sender, EventArgs e)
        {
            // options changed
            if (handleOptionChanges)
            {
                handleIndividualChanges = false;
                try
                {
                    optionsManager.setOptions(txtOptions.Text);
                    txtOptions.BackColor = txtSeed.BackColor;
                    tError.SetToolTip(txtOptions, null);
                    t.SetToolTip(txtOptions, "Paste options to quickly select randomizer settings, or copy to share them.\nTyping in here is allowed, but not recommended.");
                }
                catch (OptionsException ee)
                {
                    string errorMsg = "";
                    if(ee.versionMatch == null)
                    {
                        errorMsg = "No version specified! Options may not be compatible";
                        txtOptions.BackColor = Color.FromArgb(255, 255, 128);
                    }
                    else if(!(bool)ee.versionMatch)
                    {
                        errorMsg = "Version mismatch! Options may not be compatible";
                        txtOptions.BackColor = Color.FromArgb(255, 128, 128);
                    }
                    if(ee.valueError != null)
                    {
                        if(errorMsg.Length > 0)
                        {
                            errorMsg = errorMsg + "\n";
                        }
                        errorMsg = errorMsg + ee.valueError;
                        txtOptions.BackColor = Color.FromArgb(255, 128, 128);
                    }
                    tError.SetToolTip(txtOptions, errorMsg);
                    t.SetToolTip(txtOptions, null);
                }
                handleIndividualChanges = true;
            }
        }

        private void tabMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            // game mode selection changed
            string currentRomType = getRomType();
            if (currentRomType == VanillaRandoSettings.MODE_KEY || currentRomType == OpenWorldSettings.MODE_KEY)
            {
                // early-game rabites don't damage you with this on; it's a minor detail but we switch it off by default here
                // for vanilla-like modes, but keep it on for procgen modes
                propertyManager.setPropertyValue("generalMiscMechanical", CommonSettings.PROPERTYNAME_STARTER_GEAR, "no");
                llbDifficultyDetails.Visible = false;
                difficultyPopout = false;
                llbDifficultyDetails.LinkColor = Color.Red;
                refreshWidth();
            }
            else
            {
                propertyManager.setPropertyValue("generalMiscMechanical", CommonSettings.PROPERTYNAME_STARTER_GEAR, "yes");
                llbDifficultyDetails.Visible = true;
            }

            // update the warning for unfinished modes.
            switch(currentRomType)
            {
                case OpenWorldSettings.MODE_KEY:
                    label47.Text = "";
                    break;
                case BossRushSettings.MODE_KEY:
                    label47.Text = "Note: this mode still needs a lot of work! Don't expect much yet!";
                    break;
                case ChaosSettings.MODE_KEY:
                    label47.Text = "Note: this mode still needs a little bit of work! Don't expect much yet!";
                    break;
                case AncientCaveSettings.MODE_KEY:
                    label47.Text = "Note: this mode still needs a lot of work! Don't expect much yet!";
                    break;
                case VanillaRandoSettings.MODE_KEY:
                    label47.Text = "";
                    break;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // periodically update progress for ancient cave generation since it takes a bit
            int currentProgress = commonSettings.getInt(CommonSettings.PROPERTYNAME_CURRENT_PROGRESS);
            prgGenerationProgress.Value = currentProgress;
        }

        private void cklACBiomeTypes_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // don't allow deselection of every item in the ancient cave biome selection
            if (cklACBiomeTypes.CheckedItems.Count == 1)
            {
                if (e.CurrentValue == CheckState.Checked)
                {
                    e.NewValue = CheckState.Checked;
                }
            }
            cklACBiomeTypes.SelectedIndex = -1;
        }
        
        private void picCatBread_Click(object sender, EventArgs e)
        {
            // clicked on the cat bread
            System.Diagnostics.Process.Start("http://secretofmanaancientcave.blogspot.com");
        }

        private void txtSeed_TextChanged(object sender, EventArgs e)
        {
            // changed seed; add to output filename
            processOutputFilename();
        }

        private void btnBrowseInput_Click(object sender, EventArgs e)
        {
            // select input rom
            OpenFileDialog of = new OpenFileDialog();
            of.Multiselect = false;
            DialogResult dr = of.ShowDialog();
            if (dr == DialogResult.OK)
            {
                txtInputROM.Text = of.FileName;
                if (txtOutputROM.Text == null || txtOutputROM.Text.Length == 0)
                {
                    txtOutputROM.Text = txtInputROM.Text;
                    processOutputFilename();
                }
            }
        }

        private void processOutputFilename()
        {
            // process output filename as input filename + [seed]
            string outFilename = txtOutputROM.Text;
            string seed = txtSeed.Text;
            int periodIndex = outFilename.LastIndexOf(".");
            string beforeExtension = outFilename;
            string extension = "";

            if (periodIndex >= 0)
            {
                beforeExtension = outFilename.Substring(0, periodIndex);
                extension = outFilename.Substring(periodIndex);
            }

            int oBracketIndex = beforeExtension.LastIndexOf("[");
            if (oBracketIndex >= 0)
            {
                beforeExtension = beforeExtension.Substring(0, oBracketIndex);
            }

            if (chkAppendSeedToFilename.Checked && beforeExtension.Length > 0)
            {
                beforeExtension += "[" + seed + "]";
            }
            txtOutputROM.Text = beforeExtension + extension;
        }

        private void btnBrowseOutput_Click(object sender, EventArgs e)
        {
            // select output filename
            OpenFileDialog of = new OpenFileDialog();
            of.Multiselect = false;
            DialogResult dr = of.ShowDialog();
            if (dr == DialogResult.OK)
            {
                txtOutputROM.Text = of.FileName;
            }
        }

        private void btnRandomSeed_Click(object sender, EventArgs e)
        {
            // randomize seed with some hex characters
            randomizeSeed();
            processOutputFilename();
        }

        private void randomizeSeed()
        {
            txtSeed.Text = randomHexString();
        }

        private string randomHexString()
        {
            string chars = "0123456789ABCDEF";
            char[] stringChars = new char[16];
            Random random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new String(stringChars);
        }

        private void chkAppendSeedToFilename_CheckedChanged(object sender, EventArgs e)
        {
            // input rom path changed; re-process output filename
            processOutputFilename();
        }

        private void btnWriteROM_Click(object sender, EventArgs e)
        {
            // generate button press
            generateWithCurrentSettings(false);
        }

        public void generateWithCurrentSettings(bool hideUI)
        {
            generating = true;
            label7.Visible = true;
            foreach (Control comp in enableComponents)
            {
                comp.Enabled = false;
            }
            picCatBread.Enabled = true;
            GenerateParam p = new GenerateParam();
            p.skipConfirmation = hideUI;
            p.romType = commonSettings.get(CommonSettings.PROPERTYNAME_MODE);
            
            // this is just for the log
            commonSettings.set(CommonSettings.PROPERTYNAME_ALL_ENTERED_OPTIONS, txtOptions.Text);
            commonSettings.set(CommonSettings.PROPERTYNAME_VERSION, RomGenerator.VERSION_NUMBER);
            propertyManager.closeAll();
            generalOptionsCategory.Hide();
            openWorldOptionsCategory.Hide();

            
            // start rom generation on another thread so not to hold up the UI
            Thread t = new Thread(new ParameterizedThreadStart(generate));
            t.Start(p);
            if (!hideUI)
            {
                return;
            }
            // if the UI is hidden we wait for generation then close the app
            t.Join();
            Close();
        }

        // an object with a few settings to pass into the rom generation thread
        private class GenerateParam
        {
            public string romType;
            public bool skipConfirmation;
        }

        private void generate(object param)
        {
            GenerateParam genParam = (GenerateParam)param;
            try
            {
                if (txtOutputROM.Text != null && txtOutputROM.Text != "" && File.Exists(txtOutputROM.Text))
                {
                    if (!genParam.skipConfirmation)
                    {
                        DialogResult dialogResult = MessageBox.Show("Destination file already exists.  Sure you want to overwrite?", "Overwrite", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.No)
                        {
                            return;
                        }
                    }
                }

                // grab a couple extra things off the UI if necessary

                // these difficulty settings are only applicable to the older modes; don't bother grabbing them for vanilla rando or open world
                if (genParam.romType == AncientCaveSettings.MODE_KEY || genParam.romType == BossRushSettings.MODE_KEY || genParam.romType == ChaosSettings.MODE_KEY)
                {
                    string selectedDifficulty = this.selectedDifficulty;
                    DifficultySettings diff = customDifficulty[genParam.romType];
                    if (difficulties.ContainsKey(genParam.romType) && difficulties[genParam.romType].ContainsKey(selectedDifficulty))
                    {
                        diff = difficulties[genParam.romType][selectedDifficulty];
                    }
                    foreach (string key in DifficultySettings.displayStrings.Keys)
                    {
                        if (DifficultySettings.isGrowthValue(key))
                        {
                            IntGrowthValue val = diff.getGrowthValue(key);
                            commonSettings.set(DifficultySettings.DIFFICULTY_PROPERTY_PREFIX + DifficultySettings.displayStrings[key] + "_base", val.baseValue.ToString());
                            commonSettings.set(DifficultySettings.DIFFICULTY_PROPERTY_PREFIX + DifficultySettings.displayStrings[key] + "_growth", val.growthValue.ToString());
                            commonSettings.set(DifficultySettings.DIFFICULTY_PROPERTY_PREFIX + DifficultySettings.displayStrings[key] + "_exp", val.exponent.ToString());
                        }
                        else
                        {
                            double val = diff.getDoubleValue(key);
                            commonSettings.set(DifficultySettings.DIFFICULTY_PROPERTY_PREFIX + DifficultySettings.displayStrings[key], val.ToString());
                        }
                    }
                }

                // pass the customized character colors down if specified
                if (commonSettings.get(CommonSettings.PROPERTYNAME_RANDOMIZE_CHAR_COLORS) == "custom")
                {
                    for (int i = 1; i < 16; i++)
                    {
                        SnesColor boyColor = characterDesigner.boyCurrent.getColor(i);
                        SnesColor girlColor = characterDesigner.girlCurrent.getColor(i);
                        SnesColor spriteColor = characterDesigner.spriteCurrent.getColor(i);
                        commonSettings.set(CommonSettings.PROPERTYNAME_PREFIX_CUSTOM_CHARACTER_COLORS + "BoyRed" + i, boyColor.getRed().ToString());
                        commonSettings.set(CommonSettings.PROPERTYNAME_PREFIX_CUSTOM_CHARACTER_COLORS + "BoyGreen" + i, boyColor.getGreen().ToString());
                        commonSettings.set(CommonSettings.PROPERTYNAME_PREFIX_CUSTOM_CHARACTER_COLORS + "BoyBlue" + i, boyColor.getBlue().ToString());
                        commonSettings.set(CommonSettings.PROPERTYNAME_PREFIX_CUSTOM_CHARACTER_COLORS + "GirlRed" + i, girlColor.getRed().ToString());
                        commonSettings.set(CommonSettings.PROPERTYNAME_PREFIX_CUSTOM_CHARACTER_COLORS + "GirlGreen" + i, girlColor.getGreen().ToString());
                        commonSettings.set(CommonSettings.PROPERTYNAME_PREFIX_CUSTOM_CHARACTER_COLORS + "GirlBlue" + i, girlColor.getBlue().ToString());
                        commonSettings.set(CommonSettings.PROPERTYNAME_PREFIX_CUSTOM_CHARACTER_COLORS + "SpriteRed" + i, spriteColor.getRed().ToString());
                        commonSettings.set(CommonSettings.PROPERTYNAME_PREFIX_CUSTOM_CHARACTER_COLORS + "SpriteGreen" + i, spriteColor.getGreen().ToString());
                        commonSettings.set(CommonSettings.PROPERTYNAME_PREFIX_CUSTOM_CHARACTER_COLORS + "SpriteBlue" + i, spriteColor.getBlue().ToString());
                    }
                }

                try
                {
                    // try to make the ROM
                    RomGenerator.initGeneration(txtInputROM.Text, txtOutputROM.Text, txtSeed.Text, generatorsByRomType, commonSettings, settingsByRomType);
                    MessageBox.Show("Done!");
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            finally
            {
                // if closed window behind the popup
                if (!IsDisposed)
                {
                    BeginInvoke(new Action(done));
                }
            }
        }

        private void done()
        {
            label7.Visible = false;
            foreach (Control comp in enableComponents)
            {
                comp.Enabled = true;
            }
            generating = false;
        }

        private string getRomType()
        {
            // this is determined by the tab you've currently selected
            return commonSettings.get(CommonSettings.PROPERTYNAME_MODE);
        }

        private void btnGeneralOptions_Click(object sender, EventArgs e)
        {
            // general options button
            generalOptionsCategory.Show();
        }

        private void btnChoasMoreOptions_Click(object sender, EventArgs e)
        {
            // chaos more options button
            propertyManager.showForm("chaos");
        }

        private void btnBossRushMoreOptions_Click(object sender, EventArgs e)
        {
            // boss rush more options button
            propertyManager.showForm("bossrush");
        }

        private void trbVanillaEnemyScaling_Scroll(object sender, EventArgs e)
        {
            // show the percentage value for enemy scaling in vanilla rando
            int sel = trbVanillaEnemyScaling.Value;
            string printVal = "?";
            string[] percentStrings = vanillaRandoSettings.getOptionsValues(VanillaRandoSettings.PROPERTYNAME_ENEMY_SCALING);
            printVal = percentStrings[sel];

            if (trbVanillaEnemyScaling.Value == percentStrings.Length / 2)
            {
                label36.Text = "Normal [" + printVal + "%]";
            }
            else if (trbVanillaEnemyScaling.Value == 0)
            {
                label36.Text = "Easiest [" + printVal + "%]";
            }
            else if (trbVanillaEnemyScaling.Value < percentStrings.Length / 2)
            {
                label36.Text = "Easier [" + printVal + "%]";
            }
            else if (trbVanillaEnemyScaling.Value == percentStrings.Length - 1)
            {
                label36.Text = "Hardest [" + printVal + "%]";
            }
            else if (trbVanillaEnemyScaling.Value > percentStrings.Length / 2)
            {
                label36.Text = "Harder [" + printVal + "%]";
            }
        }

        HelpPages helpPages = new HelpPages();

        private void openInfoForm()
        {
            // open help for selected tab
            string rt = getRomType();
            helpPages.openInfoForm(rt);
        }

        private void picVanillaDetails_Click(object sender, EventArgs e)
        {
            // vanilla rando help button
            openInfoForm();
        }

        private void picDiscord_Click(object sender, EventArgs e)
        {
            // discord icon button
            System.Diagnostics.Process.Start("https://discord.gg/YfmUHqU");
        }

        private void picTwitch_Click(object sender, EventArgs e)
        {
            // twitch icon button
            System.Diagnostics.Process.Start("https://twitch.tv/moppleton");
        }

        private void picBlogspot_Click(object sender, EventArgs e)
        {
            // blogspot icon button
            System.Diagnostics.Process.Start("http://secretofmanaancientcave.blogspot.com/");
        }

        private void picMail_Click(object sender, EventArgs e)
        {
            // email icon button
            System.Diagnostics.Process.Start("mailto:umokumok@gmail.com");
        }

        private void picACDetails_Click(object sender, EventArgs e)
        {
            // ancient cave help button
            openInfoForm();
        }

        private void picBossRushDetails_Click(object sender, EventArgs e)
        {
            // boss rush help button
            openInfoForm();
        }

        private void picChaosDetails_Click(object sender, EventArgs e)
        {
            // chaos help button
            openInfoForm();
        }

        private void llbNewVersion_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // new version available label
            System.Diagnostics.Process.Start(UpdateCheck.updateDownloadUrl);
        }

        private void cmbOpenStatGrowth_SelectedIndexChanged(object sender, EventArgs e)
        {
            // open world enemy stat growth
            if(((string)cmbOpenStatGrowth.SelectedItem).Contains("None"))
            {
                // difficulty option for enemies
                cmbOpenStatGrowthDifficulty.Enabled = false;
                // randomize bosses - default to swap
                if(cmbOpenRandomizeBosses.SelectedIndex == 2)
                {
                    cmbOpenRandomizeBosses.SelectedIndex = 1;
                }
            }
            else
            {
                cmbOpenStatGrowthDifficulty.Enabled = true;
                // randomize bosses - default to random
                if (cmbOpenRandomizeBosses.SelectedIndex == 1)
                {
                    cmbOpenRandomizeBosses.SelectedIndex = 2;
                }
            }
        }

        private void picOpenDetails_Click(object sender, EventArgs e)
        {
            // open world help button
            openInfoForm();
        }

        private void btnOpenMoreOptions_Click(object sender, EventArgs e)
        {
            // open world options button
            openWorldOptionsCategory.Show();
        }

        private void btnPresets_Click(object sender, EventArgs e)
        {
            // open world presets button
            openWorldPresetsForm.Show();
        }

        private void btnOptionsCopy_Click(object sender, EventArgs e)
        {
            // options copy button
            Clipboard.SetText(txtOptions.Text);
        }

        private void btnOptionsPaste_Click(object sender, EventArgs e)
        {
            // options paste button
            try
            {
                string clipText = Clipboard.GetText();
                if (clipText.Length > 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Pasting will replace current settings.  Are you sure?", "Overwrite", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.No)
                    {
                        return;
                    }

                    txtOptions.Text = clipText;
                }
                else
                {
                    MessageBox.Show("Nothing to paste!");
                }
            }
            catch(Exception)
            {
                MessageBox.Show("Can't paste clipboard contents!");
            }
        }

        private void btnPlando_Click(object sender, EventArgs e)
        {
            // plando button
            plandoForm.Show();
        }
    }
}
