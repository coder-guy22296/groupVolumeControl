namespace VolumeControlUtility
{
    partial class MainUI
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
            this.addButton = new System.Windows.Forms.Button();
            this.removeButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.programGroupList = new System.Windows.Forms.ListBox();
            this.programBox = new System.Windows.Forms.GroupBox();
            this.programsInGroupList = new System.Windows.Forms.ListBox();
            this.AudioSessionList = new System.Windows.Forms.ListBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.addGroupButton = new System.Windows.Forms.Button();
            this.addGroupTextBox = new System.Windows.Forms.TextBox();
            this.removeGroupButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.renameGroupButton = new System.Windows.Forms.Button();
            this.infoBox = new System.Windows.Forms.GroupBox();
            this.volDownHotkey = new System.Windows.Forms.Label();
            this.volUpHotkey = new System.Windows.Forms.Label();
            this.setVolumeDown = new System.Windows.Forms.Button();
            this.setVolumeUp = new System.Windows.Forms.Button();
            this.checkBoxWIN = new System.Windows.Forms.CheckBox();
            this.checkBoxALT = new System.Windows.Forms.CheckBox();
            this.checkBoxCTRL = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.saveKbButton = new System.Windows.Forms.Button();
            this.DefaultVolBtn = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.curVolume = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.programBox.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.infoBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // addButton
            // 
            this.addButton.BackColor = System.Drawing.Color.Orange;
            this.addButton.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.addButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.addButton.Location = new System.Drawing.Point(135, 19);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(89, 23);
            this.addButton.TabIndex = 9;
            this.addButton.Text = "Add";
            this.addButton.UseVisualStyleBackColor = false;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // removeButton
            // 
            this.removeButton.BackColor = System.Drawing.Color.Orange;
            this.removeButton.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.removeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.removeButton.Location = new System.Drawing.Point(135, 57);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(89, 23);
            this.removeButton.TabIndex = 10;
            this.removeButton.Text = "Remove";
            this.removeButton.UseVisualStyleBackColor = false;
            this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.programGroupList);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.groupBox1.Size = new System.Drawing.Size(106, 408);
            this.groupBox1.TabIndex = 18;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Program Groups";
            // 
            // programGroupList
            // 
            this.programGroupList.FormattingEnabled = true;
            this.programGroupList.HorizontalScrollbar = true;
            this.programGroupList.Location = new System.Drawing.Point(6, 19);
            this.programGroupList.Name = "programGroupList";
            this.programGroupList.Size = new System.Drawing.Size(94, 381);
            this.programGroupList.TabIndex = 20;
            this.programGroupList.SelectedIndexChanged += new System.EventHandler(this.programGroupList_SelectedIndexChanged);
            // 
            // programBox
            // 
            this.programBox.Controls.Add(this.programsInGroupList);
            this.programBox.Location = new System.Drawing.Point(124, 86);
            this.programBox.Name = "programBox";
            this.programBox.Size = new System.Drawing.Size(106, 334);
            this.programBox.TabIndex = 19;
            this.programBox.TabStop = false;
            // 
            // programsInGroupList
            // 
            this.programsInGroupList.FormattingEnabled = true;
            this.programsInGroupList.Location = new System.Drawing.Point(6, 23);
            this.programsInGroupList.Name = "programsInGroupList";
            this.programsInGroupList.Size = new System.Drawing.Size(94, 303);
            this.programsInGroupList.TabIndex = 21;
            this.programsInGroupList.SelectedIndexChanged += new System.EventHandler(this.programsInGroupList_SelectedIndexChanged);
            // 
            // AudioSessionList
            // 
            this.AudioSessionList.FormattingEnabled = true;
            this.AudioSessionList.Location = new System.Drawing.Point(6, 19);
            this.AudioSessionList.Name = "AudioSessionList";
            this.AudioSessionList.Size = new System.Drawing.Size(135, 381);
            this.AudioSessionList.TabIndex = 20;
            this.AudioSessionList.Enter += new System.EventHandler(this.AudioSessionList_Enter);
            this.AudioSessionList.MouseHover += new System.EventHandler(this.AudioSessionList_MouseHover);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.AudioSessionList);
            this.groupBox3.Location = new System.Drawing.Point(563, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.groupBox3.Size = new System.Drawing.Size(147, 408);
            this.groupBox3.TabIndex = 21;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Audio Programs";
            // 
            // addGroupButton
            // 
            this.addGroupButton.BackColor = System.Drawing.Color.Orange;
            this.addGroupButton.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.addGroupButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.addGroupButton.Location = new System.Drawing.Point(468, 19);
            this.addGroupButton.Name = "addGroupButton";
            this.addGroupButton.Size = new System.Drawing.Size(89, 23);
            this.addGroupButton.TabIndex = 22;
            this.addGroupButton.Text = "Add Group";
            this.addGroupButton.UseVisualStyleBackColor = false;
            this.addGroupButton.Click += new System.EventHandler(this.addGroupButton_Click);
            // 
            // addGroupTextBox
            // 
            this.addGroupTextBox.Location = new System.Drawing.Point(373, 19);
            this.addGroupTextBox.Name = "addGroupTextBox";
            this.addGroupTextBox.Size = new System.Drawing.Size(89, 20);
            this.addGroupTextBox.TabIndex = 23;
            // 
            // removeGroupButton
            // 
            this.removeGroupButton.BackColor = System.Drawing.Color.Orange;
            this.removeGroupButton.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.removeGroupButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.removeGroupButton.Location = new System.Drawing.Point(468, 48);
            this.removeGroupButton.Name = "removeGroupButton";
            this.removeGroupButton.Size = new System.Drawing.Size(89, 23);
            this.removeGroupButton.TabIndex = 24;
            this.removeGroupButton.Text = "Remove Group";
            this.removeGroupButton.UseVisualStyleBackColor = false;
            this.removeGroupButton.Click += new System.EventHandler(this.removeGroupButton_Click);
            // 
            // saveButton
            // 
            this.saveButton.BackColor = System.Drawing.Color.Orange;
            this.saveButton.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.saveButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.saveButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.saveButton.Location = new System.Drawing.Point(482, 389);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 23);
            this.saveButton.TabIndex = 25;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = false;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // renameGroupButton
            // 
            this.renameGroupButton.BackColor = System.Drawing.Color.Orange;
            this.renameGroupButton.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.renameGroupButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.renameGroupButton.Location = new System.Drawing.Point(373, 48);
            this.renameGroupButton.Name = "renameGroupButton";
            this.renameGroupButton.Size = new System.Drawing.Size(89, 23);
            this.renameGroupButton.TabIndex = 26;
            this.renameGroupButton.Text = "Rename Group";
            this.renameGroupButton.UseVisualStyleBackColor = false;
            this.renameGroupButton.Click += new System.EventHandler(this.renameGroupButton_Click);
            // 
            // infoBox
            // 
            this.infoBox.Controls.Add(this.curVolume);
            this.infoBox.Controls.Add(this.label4);
            this.infoBox.Controls.Add(this.volDownHotkey);
            this.infoBox.Controls.Add(this.volUpHotkey);
            this.infoBox.Controls.Add(this.setVolumeDown);
            this.infoBox.Controls.Add(this.setVolumeUp);
            this.infoBox.Controls.Add(this.checkBoxWIN);
            this.infoBox.Controls.Add(this.checkBoxALT);
            this.infoBox.Controls.Add(this.checkBoxCTRL);
            this.infoBox.Controls.Add(this.label2);
            this.infoBox.Controls.Add(this.label3);
            this.infoBox.Controls.Add(this.label1);
            this.infoBox.Controls.Add(this.saveKbButton);
            this.infoBox.Location = new System.Drawing.Point(236, 86);
            this.infoBox.Name = "infoBox";
            this.infoBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.infoBox.Size = new System.Drawing.Size(321, 218);
            this.infoBox.TabIndex = 27;
            this.infoBox.TabStop = false;
            this.infoBox.Text = "Details";
            this.infoBox.Enter += new System.EventHandler(this.infoBox_Enter);
            // 
            // volDownHotkey
            // 
            this.volDownHotkey.AutoSize = true;
            this.volDownHotkey.Location = new System.Drawing.Point(109, 114);
            this.volDownHotkey.Name = "volDownHotkey";
            this.volDownHotkey.Size = new System.Drawing.Size(56, 13);
            this.volDownHotkey.TabIndex = 37;
            this.volDownHotkey.Text = "volDOWN";
            // 
            // volUpHotkey
            // 
            this.volUpHotkey.AutoSize = true;
            this.volUpHotkey.Location = new System.Drawing.Point(109, 87);
            this.volUpHotkey.Name = "volUpHotkey";
            this.volUpHotkey.Size = new System.Drawing.Size(36, 13);
            this.volUpHotkey.TabIndex = 36;
            this.volUpHotkey.Text = "volUP";
            // 
            // setVolumeDown
            // 
            this.setVolumeDown.BackColor = System.Drawing.Color.Orange;
            this.setVolumeDown.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.setVolumeDown.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.setVolumeDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.setVolumeDown.Location = new System.Drawing.Point(187, 109);
            this.setVolumeDown.Name = "setVolumeDown";
            this.setVolumeDown.Size = new System.Drawing.Size(114, 23);
            this.setVolumeDown.TabIndex = 35;
            this.setVolumeDown.Text = "Set Volume Down";
            this.setVolumeDown.UseVisualStyleBackColor = false;
            this.setVolumeDown.Click += new System.EventHandler(this.setVolumeDown_Click);
            // 
            // setVolumeUp
            // 
            this.setVolumeUp.BackColor = System.Drawing.Color.Orange;
            this.setVolumeUp.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.setVolumeUp.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.setVolumeUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.setVolumeUp.Location = new System.Drawing.Point(187, 82);
            this.setVolumeUp.Name = "setVolumeUp";
            this.setVolumeUp.Size = new System.Drawing.Size(114, 23);
            this.setVolumeUp.TabIndex = 29;
            this.setVolumeUp.Text = "Set Volume Up";
            this.setVolumeUp.UseVisualStyleBackColor = false;
            this.setVolumeUp.Click += new System.EventHandler(this.setVolumeUp_Click);
            // 
            // checkBoxWIN
            // 
            this.checkBoxWIN.AutoSize = true;
            this.checkBoxWIN.Location = new System.Drawing.Point(112, 61);
            this.checkBoxWIN.Name = "checkBoxWIN";
            this.checkBoxWIN.Size = new System.Drawing.Size(45, 17);
            this.checkBoxWIN.TabIndex = 34;
            this.checkBoxWIN.Text = "Win";
            this.checkBoxWIN.UseVisualStyleBackColor = true;
            this.checkBoxWIN.CheckedChanged += new System.EventHandler(this.checkBoxWIN_CheckedChanged);
            this.checkBoxWIN.Click += new System.EventHandler(this.checkBoxWIN_CheckedChanged);
            // 
            // checkBoxALT
            // 
            this.checkBoxALT.AutoSize = true;
            this.checkBoxALT.Location = new System.Drawing.Point(112, 38);
            this.checkBoxALT.Name = "checkBoxALT";
            this.checkBoxALT.Size = new System.Drawing.Size(38, 17);
            this.checkBoxALT.TabIndex = 33;
            this.checkBoxALT.Text = "Alt";
            this.checkBoxALT.UseVisualStyleBackColor = true;
            this.checkBoxALT.CheckedChanged += new System.EventHandler(this.checkBoxALT_CheckedChanged);
            this.checkBoxALT.Click += new System.EventHandler(this.checkBoxALT_CheckedChanged);
            // 
            // checkBoxCTRL
            // 
            this.checkBoxCTRL.AutoSize = true;
            this.checkBoxCTRL.Location = new System.Drawing.Point(112, 15);
            this.checkBoxCTRL.Name = "checkBoxCTRL";
            this.checkBoxCTRL.Size = new System.Drawing.Size(41, 17);
            this.checkBoxCTRL.TabIndex = 32;
            this.checkBoxCTRL.Text = "Ctrl";
            this.checkBoxCTRL.UseVisualStyleBackColor = true;
            this.checkBoxCTRL.CheckedChanged += new System.EventHandler(this.checkBoxCTRL_CheckedChanged);
            this.checkBoxCTRL.Click += new System.EventHandler(this.checkBoxCTRL_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(52, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 31;
            this.label2.Text = "modifiers: ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(30, 114);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(76, 13);
            this.label3.TabIndex = 29;
            this.label3.Text = "Volume Down:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(44, 87);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 27;
            this.label1.Text = "Volume Up:";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // saveKbButton
            // 
            this.saveKbButton.BackColor = System.Drawing.Color.Orange;
            this.saveKbButton.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.saveKbButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.saveKbButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.saveKbButton.Location = new System.Drawing.Point(209, 189);
            this.saveKbButton.Name = "saveKbButton";
            this.saveKbButton.Size = new System.Drawing.Size(106, 23);
            this.saveKbButton.TabIndex = 26;
            this.saveKbButton.Text = "Save Key Bindings";
            this.saveKbButton.UseVisualStyleBackColor = false;
            this.saveKbButton.Click += new System.EventHandler(this.saveKbButton_Click);
            // 
            // DefaultVolBtn
            // 
            this.DefaultVolBtn.BackColor = System.Drawing.Color.Orange;
            this.DefaultVolBtn.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.DefaultVolBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.DefaultVolBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.DefaultVolBtn.Location = new System.Drawing.Point(236, 389);
            this.DefaultVolBtn.Name = "DefaultVolBtn";
            this.DefaultVolBtn.Size = new System.Drawing.Size(96, 23);
            this.DefaultVolBtn.TabIndex = 28;
            this.DefaultVolBtn.Text = "Default Volumes";
            this.DefaultVolBtn.UseVisualStyleBackColor = false;
            this.DefaultVolBtn.Click += new System.EventHandler(this.DefaultVolBtn_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(21, 189);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(85, 13);
            this.label4.TabIndex = 38;
            this.label4.Text = "Current Volume: ";
            // 
            // curVolume
            // 
            this.curVolume.AutoSize = true;
            this.curVolume.Location = new System.Drawing.Point(112, 189);
            this.curVolume.Name = "curVolume";
            this.curVolume.Size = new System.Drawing.Size(56, 13);
            this.curVolume.TabIndex = 39;
            this.curVolume.Text = "Unknown ";
            // 
            // MainUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(722, 432);
            this.Controls.Add(this.DefaultVolBtn);
            this.Controls.Add(this.infoBox);
            this.Controls.Add(this.renameGroupButton);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.removeGroupButton);
            this.Controls.Add(this.addGroupTextBox);
            this.Controls.Add(this.addGroupButton);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.programBox);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.removeButton);
            this.Controls.Add(this.addButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "MainUI";
            this.Text = "volume control";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.programBox.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.infoBox.ResumeLayout(false);
            this.infoBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Button removeButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox programBox;
        private System.Windows.Forms.ListBox programGroupList;
        private System.Windows.Forms.ListBox programsInGroupList;
        private System.Windows.Forms.ListBox AudioSessionList;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button addGroupButton;
        private System.Windows.Forms.TextBox addGroupTextBox;
        private System.Windows.Forms.Button removeGroupButton;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button renameGroupButton;
        private System.Windows.Forms.GroupBox infoBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button saveKbButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox checkBoxWIN;
        private System.Windows.Forms.CheckBox checkBoxALT;
        private System.Windows.Forms.CheckBox checkBoxCTRL;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button DefaultVolBtn;
        private System.Windows.Forms.Button setVolumeDown;
        private System.Windows.Forms.Button setVolumeUp;
        private System.Windows.Forms.Label volDownHotkey;
        private System.Windows.Forms.Label volUpHotkey;
        private System.Windows.Forms.Label curVolume;
        private System.Windows.Forms.Label label4;
    }
}

