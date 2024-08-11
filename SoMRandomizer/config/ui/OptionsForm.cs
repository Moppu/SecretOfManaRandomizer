using SoMRandomizer.config.settings;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SoMRandomizer.config.ui
{
    /// <summary>
    /// Table of editable properties with descriptions
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public partial class OptionsForm : Form
    {
        private Dictionary<string, string> propertyValues;
        private Dictionary<string, Dictionary<string, Form>> propertyForms = new Dictionary<string, Dictionary<string, Form>>();
        private Dictionary<Control, String> propertyMapping = new Dictionary<Control, string>();
        private Dictionary<string, string[]> propertyValueMapping = new Dictionary<string, string[]>();
        private UiOptionsManager optionsManager;
        private List<string> supportedProperties = new List<string>();
        private bool shownYet = false;
        public OptionsForm(String title, Dictionary<string, string> propertyValues, UiOptionsManager optionsManager)
        {
            this.propertyValues = propertyValues;
            this.optionsManager = optionsManager;

            InitializeComponent();

            Text = title;
            label1.Text = title;

            FormClosing += OptionsForm_FormClosing;
            Shown += OptionsForm_Shown;
            tablePanel1.col3Width = 200;
        }

        private void OptionsForm_Shown(object sender, EventArgs e)
        {
            if(!shownYet)
            {
                // set form size based on number of properties; min 5, max 10
                int numProperties = supportedProperties.Count;
                if(numProperties < 5)
                {
                    numProperties = 5;
                }
                if (numProperties > 10)
                {
                    numProperties = 10;
                }
                Height = 50 + numProperties * 50;
                shownYet = true;
            }
        }

        private void OptionsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            Parent = null;
            e.Cancel = true; // this cancels the close event.
        }

        public bool hasProperty(string name)
        {
            return supportedProperties.Contains(name);
        }

        // create a dropdown with selectable options for a property
        public void makeMultiStringValueProperty(String propertyKey, String propertyName, String propertyDescription, StringValueSettings settings, Dictionary<string, Form> forms)
        {
            supportedProperties.Add(propertyKey);
            tableLayoutPanel1.RowCount++;
            ComboBox combo = new ComboBox();
            combo.DropDownStyle = ComboBoxStyle.DropDownList;
            string[] displayStrings = settings.getDisplayValues(propertyKey);
            foreach (String option in displayStrings)
            {
                combo.Items.Add(option);
            }
            combo.SelectedItem = displayStrings[settings.getDefaultIndex(propertyKey)];
            string[] optionValues = settings.getOptionsValues(propertyKey);
            propertyValues[propertyKey] = optionValues[combo.SelectedIndex];
            propertyForms[propertyKey] = forms;
            propertyMapping[combo] = propertyKey;
            propertyValueMapping[propertyKey] = optionValues;
            combo.SelectedIndexChanged += Combo_SelectedIndexChanged;
            optionsManager.addComboBox(combo, propertyKey, settings);
            tablePanel1.addRow(propertyName, propertyDescription, combo);
        }

        // callback for combobox options
        private void Combo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (propertyMapping.ContainsKey((ComboBox)sender))
            {
                string propKey = propertyMapping[(ComboBox)sender];
                if (propertyForms.ContainsKey(propKey))
                {
                    Dictionary<string, Form> forms = propertyForms[propKey];
                    if (forms != null)
                    {
                        string selItem = (string)((ComboBox)sender).SelectedItem;
                        if (forms.ContainsKey(selItem))
                        {
                            forms[selItem].Show();
                        }
                    }
                }

                int selectedIndex = ((ComboBox)sender).SelectedIndex;
                propertyValues[propKey] = propertyValueMapping[propKey][selectedIndex];
            }
        }

        public void makeSeparator(String label)
        {
            tableLayoutPanel1.RowCount++;
            Label l1 = new Label();
            l1.Text = label;
            l1.Font = new Font(l1.Font.FontFamily, l1.Font.Size + 3);
            l1.ForeColor = Color.Blue;
            tableLayoutPanel1.Controls.Add(l1, 0, tableLayoutPanel1.RowCount - 1);
        }

        // make a combo with yes/no for boolean options
        public void makeBooleanValueProperty(String propertyKey, String propertyName, String propertyDescription, StringValueSettings settings)
        {
            makeMultiStringValueProperty(propertyKey, propertyName, propertyDescription, settings, null);
        }

        // make a numericdropdown for selection of value for numeric options
        public void makeNumericValueProperty(String propertyKey, String propertyName, String propertyDescription, StringValueSettings settings)
        {
            supportedProperties.Add(propertyKey);
            tableLayoutPanel1.RowCount++;
            NumericUpDown numericUpDown = new NumericUpDown();
            numericUpDown.Minimum = new decimal(settings.getDoubleMinimum(propertyKey));
            numericUpDown.Maximum = new decimal(settings.getDoubleMaximum(propertyKey));
            int decimalPlaces = settings.getDecimalPlaces(propertyKey);
            numericUpDown.DecimalPlaces = decimalPlaces;
            double defaultValue = settings.getDoubleDefault(propertyKey);
            numericUpDown.Increment = new decimal(1.0 / Math.Pow(10, decimalPlaces));
            numericUpDown.Value = new decimal(defaultValue);
            propertyValues[propertyKey] = defaultValue.ToString();
            propertyMapping[numericUpDown] = propertyKey;
            numericUpDown.ValueChanged += NumericUpDown_ValueChanged;
            optionsManager.addNumericUpDown(numericUpDown, propertyKey, settings);
            tablePanel1.addRow(propertyName, propertyDescription, numericUpDown);
        }

        // callback for numericupdown options
        private void NumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (propertyMapping.ContainsKey((NumericUpDown)sender))
            {
                string propKey = propertyMapping[(NumericUpDown)sender];
                double newValue = ((double)((NumericUpDown)sender).Value);
                propertyValues[propKey] = newValue.ToString();
            }
        }

        public void setPropertyValue(String propertyKey, String propertyValue)
        {
            string[] vals = propertyValueMapping[propertyKey];
            for (int i = 0; i < vals.Length; i++)
            {
                if (vals[i] == propertyValue)
                {
                    foreach (ComboBox box in propertyMapping.Keys)
                    {
                        if (propertyMapping[box] == propertyKey)
                        {
                            box.SelectedIndex = i;
                        }
                    }
                }
            }
        }

        public ComboBox getPropertyCombo(string propertyKey)
        {
            foreach (Control cb in propertyMapping.Keys)
            {
                if (propertyMapping[cb] == propertyKey)
                {
                    return (ComboBox)cb;
                }
            }
            return null;
        }

        public NumericUpDown getPropertyUpDown(string propertyKey)
        {
            foreach (Control cb in propertyMapping.Keys)
            {
                if (propertyMapping[cb] == propertyKey)
                {
                    return (NumericUpDown)cb;
                }
            }
            return null;
        }

        public Dictionary<ComboBox, string> getAllPropertyCombos()
        {
            Dictionary<ComboBox, string> propertyCombos = new Dictionary<ComboBox, string>();
            foreach(Control c in propertyMapping.Keys)
            {
                if(c is ComboBox)
                {
                    propertyCombos[(ComboBox)c] = propertyMapping[c];
                }
            }
            return propertyCombos;
        }

        public Dictionary<NumericUpDown, string> getAllNumericUpDowns()
        {
            Dictionary<NumericUpDown, string> propertyUpDowns = new Dictionary<NumericUpDown, string>();
            foreach (Control c in propertyMapping.Keys)
            {
                if (c is NumericUpDown)
                {
                    propertyUpDowns[(NumericUpDown)c] = propertyMapping[c];
                }
            }
            return propertyUpDowns;
        }
    }
}
