using SoMRandomizer.config;
using SoMRandomizer.processing.common.structure;
using SoMRandomizer.util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SoMRandomizer.forms
{
    /// <summary>
    /// Palette picker for playable characters.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public partial class CharacterDesigner : Form
    {
        private SnesPalette boyDefault;
        private SnesPalette girlDefault;
        private SnesPalette spriteDefault;

        public SnesPalette boyCurrent;
        public SnesPalette girlCurrent;
        public SnesPalette spriteCurrent;

        public CharacterDesigner()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 2;

            // vanilla palettes
            boyDefault = new SnesPalette(new byte[] { 0xff, 0x7f, 0xa6, 0x10, 0x10, 0x0d, 0x79, 0x11, 0x7f, 0x12, 0x51, 0x1d, 0xfa, 0x29, 0xfe, 0x3e, 0xbd, 0x54, 0xdf, 0x52, 0x24, 0x39, 0x47, 0x62, 0x91, 0x7b, 0x1b, 0x25, 0xbf, 0x16, 0xff, 0x7f });
            girlDefault = new SnesPalette(new byte[] { 0xff, 0x7f, 0xe9, 0x18, 0x74, 0x1d, 0xfb, 0x01, 0xbe, 0x12, 0x55, 0x1d, 0x1b, 0x2e, 0x7f, 0x57, 0xaa, 0x36, 0xb3, 0x57, 0xb4, 0x38, 0x5d, 0x5d, 0x5f, 0x76, 0x7f, 0x53, 0x60, 0x39, 0xde, 0x7b });
            spriteDefault = new SnesPalette(new byte[] { 0xff, 0x7f, 0xc7, 0x14, 0xd5, 0x20, 0x7a, 0x35, 0x5d, 0x3e, 0x91, 0x15, 0x3a, 0x3a, 0x1f, 0x57, 0x1f, 0x43, 0x72, 0x0c, 0xe4, 0x2d, 0xca, 0x36, 0xee, 0x5b, 0x38, 0x55, 0xdf, 0x7a, 0xff, 0x7f });

            boyCurrent = new SnesPalette(boyDefault);
            girlCurrent = new SnesPalette(girlDefault);
            spriteCurrent = new SnesPalette(spriteDefault);

            paletteControl1.PaletteModified += paletteModified;
            paletteControl1.setPalette(boyCurrent);
            FormClosing += MyForm_FormClosing;
            reDraw();
            paletteControl1.setEditable();
        }

        private void reDraw()
        {
            int charIndex = comboBox1.SelectedIndex;
            int bgIndex = comboBox2.SelectedIndex;
            if (charIndex < 0 || bgIndex < 0)
            {
                return;
            }
            Bitmap newBitmap = null;
            Bitmap bgBitmap = null;
            SnesPalette defaultPalette = null;
            SnesPalette currentPalette = null;

            switch (charIndex)
            {
                case 0:
                    newBitmap = new Bitmap(SoMRandomizer.Properties.Resources.sprite_boy);
                    defaultPalette = boyDefault;
                    currentPalette = boyCurrent;
                    break;
                case 1:
                    newBitmap = new Bitmap(SoMRandomizer.Properties.Resources.sprite_girl);
                    defaultPalette = girlDefault;
                    currentPalette = girlCurrent;
                    break;
                case 2:
                    newBitmap = new Bitmap(SoMRandomizer.Properties.Resources.sprite_sprite);
                    defaultPalette = spriteDefault;
                    currentPalette = spriteCurrent;
                    break;
            }

            switch(bgIndex)
            {
                case 2:
                    bgBitmap = new Bitmap(SoMRandomizer.Properties.Resources.backdrop_forest);
                    break;
                case 3:
                    bgBitmap = new Bitmap(SoMRandomizer.Properties.Resources.backdrop_cave);
                    break;
                case 4:
                    bgBitmap = new Bitmap(SoMRandomizer.Properties.Resources.backdrop_ruins);
                    break;
                case 5:
                    bgBitmap = new Bitmap(SoMRandomizer.Properties.Resources.backdrop_castle);
                    break;
            }

            if (currentPalette == null)
            {
                return;
            }

            for (int y=0; y < newBitmap.Height; y++)
            {
                for (int x = 0; x < newBitmap.Width; x++)
                {
                    Color baseColor = newBitmap.GetPixel(x, y);
                    int matchIndex = 0;
                    for(int i=1; i < 16 && matchIndex == 0; i++)
                    {
                        SnesColor col = defaultPalette.getColor(i);
                        if (col.getRed() == baseColor.R &&
                            col.getGreen() == baseColor.G &&
                            col.getBlue() == baseColor.B)
                        {
                            matchIndex = i;
                        }
                    }

                    if(matchIndex == 0)
                    {
                        if(bgBitmap == null)
                        {
                            if(bgIndex == 0)
                            {
                                newBitmap.SetPixel(x, y, Color.Black);
                            }
                            else
                            {
                                newBitmap.SetPixel(x, y, Color.White);
                            }
                        }
                        else
                        {
                            newBitmap.SetPixel(x, y, bgBitmap.GetPixel(x, y));
                        }
                    }
                    else
                    {
                        SnesColor newCol = currentPalette.getColor(matchIndex);
                        Color newColor = Color.FromArgb(newCol.getRed(), newCol.getGreen(), newCol.getBlue());
                        newBitmap.SetPixel(x, y, newColor);
                    }
                }
            }

            pictureBox1.Image = newBitmap;
        }

        public void paletteModified()
        {
            int charIndex = comboBox1.SelectedIndex;
            SnesPalette currentPalette = null;
            if (charIndex < 0)
            {
                return;
            }

            switch(charIndex)
            {
                case 0:
                    currentPalette = boyCurrent;
                    break;
                case 1:
                    currentPalette = girlCurrent;
                    break;
                case 2:
                    currentPalette = spriteCurrent;
                    break;
            }

            currentPalette.copyFrom(paletteControl1.pal);
            reDraw();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int charIndex = comboBox1.SelectedIndex;
            SnesPalette currentPalette = null;
            if (charIndex < 0)
            {
                return;
            }

            switch (charIndex)
            {
                case 0:
                    currentPalette = boyCurrent;
                    break;
                case 1:
                    currentPalette = girlCurrent;
                    break;
                case 2:
                    currentPalette = spriteCurrent;
                    break;
            }

            if(currentPalette == null)
            {
                return;
            }
            paletteControl1.setPalette(currentPalette);

            reDraw();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            reDraw();
        }

        private void MyForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            e.Cancel = true; // this cancels the close event.
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // save
            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = "Properties File (*.properties)|*.properties";
            DialogResult d = sf.ShowDialog();
            if (d == DialogResult.OK)
            {
                string filename = sf.FileName;
                try
                {
                    StreamWriter fileOut = new StreamWriter(filename);

                    for (int i = 1; i < 16; i++)
                    {
                        SnesColor boyColor = boyCurrent.getColor(i);
                        SnesColor girlColor = girlCurrent.getColor(i);
                        SnesColor spriteColor = spriteCurrent.getColor(i);
                        fileOut.WriteLine("boyR" + i + "=" + boyColor.getRed());
                        fileOut.WriteLine("boyG" + i + "=" + boyColor.getGreen());
                        fileOut.WriteLine("boyB" + i + "=" + boyColor.getBlue());
                        fileOut.WriteLine("girlR" + i + "=" + girlColor.getRed());
                        fileOut.WriteLine("girlG" + i + "=" + girlColor.getGreen());
                        fileOut.WriteLine("girlB" + i + "=" + girlColor.getBlue());
                        fileOut.WriteLine("spriteR" + i + "=" + spriteColor.getRed());
                        fileOut.WriteLine("spriteG" + i + "=" + spriteColor.getGreen());
                        fileOut.WriteLine("spriteB" + i + "=" + spriteColor.getBlue());
                    }
                    
                    fileOut.Close();
                }
                catch (Exception ee)
                {
                    MessageBox.Show("Unable to write to file.");
                }
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            // load
            OpenFileDialog sf = new OpenFileDialog();
            sf.Filter = "Properties File (*.properties)|*.properties";
            sf.Multiselect = false;
            DialogResult d = sf.ShowDialog();
            if (d == DialogResult.OK)
            {
                string filename = sf.FileName;
                try
                {
                    PropertyFileReader pfr = new PropertyFileReader();
                    Dictionary<string, string> properties = pfr.readFile(filename);

                    for(int i=1; i < 16; i++)
                    {
                        boyCurrent.setColor(i, new SnesColor(Int32.Parse(properties["boyR" + i]), Int32.Parse(properties["boyG" + i]), Int32.Parse(properties["boyB" + i])));
                        girlCurrent.setColor(i, new SnesColor(Int32.Parse(properties["girlR" + i]), Int32.Parse(properties["girlG" + i]), Int32.Parse(properties["girlB" + i])));
                        spriteCurrent.setColor(i, new SnesColor(Int32.Parse(properties["spriteR" + i]), Int32.Parse(properties["spriteG" + i]), Int32.Parse(properties["spriteB" + i])));
                    }
                }
                catch (Exception ee)
                {
                    MessageBox.Show("Unable to read from file.  Verify all properties exist!");
                }
            }

            comboBox1_SelectedIndexChanged(null, null);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // swap r/g
            bool allColors = false;
            if(radioButton2.Checked)
            {
                allColors = true;
            }

            SnesPalette currentPal = paletteControl1.pal;

            if(allColors)
            {
                for(int i=1; i < 16; i++)
                {
                    SnesColor col = currentPal.getColor(i);
                    byte r = currentPal.getColor(i).getRed();
                    col.setRed(col.getGreen());
                    col.setGreen(r);
                    currentPal.setColor(i, col);
                }
            }
            else
            {
                int i = paletteControl1.selectedIndex;
                SnesColor col = currentPal.getColor(i);
                byte r = currentPal.getColor(i).getRed();
                col.setRed(col.getGreen());
                col.setGreen(r);
                currentPal.setColor(i, col);
            }

            paletteControl1.setPalette(currentPal);
            reDraw();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // swap g/b
            bool allColors = false;
            if (radioButton2.Checked)
            {
                allColors = true;
            }

            SnesPalette currentPal = paletteControl1.pal;

            if (allColors)
            {
                for (int i = 1; i < 16; i++)
                {
                    SnesColor col = currentPal.getColor(i);
                    byte g = currentPal.getColor(i).getGreen();
                    col.setGreen(col.getBlue());
                    col.setBlue(g);
                    currentPal.setColor(i, col);
                }
            }
            else
            {
                int i = paletteControl1.selectedIndex;
                SnesColor col = currentPal.getColor(i);
                byte g = currentPal.getColor(i).getGreen();
                col.setGreen(col.getBlue());
                col.setBlue(g);
                currentPal.setColor(i, col);
            }

            paletteControl1.setPalette(currentPal);
            reDraw();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // swap r/b
            bool allColors = false;
            if (radioButton2.Checked)
            {
                allColors = true;
            }

            SnesPalette currentPal = paletteControl1.pal;

            if (allColors)
            {
                for (int i = 1; i < 16; i++)
                {
                    SnesColor col = currentPal.getColor(i);
                    byte r = currentPal.getColor(i).getRed();
                    col.setRed(col.getBlue());
                    col.setBlue(r);
                    currentPal.setColor(i, col);
                }
            }
            else
            {
                int i = paletteControl1.selectedIndex;
                SnesColor col = currentPal.getColor(i);
                byte r = currentPal.getColor(i).getRed();
                col.setRed(col.getBlue());
                col.setBlue(r);
                currentPal.setColor(i, col);
            }

            paletteControl1.setPalette(currentPal);
            reDraw();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            List<int> indexes = new List<int>();
            int amount = (int)numericUpDown1.Value;
            if (radioButton2.Checked)
            {
                for (int i = 1; i < 16; i++)
                {
                    indexes.Add(i);
                }
            }
            else
            {
                indexes.Add(paletteControl1.selectedIndex);
            }

            SnesPalette currentPal = paletteControl1.pal;

            foreach(int i in indexes)
            {
                SnesColor col = currentPal.getColor(i);
                Color c = Color.FromArgb(col.getRed(), col.getGreen(), col.getBlue());
                double h, s, v;
                ColorUtil.rgbToHsv(c.R, c.G, c.B, out h, out s, out v);
                v += (amount / 100.0) * v;
                int r, g, b;
                ColorUtil.hsvToRgb(h, s, v, out r, out g, out b);
                SnesColor newCol = new SnesColor(r, g, b);
                currentPal.setColor(i, newCol);
            }

            paletteControl1.setPalette(currentPal);
            reDraw();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            List<int> indexes = new List<int>();
            int amount = (int)numericUpDown2.Value;
            if (radioButton2.Checked)
            {
                for (int i = 1; i < 16; i++)
                {
                    indexes.Add(i);
                }
            }
            else
            {
                indexes.Add(paletteControl1.selectedIndex);
            }

            SnesPalette currentPal = paletteControl1.pal;

            foreach (int i in indexes)
            {
                SnesColor col = currentPal.getColor(i);
                Color c = Color.FromArgb(col.getRed(), col.getGreen(), col.getBlue());
                double h, s, v;
                ColorUtil.rgbToHsv(c.R, c.G, c.B, out h, out s, out v);
                s += (amount / 100.0) * s;
                int r, g, b;
                ColorUtil.hsvToRgb(h, s, v, out r, out g, out b);
                SnesColor newCol = new SnesColor(r, g, b);
                currentPal.setColor(i, newCol);
            }

            paletteControl1.setPalette(currentPal);
            reDraw();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            List<int> indexes = new List<int>();
            int amount = (int)numericUpDown3.Value;
            if (radioButton2.Checked)
            {
                for (int i = 1; i < 16; i++)
                {
                    indexes.Add(i);
                }
            }
            else
            {
                indexes.Add(paletteControl1.selectedIndex);
            }

            SnesPalette currentPal = paletteControl1.pal;

            foreach (int i in indexes)
            {
                SnesColor col = currentPal.getColor(i);
                Color c = Color.FromArgb(col.getRed(), col.getGreen(), col.getBlue());
                double h, s, v;
                ColorUtil.rgbToHsv(c.R, c.G, c.B, out h, out s, out v);
                h += (amount / 100.0) * h;
                int r, g, b;
                ColorUtil.hsvToRgb(h, s, v, out r, out g, out b);
                SnesColor newCol = new SnesColor(r, g, b);
                currentPal.setColor(i, newCol);
            }

            paletteControl1.setPalette(currentPal);
            reDraw();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            boyCurrent.copyFrom(boyDefault);
            girlCurrent.copyFrom(girlDefault);
            spriteCurrent.copyFrom(spriteDefault);
            comboBox1_SelectedIndexChanged(null, null);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Multiselect = false;
            DialogResult dr = of.ShowDialog();
            int charIndex = comboBox1.SelectedIndex;
            if (dr == DialogResult.OK)
            {
                string filename = of.FileName;
                byte[] thisRom = File.ReadAllBytes(filename);
                byte[] palData = new byte[32];
                for(int i=0; i < 32; i++)
                {
                    palData[i] = thisRom[0x80FFE + (0x80 + charIndex) * 0x1E + i];
                }
                if (charIndex == 0)
                {
                    boyCurrent = new SnesPalette(palData);
                }
                else if(charIndex == 1)
                {
                    girlCurrent = new SnesPalette(palData);
                }
                else if(charIndex == 2)
                {
                    spriteCurrent = new SnesPalette(palData);
                }
                reDraw();
            }
        }
    }
}
