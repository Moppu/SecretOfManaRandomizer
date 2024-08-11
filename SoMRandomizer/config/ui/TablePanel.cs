using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SoMRandomizer.config.ui
{
    /// <summary>
    /// 3-column table used for selection of rando property values.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public partial class TablePanel : Panel
    {
        private int bufferPx = 20;
        private int yPos = 20;
        private int col1Width = 150;
        public int col3Width = 150;
        private int scrollbarSize = 20;

        private List<Label> headerLabels = new List<Label>();

        private List<Label> nameLabels = new List<Label>();
        private List<Label> descLabels = new List<Label>();
        private List<Control> rightColComponents = new List<Control>();
        private List<int> rowType = new List<int>();
        public TablePanel()
        {
            InitializeComponent();
            SizeChanged += TablePanel_SizeChanged;
            HorizontalScroll.Maximum = 0;
            AutoScroll = false;
            VerticalScroll.Visible = false;
            AutoScroll = true;
        }

        private void TablePanel_SizeChanged(object sender, EventArgs e)
        {
            // re-determine y positions
            yPos = bufferPx;
            foreach (Label headerLabel in headerLabels)
            {
                headerLabel.Size = new Size(Width - scrollbarSize - bufferPx, headerLabel.Height);
                headerLabel.MaximumSize = new Size(Width - scrollbarSize - bufferPx, 0);
            }
            // first set sizes
            foreach(Label descLabel in descLabels)
            {
                descLabel.Size = new Size(Width - col3Width - col1Width, descLabel.Height);
                descLabel.MaximumSize = new Size(Width - col3Width - col1Width, 0);
            }
            foreach (Control control in rightColComponents)
            {
                control.Location = new System.Drawing.Point(Width - control.Width - scrollbarSize, control.Location.Y);
            }

            // now set locations based on sizes
            int i0 = 0;
            int i1 = 0;
            for (int i=0; i < rowType.Count; i++)
            {
                if(rowType[i] == 0)
                {
                    // header
                    Label headerLabel = headerLabels[i0];
                    headerLabel.Location = new Point(headerLabel.Location.X, yPos);
                    yPos += headerLabel.Height + bufferPx - 10;
                    i0++;
                }
                else
                {
                    Label nameLabel = nameLabels[i1];
                    nameLabel.Location = new Point(nameLabel.Location.X, yPos);
                    Label descLabel = descLabels[i1];
                    descLabel.Location = new Point(descLabel.Location.X, yPos);
                    Control c = rightColComponents[i1];
                    c.Location = new Point(c.Location.X, yPos);
                    int height = nameLabel.Height;
                    height = Math.Max(height, descLabel.Height);
                    height = Math.Max(height, c.Height);
                    yPos += height + bufferPx;
                    i1++;
                }
            }
        }

        public TablePanel(IContainer container)
        {
            container.Add(this);

            InitializeComponent();

            SizeChanged += TablePanel_SizeChanged;
            HorizontalScroll.Maximum = 0;
            AutoScroll = false;
            VerticalScroll.Visible = false;
            AutoScroll = true;
        }

        public void addHeader(string headerLine, int fontSizeChange)
        {
            rowType.Add(0);
            Label l = new Label();
            l.Text = headerLine;
            l.Font = new Font(l.Font.FontFamily, l.Font.Size + fontSizeChange);
            l.ForeColor = Color.Blue;
            Controls.Add(l);
            l.Location = new System.Drawing.Point(bufferPx, yPos);
            l.Size = new Size(Width - scrollbarSize - bufferPx, l.Height);
            l.AutoSize = true;
            l.MaximumSize = new Size(Width - scrollbarSize - bufferPx, 0);
            yPos += l.Height + bufferPx - 10;
            headerLabels.Add(l);
        }

        public void addRow(string name, string description, Control control)
        {
            rowType.Add(1);
            Label l1 = new Label();
            l1.Text = name;
            Controls.Add(l1);
            l1.Location = new System.Drawing.Point(bufferPx, yPos);
            l1.MaximumSize = new Size(col1Width, 0);
            l1.Font = new Font(l1.Font, FontStyle.Bold);
            l1.AutoSize = true;
            int height = l1.Height;
            l1.Anchor = AnchorStyles.Left | AnchorStyles.Top;
            nameLabels.Add(l1);

            Label l2 = new Label();
            l2.Text = description;
            Controls.Add(l2);
            l2.Location = new System.Drawing.Point(bufferPx + col1Width, yPos);
            l2.Size = new Size(Width - col3Width - col1Width, l2.Height);
            l2.MaximumSize = new Size(Width - col3Width - col1Width, 0);
            l2.AutoSize = true;
            l2.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            height = Math.Max(height, l2.Height);
            descLabels.Add(l2);

            Controls.Add(control);
            control.Location = new System.Drawing.Point(Width - control.Width - scrollbarSize, yPos);
            control.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            height = Math.Max(height, control.Height);
            rightColComponents.Add(control);

            yPos += height + bufferPx;
        }
    }
}
