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
            _nameLabel = new Label
            {
                Text = _client.Attributes.PlayerName,
                Dock = DockStyle.Top,
                AutoSize = true
            };
            Controls.Add(_nameLabel);
        }
    }
}