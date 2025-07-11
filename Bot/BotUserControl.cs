using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
        using BotCore;
        using BotCore.States;

        namespace Bot
        {
            public class BotUserControl : UserControl
            {
                public Client Client;
                public ListBox LeaderListBox;
                private readonly TabControl _tabControl;
                public FollowTarget.Leader Leader;
                public BotUserControl(Client client)
                {
                    Client = client;
                    Leader = new FollowTarget.Leader
                    {
                        Name = Client.Attributes.PlayerName,
                        Serial = Client.Attributes.Serial,
                        Client = Client
                    };
                    
                    Dock = DockStyle.Fill;
                    
                    _tabControl = new TabControl
                    {
                        Dock = DockStyle.Fill,
                        Font = new Font("Arial", 10, FontStyle.Regular)
                    };
        
                    // Follow tab
                    var followTab = new TabPage("Follow");
                    var leaderLabel = new Label
                    {
                        Text = "Leader for " + Client.Attributes.PlayerName,
                        Dock = DockStyle.Top,
                        Height = 20,
                        TextAlign = System.Drawing.ContentAlignment.MiddleLeft
                    };
                    LeaderListBox = new ListBox
                    {
                        Dock = DockStyle.Fill
                    };
                    LeaderListBox.Items.Add("<None>");
                    LeaderListBox.SelectedIndex = 0;
                    
                    LeaderListBox.SelectedIndexChanged += (sender, e) =>
                    {
                        var followState = Client.StateMachine.States
                            .OfType<FollowTarget>()
                            .FirstOrDefault();
                        
                        if (followState != null)
                        {
                            if (LeaderListBox.SelectedItem is FollowTarget.Leader leader)
                            {
                                followState.setTarget(leader);
                                followState.Enabled = true;
                            }
                            else
                            {
                                followState.Enabled = false;
                                followState.setTarget(null);
                            }
                        }
                    };
                    
                    followTab.Controls.Add(LeaderListBox);
                    followTab.Controls.Add(leaderLabel);
        
                    // Other tabs
                    var buffTab = new TabPage("Buff");
                    var debuffTab = new TabPage("Debuff");
                    var healTab = new TabPage("Heal");
        
                    _tabControl.TabPages.Add(followTab);
                    _tabControl.TabPages.Add(buffTab);
                    _tabControl.TabPages.Add(debuffTab);
                    _tabControl.TabPages.Add(healTab);
        
                    Controls.Add(_tabControl);
                }
                
                public void IntroduceBot(BotUserControl newBot)
                {
                    if (Client != newBot.Client)
                    {
                        newBot.LeaderListBox.Items.Add(Leader);
                        LeaderListBox.Items.Add(newBot.Leader);
                    }
                }
            }
        }