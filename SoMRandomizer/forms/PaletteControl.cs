using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SoMRandomizer.processing.common.structure;
using SoMRandomizer.util;

namespace SoMRandomizer.forms
{
    /// <summary>
    /// This is super old code from the old mana editor, but it's used here to provide a palette editor for character colors.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public partial class PaletteControl : UserControl
    {
        public delegate void PalIndexChangedHandler(int index);
        public delegate void WorkingColorModifiedHandler(int r, int g, int b);
        public delegate void PaletteModifiedHandler();

        public event PalIndexChangedHandler PalIndexChanged;
        public event WorkingColorModifiedHandler WorkingColorModified;
        public event PaletteModifiedHandler PaletteModified;

        bool editable = false;
        public SnesPalette pal;
        public int selectedIndex = 0;
        List<int> indexes = new List<int>();
        int transparentIndex = -1;
        public PaletteControl()
        {
            InitializeComponent();
            setColorBits(4);
            setPalette(new SnesPalette());
            pictureBox2.MouseClick += new MouseEventHandler(pictureBox2_MouseClick);
        }

        private void setColorBits(int bits)
        {
            int minColor = 0;
            int maxColor = (int)(Math.Pow(2, bits) - 1);
            indexes.Clear();
            for (int i = minColor; i <= maxColor; i++)
            {
                indexes.Add(i);
            }
        }

        public void setTransparentIndex(int index)
        {
            // optional and default to -1 so it effectively doesn't exist
            transparentIndex = index;
        }

        public void setPalette(SnesPalette pal)
        {
            pictureBox2.Image = new Bitmap(pictureBox2.Width, pictureBox2.Height);
            pictureBox5.Image = new Bitmap(pictureBox5.Width, pictureBox5.Height);
            this.pal = pal;
            if (pal != null)
            {
                SnesColor col = pal.getColor(selectedIndex);
                numericUpDown1.Value = col.getRed();
                numericUpDown2.Value = col.getGreen();
                numericUpDown3.Value = col.getBlue();
            }
            Draw();
        }

        void pictureBox2_MouseClick(object sender, MouseEventArgs e)
        {
            int x = e.X;
            int y = e.Y;
            int index_x = (int)((((double)x) / pictureBox2.Width) * 8);
            index_x = DataUtil.clampToEndpoints(index_x, 0, 7);
            int index_y = (int)((((double)y) / pictureBox2.Height) * 2);
            index_y = DataUtil.clampToEndpoints(index_y, 0, 1);
            int selIndex = index_y * 8 + index_x;
            if (indexes.Contains(selIndex))
                selectedIndex = selIndex;

            if (editable)
            {
                SnesColor col = pal.getColor(selectedIndex);
                numericUpDown1.Value = col.getRed();
                numericUpDown2.Value = col.getGreen();
                numericUpDown3.Value = col.getBlue();
            }

            PalIndexChanged?.Invoke(selectedIndex);
            Draw();
        }

        public void Draw()
        {
            if (pal != null)
            {
                Graphics g = Graphics.FromImage(pictureBox2.Image);
                Rectangle selected = new Rectangle();
                g.FillRectangle(new SolidBrush(BackColor), 0, 0, pictureBox2.Width, pictureBox2.Height);
                if (!indexes.Contains(selectedIndex))
                {
                    selectedIndex = indexes[0];
                }

                for (int i = 0; i < 16; i++)
                {
                    if (indexes.Contains(i))
                    {
                        int xleft = ((i % 8) * pictureBox2.Width) / 8;
                        int xright = (((i % 8) + 1) * pictureBox2.Width) / 8;
                        int ytop = ((i / 8) * pictureBox2.Height) / 2;
                        int ybottom = (((i / 8) + 1) * pictureBox2.Height) / 2;
                        Rectangle rect = new Rectangle(xleft, ytop, xright - xleft, ybottom - ytop);
                        if (i == selectedIndex)
                        {
                            selected = new Rectangle(xleft+1, ytop+1, xright - xleft - 3, ybottom - ytop - 3);
                        }
                        SnesColor col = pal.getColor(i);
                        g.FillRectangle(new SolidBrush(Color.FromArgb(col.getRed(), col.getGreen(), col.getBlue())), rect);
                    }
                }
                for (int i = 0; i < 16; i++)
                {
                    if (indexes.Contains(i))
                    {
                        int xleft = ((i % 8) * pictureBox2.Width) / 8;
                        int xright = (((i % 8) + 1) * pictureBox2.Width) / 8;
                        int ytop = ((i / 8) * pictureBox2.Height) / 2;
                        int ybottom = (((i / 8) + 1) * pictureBox2.Height) / 2;
                        Rectangle rect = new Rectangle(xleft, ytop, xright - xleft - 1, ybottom - ytop - 1);
                        g.DrawRectangle(new Pen(Color.Black), rect);
                    }
                }

                SnesColor selectedCol = pal.getColor(selectedIndex);
                g.DrawRectangle(new Pen(Color.FromArgb(selectedCol.getRed() / 4, selectedCol.getGreen() / 4, selectedCol.getBlue() / 4)), selected);
                g.DrawRectangle(new Pen(Color.FromArgb(selectedCol.getRed() / 2, selectedCol.getGreen() / 2, selectedCol.getBlue() / 2)), selected.Left + 1, selected.Top + 1, selected.Width - 2, selected.Height - 2);
                if (indexes.Contains(transparentIndex))
                {
                    int xleft = ((transparentIndex % 8) * pictureBox2.Width) / 8;
                    int xright = (((transparentIndex % 8) + 1) * pictureBox2.Width) / 8;
                    int ytop = ((transparentIndex / 8) * pictureBox2.Height) / 2;
                    int ybottom = (((transparentIndex / 8) + 1) * pictureBox2.Height) / 2;
                    g.DrawLine(new Pen(Color.Black), xleft, ytop, xright - 1, ybottom - 1);
                    g.DrawLine(new Pen(Color.Black), xright - 1, ytop, xleft, ybottom - 1);
                }
                pictureBox2.Refresh();

                Graphics g2 = Graphics.FromImage(pictureBox5.Image);
                Rectangle rect2 = new Rectangle(0, 0, pictureBox5.Width, pictureBox5.Height);
                g2.FillRectangle(new SolidBrush(Color.FromArgb(selectedCol.getRed(), selectedCol.getGreen(), selectedCol.getBlue())), rect2);
                pictureBox5.Refresh();

                label9.Text = "Current: " + (selectedIndex == transparentIndex ? "BG" : selectedIndex.ToString());

                if (selectedIndex == transparentIndex)
                {
                    numericUpDown1.Visible = false;
                    numericUpDown2.Visible = false;
                    numericUpDown3.Visible = false;
                    redLabel.Visible = false;
                    greenLabel.Visible = false;
                    blueLabel.Visible = false;
                    okButton.Visible = false;
                }
                else if (editable)
                {
                    numericUpDown1.Visible = true;
                    numericUpDown2.Visible = true;
                    numericUpDown3.Visible = true;
                    redLabel.Visible = true;
                    greenLabel.Visible = true;
                    blueLabel.Visible = true;
                    okButton.Visible = true;
                }
            }
        }

        public void setEditable()
        {
            editable = true;
            numericUpDown1.Visible = true;
            numericUpDown2.Visible = true;
            numericUpDown3.Visible = true;
            redLabel.Visible = true;
            greenLabel.Visible = true;
            blueLabel.Visible = true;
            okButton.Visible = true;
        }

        public void setColor(int red, int green, int blue)
        {
            numericUpDown1.Value = red;
            numericUpDown2.Value = green;
            numericUpDown3.Value = blue;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SnesColor newCol = new SnesColor((byte)numericUpDown1.Value, (byte)numericUpDown2.Value, (byte)numericUpDown3.Value);
            pal.setColor(selectedIndex, newCol);
            PaletteModified?.Invoke();
            Draw();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            fixColors();
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            fixColors();
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            fixColors();
        }

        private void fixColors()
        {
            // ensure entries are a multiple of 8
            byte r = (byte)((int)numericUpDown1.Value & 0xF8);
            byte g = (byte)((int)numericUpDown2.Value & 0xF8);
            byte b = (byte)((int)numericUpDown3.Value & 0xF8);

            Graphics g2 = Graphics.FromImage(pictureBox5.Image);
            Rectangle rect2 = new Rectangle(0, 0, pictureBox5.Width, pictureBox5.Height);
            g2.FillRectangle(new SolidBrush(Color.FromArgb(r, g, b)), rect2);
            pictureBox5.Refresh();
            WorkingColorModified?.Invoke(r, g, b);
        }
    }
}
