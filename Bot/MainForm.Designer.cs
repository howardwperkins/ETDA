using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Bot
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private TabControl tabControl1;
        private ToolStrip toolStrip1;
        
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

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.tabControl1 = new TabControl();
            this.SuspendLayout();
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 400);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = false;
            this.Margin = new Padding(0, 0, 0, 0);
            this.Name = "MainForm";
            this.Text = "etda";
            this.FormClosing += new FormClosingEventHandler(this.MainForm_FormClosing);
            this.ResumeLayout(false);
            
            // Initialize components
            this.toolStrip1 = new ToolStrip();
            this.tabControl1 = new TabControl();

            //
            // toolStrip1
            //
            this.toolStrip1.Dock = DockStyle.Top;
            this.toolStrip1.Items.Add(new ToolStripButton("Button1"));
            this.toolStrip1.Items.Add(new ToolStripButton("Button2"));
            
            // 
            // tabControl1
            // 
            //this.tabControl1.Alignment = TabAlignment.Left;
            this.tabControl1.Dock = DockStyle.Fill;
            this.tabControl1.Multiline = true;
            this.tabControl1.SizeMode = TabSizeMode.Fixed;
            this.tabControl1.ItemSize = new System.Drawing.Size(160, 40); // Adjust as needed
            //this.tabControl1.Width = this.Width; // Adjust width as needed
            this.tabControl1.DrawMode = TabDrawMode.OwnerDrawFixed;
            this.tabControl1.Font = new System.Drawing.Font("Courier New", 12, System.Drawing.FontStyle.Regular);
            this.tabControl1.DrawItem += new DrawItemEventHandler(this.tabControl1_DrawItem);
            // Add the TabControl to the form
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.toolStrip1);
        }
        
        private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            TabControl tabControl = sender as TabControl;
            TabPage tabPage = tabControl.TabPages[e.Index];
            Rectangle tabBounds = tabControl.GetTabRect(e.Index);
        
            // Fill background for selected tab
            if (tabControl.SelectedIndex == e.Index)
            {
                using (Brush bgBrush = new SolidBrush(Color.LightBlue))
                {
                    e.Graphics.FillRectangle(bgBrush, tabBounds);
                }
            }
            else
            {
                using (Brush bgBrush = new SolidBrush(tabPage.BackColor))
                {
                    e.Graphics.FillRectangle(bgBrush, tabBounds);
                }
            }
        
            // Draw tab text
            Font font = tabControl.SelectedIndex == e.Index
                ? new Font(e.Font, FontStyle.Bold)
                : e.Font;
        
            string tabText = tabPage.Text;
            using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            using (Brush textBrush = new SolidBrush(tabPage.ForeColor))
            {
                e.Graphics.DrawString(tabText, font, textBrush, tabBounds, sf);
            }
        }
    }
}