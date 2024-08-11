using SoMRandomizer.util.rng;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SoMRandomizer.forms
{
    /// <summary>
    /// Form used to select presets for open world mode.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public partial class PresetsForm : Form
    {
        int tableWidth = 100;
        private TextBox seedTextBox;
        private Preset resetPreset;

        public PresetsForm(string title, List<Preset> presets, TextBox seedTextBox, Preset resetPreset) 
        {
            InitializeComponent();
            this.seedTextBox = seedTextBox;
            this.resetPreset = resetPreset;
            ToolTip t = new ToolTip();
            t.AutoPopDelay = 32767;

            label1.Text = title;
            FormClosing += MyForm_FormClosing;

            tableLayoutPanel1.RowCount = 0;

            if (IsRunningOnMono())
            {
                tableLayoutPanel1.ColumnStyles[1].SizeType = SizeType.Absolute;
                tableLayoutPanel1.ColumnStyles[1].Width = 600;
            }

            FormClosing += MyForm_FormClosing;
            tableWidth = tableLayoutPanel1.Width;
            foreach(Preset preset in presets)
            {
                addPreset(preset, t);
            }
            t.SetToolTip(button1, "Reset to the default settings shown when you load the randomizer.");
            MinimumSize = new Size(500, 200);
        }

        public static bool IsRunningOnMono()
        {
            return Type.GetType("Mono.Runtime") != null;
        }

        private void MyForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            e.Cancel = true; // this cancels the close event.
        }

        private List<Label> descLabels = new List<Label>();
        private List<Label> nameLabels = new List<Label>();

        private Dictionary<Button, Preset> callbacks = new Dictionary<Button, Preset>();

        private void addPreset(Preset preset, ToolTip t)
        {
            tableLayoutPanel1.RowCount++;
            if (preset.separatorOnly)
            {
                tablePanel1.addHeader(preset.name, preset.fontSizeChange);
            }
            else
            {
                Button button = new Button();
                button.Text = "Select";
                button.Click += Button_Click;
                callbacks[button] = preset;
                t.SetToolTip(button, preset.tooltip);
                tablePanel1.addRow(preset.name, preset.description, button);
            }
        }

        private void Button_Click(object sender, EventArgs e)
        {
            callbacks[(Button)sender].select(this, seedTextBox.Text);
        }

        public class Preset
        {
            public Dictionary<TabControl, string[]> tabNameSelections = new Dictionary<TabControl, string[]>();
            public Dictionary<ComboBox, string[]> comboBoxSelections = new Dictionary<ComboBox, string[]>();
            public Dictionary<CheckBox, bool[]> checkBoxSelections = new Dictionary<CheckBox, bool[]>();
            public Dictionary<NumericUpDown, double[]> numericUpDownSelections = new Dictionary<NumericUpDown, double[]>();

            public bool hideUi = false;
            public Form1 mainForm;
            public bool separatorOnly = false;
            public int fontSizeChange = 0;

            public string name;
            public string description;
            public string tooltip;

            public void select(PresetsForm presetsForm, string seedSelection)
            {
                Random r = new DotNet110Random(HashcodeUtil.GetDeterministicHashCode(seedSelection));
                bool confirmHide = false;
                if (hideUi)
                {
                    DialogResult dialogResult = MessageBox.Show("Would you like to obscure the chosen settings?  If you pick \"Yes,\" the main window will be hidden and the current selections will " +
                        "be generated into a ROM immediately.  When the \"Done\" popup comes up, you'll know it's ready.  " +
                        "If you pick no, the settings will be selected on the main window, and you'll be able to see what it picked for you.", "Confirm", MessageBoxButtons.YesNo);
                    confirmHide = dialogResult == DialogResult.Yes;

                    if (confirmHide)
                    {
                        presetsForm.Visible = false;
                        if (mainForm != null)
                        {
                            mainForm.Visible = false;
                        }
                    }
                }
                foreach(TabControl t in tabNameSelections.Keys)
                {
                    foreach(TabPage tp in t.TabPages)
                    {
                        if(tp.Text == tabNameSelections[t][r.Next() % tabNameSelections[t].Length])
                        {
                            t.SelectedTab = tp;
                        }
                    }
                }

                foreach (ComboBox t in comboBoxSelections.Keys)
                {
                    t.SelectedItem = comboBoxSelections[t][r.Next() % comboBoxSelections[t].Length];
                }

                foreach (CheckBox t in checkBoxSelections.Keys)
                {
                    t.Checked = checkBoxSelections[t][r.Next() % checkBoxSelections[t].Length];
                }

                foreach (NumericUpDown t in numericUpDownSelections.Keys)
                {
                    t.Value = new decimal(numericUpDownSelections[t][r.Next() % numericUpDownSelections[t].Length]);
                }

                if (hideUi && confirmHide)
                {
                    mainForm.generateWithCurrentSettings(true);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            resetPreset.select(this, seedTextBox.Text);
        }
    }
}
