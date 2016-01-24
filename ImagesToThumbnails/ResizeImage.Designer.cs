namespace ImagesToThumbnails
{
    partial class ResizeImage
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnResize = new System.Windows.Forms.Button();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.tbOutput = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.nudThreads = new System.Windows.Forms.NumericUpDown();
            this.cbAutoCalcThreads = new System.Windows.Forms.CheckBox();
            this.gbSize = new System.Windows.Forms.GroupBox();
            this.rbSize200x200 = new System.Windows.Forms.RadioButton();
            this.rbSize100x100 = new System.Windows.Forms.RadioButton();
            this.gbFitMode = new System.Windows.Forms.GroupBox();
            this.rbFitModeStretch = new System.Windows.Forms.RadioButton();
            this.rbFitModeFitHeight = new System.Windows.Forms.RadioButton();
            this.rbFitModeFitWidth = new System.Windows.Forms.RadioButton();
            this.rbFitModeFit = new System.Windows.Forms.RadioButton();
            this.cbOverwriteExistingfiles = new System.Windows.Forms.CheckBox();
            this.btnFolder = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudThreads)).BeginInit();
            this.gbSize.SuspendLayout();
            this.gbFitMode.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnResize
            // 
            this.btnResize.Font = new System.Drawing.Font("Microsoft Sans Serif", 99.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnResize.Location = new System.Drawing.Point(13, 352);
            this.btnResize.Name = "btnResize";
            this.btnResize.Size = new System.Drawing.Size(736, 149);
            this.btnResize.TabIndex = 0;
            this.btnResize.Text = "RESIZE!";
            this.btnResize.UseVisualStyleBackColor = true;
            this.btnResize.Click += new System.EventHandler(this.btnResize_Click);
            // 
            // folderBrowserDialog
            // 
            this.folderBrowserDialog.SelectedPath = "E:\\";
            this.folderBrowserDialog.ShowNewFolderButton = false;
            // 
            // tbOutput
            // 
            this.tbOutput.BackColor = System.Drawing.SystemColors.Window;
            this.tbOutput.Location = new System.Drawing.Point(12, 19);
            this.tbOutput.Multiline = true;
            this.tbOutput.Name = "tbOutput";
            this.tbOutput.ReadOnly = true;
            this.tbOutput.Size = new System.Drawing.Size(574, 296);
            this.tbOutput.TabIndex = 1;
            this.tbOutput.TextChanged += new System.EventHandler(this.tbOutput_TextChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.AutoSize = true;
            this.groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox1.Controls.Add(this.tbOutput);
            this.groupBox1.Location = new System.Drawing.Point(163, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(592, 334);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Output";
            // 
            // nudThreads
            // 
            this.nudThreads.Enabled = false;
            this.nudThreads.Location = new System.Drawing.Point(107, 156);
            this.nudThreads.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.nudThreads.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudThreads.Name = "nudThreads";
            this.nudThreads.Size = new System.Drawing.Size(34, 20);
            this.nudThreads.TabIndex = 3;
            this.nudThreads.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // cbAutoCalcThreads
            // 
            this.cbAutoCalcThreads.AutoSize = true;
            this.cbAutoCalcThreads.Checked = true;
            this.cbAutoCalcThreads.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbAutoCalcThreads.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbAutoCalcThreads.Location = new System.Drawing.Point(13, 156);
            this.cbAutoCalcThreads.Name = "cbAutoCalcThreads";
            this.cbAutoCalcThreads.Size = new System.Drawing.Size(88, 17);
            this.cbAutoCalcThreads.TabIndex = 4;
            this.cbAutoCalcThreads.Text = "Automatically";
            this.cbAutoCalcThreads.UseVisualStyleBackColor = true;
            this.cbAutoCalcThreads.CheckedChanged += new System.EventHandler(this.cbAutoCalcThreads_CheckedChanged);
            // 
            // gbSize
            // 
            this.gbSize.Controls.Add(this.rbSize200x200);
            this.gbSize.Controls.Add(this.rbSize100x100);
            this.gbSize.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbSize.Location = new System.Drawing.Point(13, 46);
            this.gbSize.Name = "gbSize";
            this.gbSize.Size = new System.Drawing.Size(130, 88);
            this.gbSize.TabIndex = 5;
            this.gbSize.TabStop = false;
            this.gbSize.Text = "Size";
            // 
            // rbSize200x200
            // 
            this.rbSize200x200.AutoSize = true;
            this.rbSize200x200.Location = new System.Drawing.Point(7, 44);
            this.rbSize200x200.Name = "rbSize200x200";
            this.rbSize200x200.Size = new System.Drawing.Size(80, 20);
            this.rbSize200x200.TabIndex = 1;
            this.rbSize200x200.Text = "200 x 200";
            this.rbSize200x200.UseVisualStyleBackColor = true;
            // 
            // rbSize100x100
            // 
            this.rbSize100x100.AutoSize = true;
            this.rbSize100x100.Checked = true;
            this.rbSize100x100.Location = new System.Drawing.Point(9, 20);
            this.rbSize100x100.Name = "rbSize100x100";
            this.rbSize100x100.Size = new System.Drawing.Size(80, 20);
            this.rbSize100x100.TabIndex = 0;
            this.rbSize100x100.TabStop = true;
            this.rbSize100x100.Text = "100 x 100";
            this.rbSize100x100.UseVisualStyleBackColor = true;
            // 
            // gbFitMode
            // 
            this.gbFitMode.Controls.Add(this.rbFitModeStretch);
            this.gbFitMode.Controls.Add(this.rbFitModeFitHeight);
            this.gbFitMode.Controls.Add(this.rbFitModeFitWidth);
            this.gbFitMode.Controls.Add(this.rbFitModeFit);
            this.gbFitMode.Location = new System.Drawing.Point(13, 202);
            this.gbFitMode.Name = "gbFitMode";
            this.gbFitMode.Size = new System.Drawing.Size(130, 143);
            this.gbFitMode.TabIndex = 6;
            this.gbFitMode.TabStop = false;
            this.gbFitMode.Text = "Fit mode";
            // 
            // rbFitModeStretch
            // 
            this.rbFitModeStretch.AutoSize = true;
            this.rbFitModeStretch.Location = new System.Drawing.Point(16, 108);
            this.rbFitModeStretch.Name = "rbFitModeStretch";
            this.rbFitModeStretch.Size = new System.Drawing.Size(59, 17);
            this.rbFitModeStretch.TabIndex = 5;
            this.rbFitModeStretch.Text = "Stretch";
            this.rbFitModeStretch.UseVisualStyleBackColor = true;
            // 
            // rbFitModeFitHeight
            // 
            this.rbFitModeFitHeight.AutoSize = true;
            this.rbFitModeFitHeight.Location = new System.Drawing.Point(16, 85);
            this.rbFitModeFitHeight.Name = "rbFitModeFitHeight";
            this.rbFitModeFitHeight.Size = new System.Drawing.Size(68, 17);
            this.rbFitModeFitHeight.TabIndex = 4;
            this.rbFitModeFitHeight.Text = "Fit height";
            this.rbFitModeFitHeight.UseVisualStyleBackColor = true;
            // 
            // rbFitModeFitWidth
            // 
            this.rbFitModeFitWidth.AutoSize = true;
            this.rbFitModeFitWidth.Location = new System.Drawing.Point(16, 62);
            this.rbFitModeFitWidth.Name = "rbFitModeFitWidth";
            this.rbFitModeFitWidth.Size = new System.Drawing.Size(64, 17);
            this.rbFitModeFitWidth.TabIndex = 3;
            this.rbFitModeFitWidth.Text = "Fit width";
            this.rbFitModeFitWidth.UseVisualStyleBackColor = true;
            // 
            // rbFitModeFit
            // 
            this.rbFitModeFit.AutoSize = true;
            this.rbFitModeFit.Checked = true;
            this.rbFitModeFit.Location = new System.Drawing.Point(16, 41);
            this.rbFitModeFit.Name = "rbFitModeFit";
            this.rbFitModeFit.Size = new System.Drawing.Size(36, 17);
            this.rbFitModeFit.TabIndex = 2;
            this.rbFitModeFit.TabStop = true;
            this.rbFitModeFit.Text = "Fit";
            this.rbFitModeFit.UseVisualStyleBackColor = true;
            // 
            // cbOverwriteExistingfiles
            // 
            this.cbOverwriteExistingfiles.AutoSize = true;
            this.cbOverwriteExistingfiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbOverwriteExistingfiles.Location = new System.Drawing.Point(13, 182);
            this.cbOverwriteExistingfiles.Name = "cbOverwriteExistingfiles";
            this.cbOverwriteExistingfiles.Size = new System.Drawing.Size(130, 17);
            this.cbOverwriteExistingfiles.TabIndex = 7;
            this.cbOverwriteExistingfiles.Text = "Overwrite existing files";
            this.cbOverwriteExistingfiles.UseVisualStyleBackColor = true;
            // 
            // btnFolder
            // 
            this.btnFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFolder.Location = new System.Drawing.Point(13, 12);
            this.btnFolder.Name = "btnFolder";
            this.btnFolder.Size = new System.Drawing.Size(130, 28);
            this.btnFolder.TabIndex = 8;
            this.btnFolder.Text = "Select folder";
            this.btnFolder.UseVisualStyleBackColor = true;
            this.btnFolder.Click += new System.EventHandler(this.btnFolder_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(10, 137);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(118, 16);
            this.label1.TabIndex = 9;
            this.label1.Text = "Number of threads";
            // 
            // ResizeImage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(763, 513);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnFolder);
            this.Controls.Add(this.cbOverwriteExistingfiles);
            this.Controls.Add(this.gbFitMode);
            this.Controls.Add(this.gbSize);
            this.Controls.Add(this.cbAutoCalcThreads);
            this.Controls.Add(this.nudThreads);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnResize);
            this.MaximizeBox = false;
            this.Name = "ResizeImage";
            this.Text = "ResizeImage";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudThreads)).EndInit();
            this.gbSize.ResumeLayout(false);
            this.gbSize.PerformLayout();
            this.gbFitMode.ResumeLayout(false);
            this.gbFitMode.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnResize;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.TextBox tbOutput;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown nudThreads;
        private System.Windows.Forms.CheckBox cbAutoCalcThreads;
        private System.Windows.Forms.GroupBox gbSize;
        private System.Windows.Forms.RadioButton rbSize200x200;
        private System.Windows.Forms.RadioButton rbSize100x100;
        private System.Windows.Forms.GroupBox gbFitMode;
        private System.Windows.Forms.RadioButton rbFitModeStretch;
        private System.Windows.Forms.RadioButton rbFitModeFitHeight;
        private System.Windows.Forms.RadioButton rbFitModeFitWidth;
        private System.Windows.Forms.RadioButton rbFitModeFit;
        private System.Windows.Forms.CheckBox cbOverwriteExistingfiles;
        private System.Windows.Forms.Button btnFolder;
        private System.Windows.Forms.Label label1;
    }
}