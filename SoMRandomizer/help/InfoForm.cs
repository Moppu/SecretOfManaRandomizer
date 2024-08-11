using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SoMRandomizer.help
{
    /// <summary>
    /// A little documentation form for help on each game mode.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public partial class InfoForm : Form
    {
        Dictionary<Control, int> headerYPositions = new Dictionary<Control, int>();
        string title;
        Control titleControl;
        List<Control> subtitleControls;
        List<Control> headerControls;
        List<Control> controls;
        int windowWidth = 0;
        int windowHeight = 0;

        public InfoForm(string title, Control titleControl, List<Control> subtitleControls, List<Control> headerControls, List<Control> controls)
        {
            InitializeComponent();
            this.title = title;
            this.titleControl = titleControl;
            this.subtitleControls = subtitleControls;
            this.headerControls = headerControls;
            this.controls = controls;

            Text = title;

            windowWidth = Width;
            windowHeight = Height;
            reRender();
        }

        private void reRender()
        {
            Controls.Clear();
            int yPos = 10;
            addControl(titleControl, ref yPos);
            foreach(Control c in subtitleControls)
            {
                addControl(c, ref yPos);
            }
            headerYPositions.Clear();
            Dictionary<Control, Control> sourceToLink = new Dictionary<Control, Control>();
            foreach (Control c in headerControls)
            {
                LinkLabel link = new LinkLabel();
                link.Text = c.Text.Replace("\n", "");
                int dotPos = c.Text.IndexOf(".");
                int xPos = 10;
                if(dotPos >= 0 && dotPos < c.Text.Length - 1)
                {
                    string nextChar = "" + c.Text[dotPos + 1];
                    int o;
                    if(Int32.TryParse(nextChar, out o))
                    {
                        xPos += 20;
                    }
                }
                link.AutoSize = true;
                addControl(link, xPos, ref yPos);
                link.Click += Link_Click;
                sourceToLink[c] = link;
            }

            foreach (Control c in controls)
            {
                if (headerControls.Contains(c))
                {
                    Control link = sourceToLink[c];
                    headerYPositions[link] = yPos;
                }
                addControl(c, ref yPos);
            }
            VisibleChanged += InfoForm_VisibleChanged;
            FormClosing += MyForm_FormClosing;
            ResizeEnd += InfoForm_ResizeEnd;
        }

        private void InfoForm_VisibleChanged(object sender, EventArgs e)
        {
            // required to make window moving not trigger the resize event.  dumb
            windowWidth = Width;
            windowHeight = Height;
        }


        private void InfoForm_ResizeEnd(object sender, EventArgs e)
        {
            // for some reason this gets called more and more times as i move the window? god why
            if (windowWidth != Width || windowHeight != Height)
            {
                windowWidth = Width;
                windowHeight = Height;
                reRender();
            }
        }

        private void Link_Click(object sender, EventArgs e)
        {
            // if i don't set this twice the goddamn scrollbar doesn't move. idk why
            VerticalScroll.Value = headerYPositions[(Control)sender];
            VerticalScroll.Value = headerYPositions[(Control)sender];
        }

        private void addControl(Control c, ref int yPos)
        {
            c.Location = new Point(10, yPos);
            Controls.Add(c);
            yPos += c.Height + 5;
        }
        private void addControl(Control c, int xPos, ref int yPos)
        {
            c.Location = new Point(xPos, yPos);
            Controls.Add(c);
            yPos += c.Height + 5;
        }
        private void MyForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            e.Cancel = true; // this cancels the close event.
        }
    }
}
