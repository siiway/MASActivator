namespace MASActivator
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            label1 = new Label();
            linkLabel1 = new LinkLabel();
            linkLabel2 = new LinkLabel();
            linkLabel3 = new LinkLabel();
            linkLabel4 = new LinkLabel();
            tabControl = new TabControl();
            HomePage = new TabPage();
            WinPage = new TabPage();
            OfficePage = new TabPage();
            OtherPage = new TabPage();
            gbChangeWindowsEdition = new GroupBox();
            btnChangeWindowsEdition = new Button();
            comboBoxWindowsEditions = new ComboBox();
            TroubleshootPage = new TabPage();
            groupBox1 = new GroupBox();
            tabControl.SuspendLayout();
            OtherPage.SuspendLayout();
            gbChangeWindowsEdition.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Cascadia Code", 36F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(32, 48);
            label1.Name = "label1";
            label1.Size = new Size(726, 127);
            label1.TabIndex = 0;
            label1.Text = "MASActivator";
            // 
            // linkLabel1
            // 
            linkLabel1.AutoSize = true;
            linkLabel1.LinkColor = Color.Blue;
            linkLabel1.Location = new Point(52, 175);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new Size(184, 31);
            linkLabel1.TabIndex = 1;
            linkLabel1.TabStop = true;
            linkLabel1.Text = "MAS [Website]";
            linkLabel1.VisitedLinkColor = Color.Blue;
            linkLabel1.LinkClicked += linkLabel1_LinkClicked;
            // 
            // linkLabel2
            // 
            linkLabel2.AutoSize = true;
            linkLabel2.LinkColor = Color.Blue;
            linkLabel2.Location = new Point(242, 175);
            linkLabel2.Name = "linkLabel2";
            linkLabel2.Size = new Size(173, 31);
            linkLabel2.TabIndex = 2;
            linkLabel2.TabStop = true;
            linkLabel2.Text = "MAS [GitHub]";
            linkLabel2.VisitedLinkColor = Color.Blue;
            linkLabel2.LinkClicked += linkLabel2_LinkClicked;
            // 
            // linkLabel3
            // 
            linkLabel3.AutoSize = true;
            linkLabel3.LinkColor = Color.Blue;
            linkLabel3.Location = new Point(421, 175);
            linkLabel3.Name = "linkLabel3";
            linkLabel3.Size = new Size(190, 31);
            linkLabel3.TabIndex = 3;
            linkLabel3.TabStop = true;
            linkLabel3.Text = "MASA [GitHub]";
            linkLabel3.VisitedLinkColor = Color.Blue;
            linkLabel3.LinkClicked += linkLabel3_LinkClicked;
            // 
            // linkLabel4
            // 
            linkLabel4.AutoSize = true;
            linkLabel4.LinkColor = Color.Blue;
            linkLabel4.Location = new Point(617, 175);
            linkLabel4.Name = "linkLabel4";
            linkLabel4.Size = new Size(172, 31);
            linkLabel4.TabIndex = 4;
            linkLabel4.TabStop = true;
            linkLabel4.Text = "About Project";
            linkLabel4.VisitedLinkColor = Color.Blue;
            linkLabel4.LinkClicked += linkLabel4_LinkClicked;
            // 
            // tabControl
            // 
            tabControl.Controls.Add(HomePage);
            tabControl.Controls.Add(WinPage);
            tabControl.Controls.Add(OfficePage);
            tabControl.Controls.Add(OtherPage);
            tabControl.Controls.Add(TroubleshootPage);
            tabControl.Location = new Point(60, 253);
            tabControl.Name = "tabControl";
            tabControl.SelectedIndex = 0;
            tabControl.Size = new Size(1443, 556);
            tabControl.TabIndex = 5;
            // 
            // HomePage
            // 
            HomePage.Location = new Point(8, 45);
            HomePage.Name = "HomePage";
            HomePage.Padding = new Padding(3);
            HomePage.Size = new Size(1427, 503);
            HomePage.TabIndex = 0;
            HomePage.Text = "Home";
            HomePage.UseVisualStyleBackColor = true;
            // 
            // WinPage
            // 
            WinPage.Location = new Point(8, 45);
            WinPage.Name = "WinPage";
            WinPage.Padding = new Padding(3);
            WinPage.Size = new Size(1427, 503);
            WinPage.TabIndex = 1;
            WinPage.Text = "Windows";
            WinPage.UseVisualStyleBackColor = true;
            // 
            // OfficePage
            // 
            OfficePage.Location = new Point(8, 45);
            OfficePage.Name = "OfficePage";
            OfficePage.Size = new Size(1427, 503);
            OfficePage.TabIndex = 2;
            OfficePage.Text = "Office";
            OfficePage.UseVisualStyleBackColor = true;
            // 
            // OtherPage
            // 
            OtherPage.Controls.Add(groupBox1);
            OtherPage.Controls.Add(gbChangeWindowsEdition);
            OtherPage.Location = new Point(8, 45);
            OtherPage.Name = "OtherPage";
            OtherPage.Size = new Size(1427, 503);
            OtherPage.TabIndex = 3;
            OtherPage.Text = "Other";
            OtherPage.UseVisualStyleBackColor = true;
            // 
            // gbChangeWindowsEdition
            // 
            gbChangeWindowsEdition.Controls.Add(btnChangeWindowsEdition);
            gbChangeWindowsEdition.Controls.Add(comboBoxWindowsEditions);
            gbChangeWindowsEdition.Location = new Point(34, 37);
            gbChangeWindowsEdition.Name = "gbChangeWindowsEdition";
            gbChangeWindowsEdition.Size = new Size(538, 191);
            gbChangeWindowsEdition.TabIndex = 0;
            gbChangeWindowsEdition.TabStop = false;
            gbChangeWindowsEdition.Text = "Change Windows Edition";
            // 
            // btnChangeWindowsEdition
            // 
            btnChangeWindowsEdition.Location = new Point(17, 110);
            btnChangeWindowsEdition.Name = "btnChangeWindowsEdition";
            btnChangeWindowsEdition.Size = new Size(499, 55);
            btnChangeWindowsEdition.TabIndex = 1;
            btnChangeWindowsEdition.Text = "Change Windows Edition";
            btnChangeWindowsEdition.UseVisualStyleBackColor = true;
            btnChangeWindowsEdition.Click += btnChangeWindowsEdition_Click;
            // 
            // comboBoxWindowsEditions
            // 
            comboBoxWindowsEditions.FormattingEnabled = true;
            comboBoxWindowsEditions.Location = new Point(17, 49);
            comboBoxWindowsEditions.Name = "comboBoxWindowsEditions";
            comboBoxWindowsEditions.Size = new Size(496, 39);
            comboBoxWindowsEditions.TabIndex = 0;
            comboBoxWindowsEditions.Text = "Select an edition...";
            // 
            // TroubleshootPage
            // 
            TroubleshootPage.Location = new Point(8, 45);
            TroubleshootPage.Name = "TroubleshootPage";
            TroubleshootPage.Size = new Size(1427, 503);
            TroubleshootPage.TabIndex = 4;
            TroubleshootPage.Text = "Troubleshoot";
            TroubleshootPage.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            groupBox1.Location = new Point(34, 261);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(538, 201);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "Change Office Edition";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(14F, 31F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1541, 847);
            Controls.Add(tabControl);
            Controls.Add(linkLabel4);
            Controls.Add(linkLabel3);
            Controls.Add(linkLabel2);
            Controls.Add(linkLabel1);
            Controls.Add(label1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "MainForm";
            Text = "MASA";
            Load += MainForm_Load;
            tabControl.ResumeLayout(false);
            OtherPage.ResumeLayout(false);
            gbChangeWindowsEdition.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private LinkLabel linkLabel1;
        private LinkLabel linkLabel2;
        private LinkLabel linkLabel3;
        private LinkLabel linkLabel4;
        private TabControl tabControl;
        private TabPage HomePage;
        private TabPage WinPage;
        private TabPage OfficePage;
        private TabPage OtherPage;
        private TabPage TroubleshootPage;
        private GroupBox gbChangeWindowsEdition;
        private ComboBox comboBoxWindowsEditions;
        private Button btnChangeWindowsEdition;
        private GroupBox groupBox1;
    }
}
