using System;
using System.Windows.Forms;

namespace BotCore
{
    partial class BotInterface
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private ListBox leaderListBox;
        
        public void addClientToLeaderListBox(string leaderName)
        {
            // Find the leaderListBox in the followTab
            if (leaderListBox != null)
            {
                // Add the leader name to the listbox
                leaderListBox.Items.Add(leaderName);
                Console.WriteLine("Added leader: " + leaderName);
            }
        }
        
        private TabPage followTab()
        {
            TabPage followTab = new TabPage();
            
            Panel leaderPanel = new Panel();
            leaderPanel.Location = new System.Drawing.Point(0, 0);
            leaderPanel.Name = "leaderPanel";
            leaderPanel.Size = new System.Drawing.Size(200, 340);
            leaderPanel.TabIndex = 6;
            leaderPanel.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
            followTab.Controls.Add(leaderPanel);
            
            // add a listbox to the leaderPanel
            leaderListBox = new ListBox();
            leaderListBox.Location = new System.Drawing.Point(10, 10);
            leaderListBox.Name = "leaderListBox";
            leaderListBox.Size = new System.Drawing.Size(180, 320);
            leaderListBox.TabIndex = 0;
            leaderListBox.SelectedIndexChanged += (sender, e) =>
            {
                // Handle selection change
                // For example, update the selected leader in the FollowTarget state
                if (leaderListBox.SelectedItem != null)
                {
                    string selectedLeader = leaderListBox.SelectedItem.ToString();
                    // Update the FollowTarget state with the selected leader
                    // FollowTarget.UpdateSelectedLeader(selectedLeader);
                }
            };
            
            lock (AllLeadersLock)
            {
                foreach (var leader in _allLeaders)
                {
                    if (leaderListBox != null && !leaderListBox.Items.Contains(leader))
                        leaderListBox.Items.Add(leader);
                }
            }
            
            leaderPanel.Controls.Add(leaderListBox);
            
            
            //this.followTab.Controls.Add(this.button4);
            //this.followTab.Controls.Add(this.button3);
            //this.followTab.Controls.Add(this.comboBox1);
            //this.followTab.Controls.Add(this.groupBox1);
            followTab.Location = new System.Drawing.Point(4, 25);
            followTab.Name = "followTab";
            followTab.Padding = new Padding(3);
            followTab.Size = new System.Drawing.Size(716, 431);
            followTab.TabIndex = 2;
            followTab.Text = "Follow";
            followTab.UseVisualStyleBackColor = true;
            followTab.ResumeLayout(false);
            
            return followTab;
        }
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BotInterface));

            
            this.tabPage4 = new TabPage();
            this.groupBox2 = new GroupBox();
            this.checkBox8 = new CheckBox();
            this.checkBox7 = new CheckBox();
            this.checkBox6 = new CheckBox();
            this.checkBox5 = new CheckBox();
            this.checkBox4 = new CheckBox();
            this.tabPage2 = new TabPage();
            this.button2 = new Button();
            this.comboBox2 = new ComboBox();
            this.button1 = new Button();
            this.checkBox3 = new CheckBox();
            this.checkBox2 = new CheckBox();
            this.richTextBox2 = new RichTextBox();
            this.richTextBox1 = new RichTextBox();
            this.checkBox1 = new CheckBox();
            this.tabPage1 = new TabPage();
            this.panel1 = new Panel();
            this.button4 = new Button();
            this.button3 = new Button();
            this.comboBox1 = new ComboBox();
            this.groupBox1 = new GroupBox();
            this.statepanel = new Panel();
            this.tabControl1 = new TabControl();
            this.tabPage5 = new TabPage();
            this.button5 = new Button();
            this.tabPage4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.SuspendLayout();
            

            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.groupBox2);
            this.tabPage4.Location = new System.Drawing.Point(4, 25);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(716, 431);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Settings";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkBox8);
            this.groupBox2.Controls.Add(this.checkBox7);
            this.groupBox2.Controls.Add(this.checkBox6);
            this.groupBox2.Controls.Add(this.checkBox5);
            this.groupBox2.Controls.Add(this.checkBox4);
            this.groupBox2.Location = new System.Drawing.Point(8, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(148, 120);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Display Filters";
            // 
            // checkBox8
            // 
            this.checkBox8.AutoSize = true;
            this.checkBox8.Location = new System.Drawing.Point(21, 57);
            this.checkBox8.Name = "checkBox8";
            this.checkBox8.Size = new System.Drawing.Size(105, 20);
            this.checkBox8.TabIndex = 4;
            this.checkBox8.Text = "Hide Assails";
            this.checkBox8.UseVisualStyleBackColor = true;
            this.checkBox8.CheckedChanged += new System.EventHandler(this.checkBox8_CheckedChanged);
            // 
            // checkBox7
            // 
            this.checkBox7.AutoSize = true;
            this.checkBox7.Location = new System.Drawing.Point(21, 76);
            this.checkBox7.Name = "checkBox7";
            this.checkBox7.Size = new System.Drawing.Size(153, 20);
            this.checkBox7.TabIndex = 3;
            this.checkBox7.Text = "Monster Animations";
            this.checkBox7.UseVisualStyleBackColor = true;
            this.checkBox7.CheckedChanged += new System.EventHandler(this.checkBox7_CheckedChanged);
            // 
            // checkBox6
            // 
            this.checkBox6.AutoSize = true;
            this.checkBox6.Location = new System.Drawing.Point(21, 94);
            this.checkBox6.Name = "checkBox6";
            this.checkBox6.Size = new System.Drawing.Size(114, 20);
            this.checkBox6.TabIndex = 2;
            this.checkBox6.Text = "Filter Sounds";
            this.checkBox6.UseVisualStyleBackColor = true;
            this.checkBox6.CheckedChanged += new System.EventHandler(this.checkBox6_CheckedChanged);
            // 
            // checkBox5
            // 
            this.checkBox5.AutoSize = true;
            this.checkBox5.Location = new System.Drawing.Point(21, 38);
            this.checkBox5.Name = "checkBox5";
            this.checkBox5.Size = new System.Drawing.Size(120, 20);
            this.checkBox5.TabIndex = 1;
            this.checkBox5.Text = "Simple Sprites";
            this.checkBox5.UseVisualStyleBackColor = true;
            this.checkBox5.CheckedChanged += new System.EventHandler(this.checkBox5_CheckedChanged);
            // 
            // checkBox4
            // 
            this.checkBox4.AutoSize = true;
            this.checkBox4.Location = new System.Drawing.Point(21, 20);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(98, 20);
            this.checkBox4.TabIndex = 0;
            this.checkBox4.Text = "Hide Coins";
            this.checkBox4.UseVisualStyleBackColor = true;
            this.checkBox4.CheckedChanged += new System.EventHandler(this.checkBox4_CheckedChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.button2);
            this.tabPage2.Controls.Add(this.comboBox2);
            this.tabPage2.Controls.Add(this.button1);
            this.tabPage2.Controls.Add(this.checkBox3);
            this.tabPage2.Controls.Add(this.checkBox2);
            this.tabPage2.Controls.Add(this.richTextBox2);
            this.tabPage2.Controls.Add(this.richTextBox1);
            this.tabPage2.Controls.Add(this.checkBox1);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Margin = new Padding(2, 3, 2, 3);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new Padding(2, 3, 2, 3);
            this.tabPage2.Size = new System.Drawing.Size(716, 431);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Packet Editor";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Anchor = ((AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom) 
            | AnchorStyles.Left) 
            | AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(14, 394);
            this.button2.Margin = new Padding(2, 3, 2, 3);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(74, 29);
            this.button2.TabIndex = 7;
            this.button2.Text = "Clear";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // comboBox2
            // 
            this.comboBox2.Anchor = ((AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom) 
            | AnchorStyles.Left) 
            | AnchorStyles.Right)));
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Items.AddRange(new object[] {
            "Client",
            "Server",
            "Auto"});
            this.comboBox2.Location = new System.Drawing.Point(470, 396);
            this.comboBox2.Margin = new Padding(2, 3, 2, 3);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(121, 24);
            this.comboBox2.TabIndex = 6;
            this.comboBox2.Text = "Server";
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.button1.FlatStyle = FlatStyle.Flat;
            this.button1.ForeColor = System.Drawing.Color.Crimson;
            this.button1.Location = new System.Drawing.Point(598, 394);
            this.button1.Margin = new Padding(2, 3, 2, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(100, 28);
            this.button1.TabIndex = 5;
            this.button1.Text = "Inject";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // checkBox3
            // 
            this.checkBox3.Anchor = ((AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom) 
            | AnchorStyles.Left) 
            | AnchorStyles.Right)));
            this.checkBox3.AutoSize = true;
            this.checkBox3.Location = new System.Drawing.Point(482, 280);
            this.checkBox3.Margin = new Padding(2, 3, 2, 3);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(86, 20);
            this.checkBox3.TabIndex = 4;
            this.checkBox3.Text = "Incoming";
            this.checkBox3.UseVisualStyleBackColor = true;
            this.checkBox3.CheckedChanged += new System.EventHandler(this.checkBox3_CheckedChanged);
            // 
            // checkBox2
            // 
            this.checkBox2.Anchor = ((AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom) 
            | AnchorStyles.Left) 
            | AnchorStyles.Right)));
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(587, 280);
            this.checkBox2.Margin = new Padding(2, 3, 2, 3);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(88, 20);
            this.checkBox2.TabIndex = 3;
            this.checkBox2.Text = "Outgoing";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // richTextBox2
            // 
            this.richTextBox2.Anchor = ((AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom) 
            | AnchorStyles.Left) 
            | AnchorStyles.Right)));
            this.richTextBox2.Location = new System.Drawing.Point(14, 306);
            this.richTextBox2.Margin = new Padding(2, 3, 2, 3);
            this.richTextBox2.Name = "richTextBox2";
            this.richTextBox2.Size = new System.Drawing.Size(684, 83);
            this.richTextBox2.TabIndex = 2;
            this.richTextBox2.Text = "";
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom) 
            | AnchorStyles.Left) 
            | AnchorStyles.Right)));
            this.richTextBox1.Enabled = false;
            this.richTextBox1.Font = new System.Drawing.Font("Segoe UI Emoji", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox1.Location = new System.Drawing.Point(14, 20);
            this.richTextBox1.Margin = new Padding(2, 3, 2, 3);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(684, 254);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            this.richTextBox1.MouseDown += new MouseEventHandler(this.richTextBox1_MouseDown);
            this.richTextBox1.MouseUp += new MouseEventHandler(this.richTextBox1_MouseUp);
            // 
            // checkBox1
            // 
            this.checkBox1.Anchor = ((AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom) 
            | AnchorStyles.Left) 
            | AnchorStyles.Right)));
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(14, 280);
            this.checkBox1.Margin = new Padding(2, 3, 2, 3);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(74, 20);
            this.checkBox1.TabIndex = 1;
            this.checkBox1.Text = "Enable";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.panel1);
            this.tabPage1.Controls.Add(this.button4);
            this.tabPage1.Controls.Add(this.button3);
            this.tabPage1.Controls.Add(this.comboBox1);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Margin = new Padding(4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new Padding(4);
            this.tabPage1.Size = new System.Drawing.Size(716, 431);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Botting States";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(9, 77);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 340);
            this.panel1.TabIndex = 6;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(110, 38);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(93, 33);
            this.button4.TabIndex = 5;
            this.button4.Text = "Resume";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(11, 38);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(93, 33);
            this.button3.TabIndex = 4;
            this.button3.Text = "Pause";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.Anchor = ((AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom) 
            | AnchorStyles.Left) 
            | AnchorStyles.Right)));
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(11, 8);
            this.comboBox1.Margin = new Padding(4);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(192, 24);
            this.comboBox1.TabIndex = 2;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom) 
            | AnchorStyles.Left) 
            | AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.statepanel);
            this.groupBox1.Location = new System.Drawing.Point(220, 8);
            this.groupBox1.Margin = new Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(484, 413);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "State Settings";
            // 
            // statepanel
            // 
            this.statepanel.Dock = DockStyle.Fill;
            this.statepanel.Location = new System.Drawing.Point(4, 20);
            this.statepanel.Margin = new Padding(4);
            this.statepanel.Name = "statepanel";
            this.statepanel.Size = new System.Drawing.Size(476, 389);
            this.statepanel.TabIndex = 0;
            
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.followTab());
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Dock = DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new Padding(4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(724, 460);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.button5);
            this.tabPage5.Location = new System.Drawing.Point(4, 25);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(716, 431);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "Components";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(55, 47);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(75, 23);
            this.button5.TabIndex = 0;
            this.button5.Text = "button5x";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // BotInterface
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(724, 460);
            this.Controls.Add(this.tabControl1);
            this.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new Padding(2, 3, 2, 3);
            this.MaximizeBox = false;
            this.Name = "BotInterface";
            this.Text = "BotInterface";
            this.FormClosing += new FormClosingEventHandler(this.BotInterface_FormClosing);
            this.Load += new System.EventHandler(this.BotInterface_Load);
            
            //this.followTab.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private TabPage tabPage4;
        private GroupBox groupBox2;
        private CheckBox checkBox8;
        private CheckBox checkBox7;
        private CheckBox checkBox6;
        private CheckBox checkBox5;
        private CheckBox checkBox4;
        private TabPage tabPage2;
        private Button button2;
        private ComboBox comboBox2;
        private Button button1;
        private CheckBox checkBox3;
        private CheckBox checkBox2;
        private RichTextBox richTextBox2;
        private RichTextBox richTextBox1;
        private CheckBox checkBox1;
        private TabPage tabPage1;
        private Panel panel1;
        private Button button4;
        private Button button3;
        private ComboBox comboBox1;
        private GroupBox groupBox1;
        private Panel statepanel;
        private TabControl tabControl1;
        private TabPage tabPage5;
        private Button button5;
    }
}