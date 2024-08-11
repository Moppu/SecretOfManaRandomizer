namespace SoMRandomizer.forms
{
    partial class PaletteControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label9 = new System.Windows.Forms.Label();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.paletteLabel = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.redLabel = new System.Windows.Forms.Label();
            this.greenLabel = new System.Windows.Forms.Label();
            this.blueLabel = new System.Windows.Forms.Label();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown3 = new System.Windows.Forms.NumericUpDown();
            this.okButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).BeginInit();
            this.SuspendLayout();
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(135, 74);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(53, 13);
            this.label9.TabIndex = 93;
            this.label9.Text = "Current: 0";
            // 
            // pictureBox5
            // 
            this.pictureBox5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox5.Location = new System.Drawing.Point(146, 3);
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Size = new System.Drawing.Size(32, 64);
            this.pictureBox5.TabIndex = 92;
            this.pictureBox5.TabStop = false;
            // 
            // paletteLabel
            // 
            this.paletteLabel.AutoSize = true;
            this.paletteLabel.Location = new System.Drawing.Point(47, 74);
            this.paletteLabel.Name = "paletteLabel";
            this.paletteLabel.Size = new System.Drawing.Size(40, 13);
            this.paletteLabel.TabIndex = 91;
            this.paletteLabel.Text = "Palette";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Location = new System.Drawing.Point(3, 3);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(128, 64);
            this.pictureBox2.TabIndex = 90;
            this.pictureBox2.TabStop = false;
            // 
            // redLabel
            // 
            this.redLabel.Location = new System.Drawing.Point(53, 100);
            this.redLabel.Name = "redLabel";
            this.redLabel.Size = new System.Drawing.Size(78, 13);
            this.redLabel.TabIndex = 94;
            this.redLabel.Text = "Red:";
            this.redLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.redLabel.Visible = false;
            // 
            // greenLabel
            // 
            this.greenLabel.Location = new System.Drawing.Point(50, 122);
            this.greenLabel.Name = "greenLabel";
            this.greenLabel.Size = new System.Drawing.Size(81, 13);
            this.greenLabel.TabIndex = 95;
            this.greenLabel.Text = "Green:";
            this.greenLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.greenLabel.Visible = false;
            // 
            // blueLabel
            // 
            this.blueLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.blueLabel.Location = new System.Drawing.Point(78, 145);
            this.blueLabel.Name = "blueLabel";
            this.blueLabel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.blueLabel.Size = new System.Drawing.Size(53, 13);
            this.blueLabel.TabIndex = 96;
            this.blueLabel.Text = "Blue:";
            this.blueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.blueLabel.Visible = false;
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Increment = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.numericUpDown1.Location = new System.Drawing.Point(138, 96);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(62, 20);
            this.numericUpDown1.TabIndex = 97;
            this.numericUpDown1.Visible = false;
            this.numericUpDown1.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // numericUpDown2
            // 
            this.numericUpDown2.Increment = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.numericUpDown2.Location = new System.Drawing.Point(138, 119);
            this.numericUpDown2.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericUpDown2.Name = "numericUpDown2";
            this.numericUpDown2.Size = new System.Drawing.Size(62, 20);
            this.numericUpDown2.TabIndex = 98;
            this.numericUpDown2.Visible = false;
            this.numericUpDown2.ValueChanged += new System.EventHandler(this.numericUpDown2_ValueChanged);
            // 
            // numericUpDown3
            // 
            this.numericUpDown3.Increment = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.numericUpDown3.Location = new System.Drawing.Point(138, 142);
            this.numericUpDown3.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericUpDown3.Name = "numericUpDown3";
            this.numericUpDown3.Size = new System.Drawing.Size(62, 20);
            this.numericUpDown3.TabIndex = 99;
            this.numericUpDown3.Visible = false;
            this.numericUpDown3.ValueChanged += new System.EventHandler(this.numericUpDown3_ValueChanged);
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(138, 169);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(62, 23);
            this.okButton.TabIndex = 100;
            this.okButton.Text = "Ok";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Visible = false;
            this.okButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // PaletteControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.numericUpDown3);
            this.Controls.Add(this.numericUpDown2);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.blueLabel);
            this.Controls.Add(this.greenLabel);
            this.Controls.Add(this.redLabel);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.pictureBox5);
            this.Controls.Add(this.paletteLabel);
            this.Controls.Add(this.pictureBox2);
            this.Name = "PaletteControl";
            this.Size = new System.Drawing.Size(215, 207);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.PictureBox pictureBox5;
        private System.Windows.Forms.Label paletteLabel;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label redLabel;
        private System.Windows.Forms.Label greenLabel;
        private System.Windows.Forms.Label blueLabel;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.NumericUpDown numericUpDown2;
        private System.Windows.Forms.NumericUpDown numericUpDown3;
        private System.Windows.Forms.Button okButton;
    }
}
