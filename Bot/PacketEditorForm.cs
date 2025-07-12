using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BotCore;
using BotCore.Shared.Memory;
using BotCore.Types;

namespace Bot
{
    public partial class PacketEditorForm : Form
    {
        private Client client;
        private bool EnablePacketEditor = false;
        private bool IsReceivingPackets = false;
        private bool IsSendingPackets = false;
        private bool pausePackets = false;

        private RichTextBox richTextBox1;
        private RichTextBox richTextBox2;
        private CheckBox checkBox1;
        private CheckBox checkBox2;
        private CheckBox checkBox3;
        private ComboBox comboBox2;
        private Button button1;
        private Button button2;

        public PacketEditorForm(Client client)
        {
            this.client = client;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form properties
            this.Text = "Packet Editor";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterParent;

            // richTextBox1 - packet display
            this.richTextBox1 = new RichTextBox();
            this.richTextBox1.BackColor = Color.White;
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Location = new Point(12, 12);
            this.richTextBox1.Size = new Size(760, 300);
            this.richTextBox1.MouseDown += richTextBox1_MouseDown;
            this.richTextBox1.MouseUp += richTextBox1_MouseUp;

            // richTextBox2 - packet input
            this.richTextBox2 = new RichTextBox();
            this.richTextBox2.Location = new Point(12, 380);
            this.richTextBox2.Size = new Size(760, 120);

            // checkBox1 - Enable packet editor
            this.checkBox1 = new CheckBox();
            this.checkBox1.Text = "Enable Packet Editor";
            this.checkBox1.Location = new Point(12, 320);
            this.checkBox1.CheckedChanged += checkBox1_CheckedChanged;

            // checkBox2 - Sending packets
            this.checkBox2 = new CheckBox();
            this.checkBox2.Text = "Sending Packets";
            this.checkBox2.Location = new Point(150, 320);
            this.checkBox2.CheckedChanged += checkBox2_CheckedChanged;

            // checkBox3 - Receiving packets
            this.checkBox3 = new CheckBox();
            this.checkBox3.Text = "Receiving Packets";
            this.checkBox3.Location = new Point(280, 320);
            this.checkBox3.CheckedChanged += checkBox3_CheckedChanged;

            // comboBox2 - packet direction
            this.comboBox2 = new ComboBox();
            this.comboBox2.Items.AddRange(new string[] { "Auto", "Client", "Server" });
            this.comboBox2.SelectedIndex = 0;
            this.comboBox2.Location = new Point(12, 350);
            this.comboBox2.Size = new Size(100, 21);

            // button1 - Send packet
            this.button1 = new Button();
            this.button1.Text = "Send";
            this.button1.Location = new Point(120, 348);
            this.button1.Size = new Size(75, 23);
            this.button1.Click += button1_Click;

            // button2 - Clear
            this.button2 = new Button();
            this.button2.Text = "Clear";
            this.button2.Location = new Point(200, 348);
            this.button2.Size = new Size(75, 23);
            this.button2.Click += button2_Click;

            // Add controls to form
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.richTextBox2);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.checkBox3);
            this.Controls.Add(this.comboBox2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button2);

            this.ResumeLayout(false);
        }

        public void AppendText(RichTextBox box, string text, Color color, bool AddNewLine = false)
        {
            if (box.Lines.Length > 256)
                box.Clear();

            if (pausePackets)
                return;

            if (AddNewLine)
                text += Environment.NewLine;

            box.SelectionColor = color;
            box.AppendText(text);
            
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            EnablePacketEditor = !EnablePacketEditor;

            if (EnablePacketEditor)
            {
                client.OnPacketRecevied += EditorReceivedPacket;
                client.OnPacketSent += EditorSentPacket;
                richTextBox1.Enabled = true;
            }
            else
            {
                client.OnPacketRecevied -= EditorReceivedPacket;
                client.OnPacketSent -= EditorSentPacket;
                richTextBox1.Enabled = false;
            }
        }

        private void EditorSentPacket(object sender, Packet e)
        {
            if (!IsSendingPackets)
                return;

            AppendText(richTextBox1, "s: " + e.ToString(), Color.Blue, true);
        }

        private void EditorReceivedPacket(object sender, Packet e)
        {
            if (!IsReceivingPackets)
                return;

            AppendText(richTextBox1, "r: " + e.ToString(), Color.Red, true);
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            IsReceivingPackets = !IsReceivingPackets;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            IsSendingPackets = !IsSendingPackets;
        }

        public static byte[] StringToByteArray(string hex)
        {
            try
            {
                return Enumerable.Range(0, hex.Length)
                    .Where(x => x % 2 == 0)
                    .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                    .ToArray();
            }
            catch (Exception)
            {
                throw new InvalidOperationException("Invalid packet");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (var line in richTextBox2.Lines)
            {
                try
                {
                    byte[] buffer = null;
                    var newline = line;

                    if (comboBox2.Text == "Auto")
                    {
                        bool outgoing = false;

                        if (line.StartsWith("r:"))
                            newline = line.Replace("r:", string.Empty);
                        else if (line.StartsWith("s:"))
                        {
                            newline = line.Replace("s:", string.Empty);
                            outgoing = true;
                        }

                        newline = newline.Trim();
                        buffer = StringToByteArray(newline.Replace(" ", string.Empty).Trim());
                        if (buffer != null && buffer.Length <= 0)
                            continue;

                        if (outgoing)
                            GameClient.InjectPacket<ServerPacket>(client, new Packet(buffer), true);
                        else
                            GameClient.InjectPacket<ClientPacket>(client, new Packet(buffer), true);

                        continue;
                    }

                    if (line.StartsWith("r:"))
                        newline = line.Replace("r:", string.Empty);
                    else if (line.StartsWith("s:"))
                        newline = line.Replace("s:", string.Empty);

                    buffer = StringToByteArray(newline.Replace(" ", string.Empty).Trim());
                    if (buffer != null && buffer.Length <= 0)
                        continue;

                    if (comboBox2.Text == "Client")
                        GameClient.InjectPacket<ClientPacket>(client, new Packet(buffer), true);
                    if (comboBox2.Text == "Server")
                        GameClient.InjectPacket<ServerPacket>(client, new Packet(buffer), true);
                }
                catch (InvalidOperationException ex)
                {
                    Console.Error.WriteLine(ex.Message);
                    Console.Error.WriteLine(ex.StackTrace);
                    continue;
                }
            }
        }

        private void richTextBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                pausePackets = true;
        }

        private void richTextBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && pausePackets)
                pausePackets = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
        }
    }
}