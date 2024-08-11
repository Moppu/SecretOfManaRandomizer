using SoMRandomizer.config.settings;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SoMRandomizer.config.ui
{
    /// <summary>
    /// Manage options selection subforms and add property selections to them.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class OptionsFormManager
    {
        private Dictionary<string, string> propertyValues;
        private UiOptionsManager optionsManager;
        public OptionsFormManager(UiOptionsManager optionsManager)
        {
            this.propertyValues = new Dictionary<string, string>();
            this.optionsManager = optionsManager;
        }
        // mapped by "class" - ie, the form with the control to change it. there are several for general and open world, and one for the others atm
        Dictionary<string, OptionsForm> propertyForms = new Dictionary<string, OptionsForm>();
        public void makePropertyClass(string name, string formTitle)
        {
            OptionsForm form = new OptionsForm(formTitle, propertyValues, optionsManager);
            propertyForms[name] = form;
        }
        public void makeMultiStringValueProperty(string propertyClass, string propertyKey, string propertyName, string propertyDescription, StringValueSettings settings)
        {
            propertyForms[propertyClass].makeMultiStringValueProperty(propertyKey, propertyName, propertyDescription, settings, null);
        }
        public void makeMultiStringValueProperty(string propertyClass, string propertyKey, string propertyName, string propertyDescription, StringValueSettings settings, Dictionary<string, Form> forms)
        {
            propertyForms[propertyClass].makeMultiStringValueProperty(propertyKey, propertyName, propertyDescription, settings, forms);
        }

        public void makeSeparator(string propertyClass, string label)
        {
            propertyForms[propertyClass].makeSeparator(label);
        }

        public void makeBooleanValueProperty(string propertyClass, string propertyKey, string propertyName, string propertyDescription, StringValueSettings settings)
        {
            propertyForms[propertyClass].makeBooleanValueProperty(propertyKey, propertyName, propertyDescription, settings);
        }

        public void makeNumericValueProperty(string propertyClass, string propertyKey, string propertyName, string propertyDescription, StringValueSettings settings)
        {
            propertyForms[propertyClass].makeNumericValueProperty(propertyKey, propertyName, propertyDescription, settings);
        }

        public void setPropertyValue(string propertyClass, string propertyKey, string propertyValue)
        {
            propertyForms[propertyClass].setPropertyValue(propertyKey, propertyValue);
        }
        
        public void showForm(string propertyClass)
        {
            propertyForms[propertyClass].Show();
        }

        public OptionsForm getForm(string propertyClass)
        {
            return propertyForms[propertyClass];
        }

        public List<ComboBox> getFormCombosByPrefix(string prefix)
        {
            List<ComboBox> values = new List<ComboBox>();
            foreach(string className in propertyForms.Keys)
            {
                if(className.StartsWith(prefix))
                {
                    values.AddRange(propertyForms[className].getAllPropertyCombos().Keys);
                }
            }
            return values;
        }

        public List<NumericUpDown> getFormUpDownsByPrefix(string prefix)
        {
            List<NumericUpDown> values = new List<NumericUpDown>();
            foreach (string className in propertyForms.Keys)
            {
                if (className.StartsWith(prefix))
                {
                    values.AddRange(propertyForms[className].getAllNumericUpDowns().Keys);
                }
            }
            return values;
        }

        public OptionsForm getFormByProperty(string property)
        {
            foreach(OptionsForm form in propertyForms.Values)
            {
                if(form.hasProperty(property))
                {
                    return form;
                }
            }
            return null;
        }

        public void closeAll()
        {
            foreach(Form value in propertyForms.Values)
            {
                value.Hide();
            }
        }
    }
}
