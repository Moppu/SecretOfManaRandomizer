using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SoMRandomizer.config.ui
{
    /// <summary>
    /// Form allowing selection of an options category, which in turn opens another form with the individual settings.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public partial class OptionsCategoryForm : Form
    {
        private OptionsFormManager propManager;
        private bool shownYet = false;
        public OptionsCategoryForm(OptionsFormManager propManager, string title, string labelTitle)
        {
            this.propManager = propManager;
            InitializeComponent();

            Text = title;
            label1.Text = labelTitle;
            FormClosing += OptionsCategoryForm_FormClosing;
            Shown += OptionsCategoryForm_Shown;
        }

        private void OptionsCategoryForm_Shown(object sender, EventArgs e)
        {
            if (!shownYet)
            {
                int numProperties = tableLayoutPanel1.RowCount;
                if (numProperties < 5)
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

        private void OptionsCategoryForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            e.Cancel = true; // this cancels the close event.
            Parent = null;
        }

        private Dictionary<Button, string> callbacks = new Dictionary<Button, string>();

        public void addCategory(string name, string description, string formName)
        {
            tableLayoutPanel1.RowCount++;
            {
                Button button = new Button();
                button.Text = "Options";
                button.Click += Button_Click;
                callbacks[button] = formName;
                tablePanel1.addRow(name, description, button);
            }
        }

        private void Button_Click(object sender, EventArgs e)
        {
            propManager.showForm(callbacks[(Button)sender]);
        }
    }
}
