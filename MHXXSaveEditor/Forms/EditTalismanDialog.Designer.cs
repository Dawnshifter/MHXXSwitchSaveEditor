namespace MHXXSaveEditor.Forms
{
    partial class EditTalismanDialog
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboBoxSkillName2 = new System.Windows.Forms.ComboBox();
            this.comboBoxSkillName1 = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.numericUpDownSkillLevel2 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownSkillLevel1 = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.numericUpDownSlots = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSkillLevel2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSkillLevel1)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSlots)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboBoxSkillName2);
            this.groupBox1.Controls.Add(this.comboBoxSkillName1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 100);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Skills";
            // 
            // comboBoxSkillName2
            // 
            this.comboBoxSkillName2.FormattingEnabled = true;
            this.comboBoxSkillName2.Location = new System.Drawing.Point(73, 63);
            this.comboBoxSkillName2.Name = "comboBoxSkillName2";
            this.comboBoxSkillName2.Size = new System.Drawing.Size(121, 24);
            this.comboBoxSkillName2.TabIndex = 6;
            this.comboBoxSkillName2.SelectionChangeCommitted += new System.EventHandler(this.ComboBoxSkillName2_SelectionChangeCommitted);
            // 
            // comboBoxSkillName1
            // 
            this.comboBoxSkillName1.FormattingEnabled = true;
            this.comboBoxSkillName1.Location = new System.Drawing.Point(73, 26);
            this.comboBoxSkillName1.Name = "comboBoxSkillName1";
            this.comboBoxSkillName1.Size = new System.Drawing.Size(121, 24);
            this.comboBoxSkillName1.TabIndex = 5;
            this.comboBoxSkillName1.SelectionChangeCommitted += new System.EventHandler(this.ComboBoxSkillName1_SelectionChangeCommitted);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "Skill 2";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "Skill 1";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.numericUpDownSkillLevel2);
            this.groupBox2.Controls.Add(this.numericUpDownSkillLevel1);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(218, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(200, 100);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Level";
            // 
            // numericUpDownSkillLevel2
            // 
            this.numericUpDownSkillLevel2.Location = new System.Drawing.Point(74, 64);
            this.numericUpDownSkillLevel2.Maximum = new decimal(new int[] {
            13,
            0,
            0,
            0});
            this.numericUpDownSkillLevel2.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            this.numericUpDownSkillLevel2.Name = "numericUpDownSkillLevel2";
            this.numericUpDownSkillLevel2.Size = new System.Drawing.Size(120, 22);
            this.numericUpDownSkillLevel2.TabIndex = 6;
            // 
            // numericUpDownSkillLevel1
            // 
            this.numericUpDownSkillLevel1.Location = new System.Drawing.Point(74, 27);
            this.numericUpDownSkillLevel1.Maximum = new decimal(new int[] {
            13,
            0,
            0,
            0});
            this.numericUpDownSkillLevel1.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            this.numericUpDownSkillLevel1.Name = "numericUpDownSkillLevel1";
            this.numericUpDownSkillLevel1.Size = new System.Drawing.Size(120, 22);
            this.numericUpDownSkillLevel1.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 66);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 17);
            this.label3.TabIndex = 3;
            this.label3.Text = "Skill 2";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 29);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(45, 17);
            this.label4.TabIndex = 2;
            this.label4.Text = "Skill 1";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.numericUpDownSlots);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Location = new System.Drawing.Point(424, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(200, 100);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Slots";
            // 
            // numericUpDownSlots
            // 
            this.numericUpDownSlots.Location = new System.Drawing.Point(74, 27);
            this.numericUpDownSlots.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.numericUpDownSlots.Name = "numericUpDownSlots";
            this.numericUpDownSlots.Size = new System.Drawing.Size(120, 22);
            this.numericUpDownSlots.TabIndex = 5;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 29);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(39, 17);
            this.label7.TabIndex = 2;
            this.label7.Text = "Slots";
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(169, 118);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(100, 30);
            this.buttonSave.TabIndex = 8;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.ButtonSave_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(369, 118);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(100, 30);
            this.buttonCancel.TabIndex = 9;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.ButtonCancel_Click);
            // 
            // EditTalismanDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(637, 163);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditTalismanDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Editing Talisman";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSkillLevel2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSkillLevel1)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSlots)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox comboBoxSkillName2;
        private System.Windows.Forms.ComboBox comboBoxSkillName1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.NumericUpDown numericUpDownSkillLevel2;
        private System.Windows.Forms.NumericUpDown numericUpDownSkillLevel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.NumericUpDown numericUpDownSlots;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonCancel;
    }
}