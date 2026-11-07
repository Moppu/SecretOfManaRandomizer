using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;

namespace SoMRandomizer.util
{
    /// <summary>
    /// Utilities for Windows Forms.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class FormUtil
    {
        /// <summary>
        /// File dialog filter for SNES ROM images. Without a filter, Mono shows an empty file type list.
        /// </summary>
        public const string ROM_FILE_FILTER = "SNES ROM (*.sfc;*.smc)|*.sfc;*.smc|All files (*.*)|*.*";

        delegate void Invoker(Control formInstance, String attributeName, Object value);
        // Delegate to control creation thread to safely set fields
        public static void SetFormField(Control formInstance, String attributeName, Object value)
        {
            if (formInstance.InvokeRequired)
            {
                formInstance.BeginInvoke(new Invoker(SetFormField), new object[] { formInstance, attributeName, value });
            }
            else
            {
                Type t = formInstance.GetType();
                PropertyInfo fi = t.GetProperty(attributeName);
                fi.SetValue(formInstance, value, null);
            }
        }

        private static Dictionary<CheckBox, List<CheckBox>> checkboxListLookup = new Dictionary<CheckBox, List<CheckBox>>();
        // don't allow all of the given checkboxes to be deselected at once
        public static void RequireAtLeastOneCheckbox(List<CheckBox> checkboxes)
        {
            foreach(CheckBox box in checkboxes)
            {
                checkboxListLookup[box] = checkboxes;
                box.CheckedChanged += Box_CheckedChanged;
            }
        }

        // callback for keeping at least one checkbox of a group selected at all times
        private static void Box_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox box = (CheckBox)sender;
            if (!box.Checked)
            {
                if (checkboxListLookup.ContainsKey(box))
                {
                    bool anyChecked = false;
                    foreach(CheckBox box2 in checkboxListLookup[box])
                    {
                        if(box2 != box && box2.Checked)
                        {
                            anyChecked = true;
                            break;
                        }
                    }
                    if(!anyChecked)
                    {
                        box.Checked = true;
                    }
                }
            }
        }
    }
}
