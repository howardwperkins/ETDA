﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using BotCore.Actions;
using BotCore.DataHandlers;
using BotCore.States;
using BotCore.Shared.Memory;

namespace BotCore
{
    [Serializable]
    public class Client : GameClient
    {
        public Client()
        {
            Client = this;

            //Add any Client Extenders here.
            Client.SpellBar = new List<short>();
            Client.LocalWorldUsers = new List<string>();
        }

        public new Client OnAttached()
        {
            //Initialize Map from Cache
            Incoming.InitializeMapLoad(this);

            AddServerHandlers();
            AddClientHandlers();
            PreparePrelims();

            base.OnAttached();
            return this;
        }

        private void AddClientHandlers()
        {
            AddClientHandler(0x03, Outgoing.LoggingIn);
            AddClientHandler(0x0B, Outgoing.LoggingOut);
            AddClientHandler(0x1C, Outgoing.UseInventorySlot);
            AddClientHandler(0x0F, Outgoing.SpellCasted);
            AddClientHandler(0x4D, Outgoing.SpellBegin);
        }

        private void AddServerHandlers()
        {
            AddServerHandler(0x04, Incoming.ClientLocationUpdated);
            AddServerHandler(0x07, Incoming.EntitiesAdded);
            AddServerHandler(0x0A, Incoming.Barmessage);
            AddServerHandler(0x0B, Incoming.ClientPlayerWalked);
            AddServerHandler(0x0C, Incoming.ObjectWalked);
            AddServerHandler(0x0D, Incoming.ChatMessages);
            AddServerHandler(0x0E, Incoming.ObjectRemoved);
            AddServerHandler(0x11, Incoming.EntitiesChangedDirection);
            AddServerHandler(0x15, Incoming.MapLoaded);
            AddServerHandler(0x19, Incoming.PlaySound);
            AddServerHandler(0x1A, Incoming.PlayerAction);
            AddServerHandler(0x29, Incoming.Animation);
            AddServerHandler(0x33, Incoming.AislingsAdded);
            AddServerHandler(0x3A, Incoming.Sidebar);
            AddServerHandler(0x39, Incoming.ProfileRequested);
            AddServerHandler(0x05, Incoming.PlayerSerialAssigned);
            AddServerHandler(0x06, Incoming.LoadingMap);
            AddServerHandler(0x37, Incoming.EquipmentUpdated);
            AddServerHandler(0x08, Incoming.Stats);
            AddServerHandler(0x36, Incoming.WorldList);
        }

        private void PreparePrelims()
        {
            // TODO: Add new tabs for each Client to MainForm or any other UI component

            GameActions.Refresh(Client, true, (a, b) => true);
            GameActions.Refresh(Client, true, (a, b) => true);
        }

        //This is used to manage Auto Logging-In (If Enabled).
        internal void OnClientStateUpdated(bool transit)
        {
            if (ClientReady && !transit)
            {
                Client.Active?.HardReset();
                Client.CleanUpMememory();
            }

            Console.WriteLine("Client is ready.");
            ClientReady = transit;
        }

        public override void TransitionTo(GameState current, TimeSpan Elapsed)
        {
            current.InTransition = false;

            //we must signal that no states are running here.
            Client.RunningState = null;
        }
    }
}