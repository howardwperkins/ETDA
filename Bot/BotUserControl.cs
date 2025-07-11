using System.Windows.Forms;
using BotCore;

namespace Bot
{
    public class BotUserControl : UserControl
    {
        private readonly Client _client;
        private readonly Label _nameLabel;

        public BotUserControl(Client client)
        {
            _client = client;
            Dock = DockStyle.Fill; // Fill the tab page
  
            _nameLabel = new Label
            {
                Text = _client.Attributes.PlayerName,
                
                AutoSize = false,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Font = new System.Drawing.Font("Segoe UI", 14, System.Drawing.FontStyle.Bold),
                Height = 40
            };
            Controls.Add(_nameLabel);
        }
    }
}