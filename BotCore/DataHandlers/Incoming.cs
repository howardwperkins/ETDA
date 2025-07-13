﻿using BotCore.Actions;
using BotCore.Types;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using BotCore.Components;
using BotCore.States;

namespace BotCore.DataHandlers
{
    public static class Incoming
    {
        /// <summary>
        /// This handles the MapLoaded packet 0x15.
        ///
        /// This packet is sent by the server when the game first loads, when the player loads into a new map,
        /// or when the player presses F5 to reload the current map.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="packet"></param>
        public static void MapLoaded(object sender, Packet packet)
        {
            var client = Collections.AttachedClients[(int)sender];
            client.MapLoaded = false;

            var number = packet.ReadInt16();
            var width = packet.ReadByte();
            var height = packet.ReadByte();
            var name = string.Empty;

            packet.ReadByte();
            packet.ReadByte();
            packet.ReadByte();
            packet.ReadUInt16();
            name = packet.ReadString8();

            new Thread(delegate()
               {
                   LoadMap(client, number, width, height);
                   client.MapLoaded = true;
                   client.MapId = number;
                   client.MapName = name;
                   //reset active bar pointer
                   client.Active?.HardReset();
                   //Console.WriteLine("Map Loaded: " + name + " (" + number + ")");
                   //todo: reset any other pointers here on new map load.
                   ;
               }) { IsBackground = true }.Start();

            GameActions.RequestProfile(client);
        }

        /// <summary>
        /// This handles the ClientLocationUpdated packet 0x04.
        ///
        /// This packet is sent by the server after the MapLoaded packet and tells the client where the player is
        /// located on the newly loaded map.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="packet"></param>
        public static void ClientLocationUpdated(object sender, Packet packet)
        {
            var client = Collections.AttachedClients[(int)sender];
            var x = (short)packet.ReadUInt16();
            var y = (short)packet.ReadUInt16();

            var oldPosition = client.Attributes.ServerPosition;
            var newPosition = new Position(x, y);
            
            client.Attributes.ServerPosition = newPosition;
            
            foreach (var otherClient in Collections.AttachedClients.Values)
            {
                if (otherClient == client) continue; // Skip the client that moved
        
                var followState = otherClient.StateMachine.States
                    .OfType<FollowTarget>()
                    .FirstOrDefault();
            
                if (followState != null && followState.Leader?.Client == client)
                {
                    int followerMapId = followState.Client.MapId;
                    if (oldPosition != null 
                        && (oldPosition.X != newPosition.X || oldPosition.Y != newPosition.Y)
                        )
                    {
                        // We have to assume that the leader has entered a new map or moved significantly.
                        // We need to drop a breadcrumb one cell toward where end of trail is facing so that
                        // followers will walk into a presumed door.
                        var breadcrumbPosition = new Position(oldPosition.X, oldPosition.Y);
                        breadcrumbPosition.IsDoor = true;
                        breadcrumbPosition.Direction = oldPosition.Direction;
                        
                        switch (breadcrumbPosition.Direction)
                        {
                            case Direction.South:
                                breadcrumbPosition.Y++;
                                break;
                            case Direction.West:
                                breadcrumbPosition.X--;
                                break;
                            case Direction.East:
                                breadcrumbPosition.X++;
                                break;
                            case Direction.North:
                                breadcrumbPosition.Y--;
                                break;
                        }
                        
                        followState.Leader.DropBreadcrumbsToFollowers(followerMapId, breadcrumbPosition);
                        Console.WriteLine("Breadcrumb Dropped: {0} ({1},{2})", breadcrumbPosition.Direction, breadcrumbPosition.X, breadcrumbPosition.Y);
                    }
                }
            }
            
            if (!client.MapLoaded)
                return;
            
            var obj = client.FieldMap.GetObject(i => i.Serial == client.Attributes.Serial);
            if (obj != null)
            {
                obj.ServerPosition = newPosition;
                obj.OnPositionUpdated(client, oldPosition, newPosition);
            }
        }

        /// <summary>
        /// This handles the ClientPlayerWalked packet 0x0B.
        ///
        /// This packet is sent by the server after the player moves to a different coordinate on the map.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="packet"></param>
        public static void ClientPlayerWalked(object sender, Packet packet)
        {
            var client = Collections.AttachedClients[(int)sender];
            var direction = packet.ReadByte();
            var x = packet.ReadInt16();
            var y = packet.ReadInt16();
            
            Position oldPosition = new Position(x, y);
            Position newPosition;

            switch ((Direction)direction)
            {
                case Direction.South:
                    y++;
                    break;
                case Direction.West:
                    x--;
                    break;
                case Direction.East:
                    x++;
                    break;
                case Direction.North:
                    y--;
                    break;
            }

            //Console.WriteLine("ClientPlayerWalked: {0} ({1},{2})", (Direction)direction, x, y);
            
            client.Attributes.ServerPosition = newPosition = new Position(x, y);
            client.Attributes.ServerPosition.Direction = (Direction)direction;
            Console.WriteLine("Walked to ({0},{1})", newPosition.X, newPosition.Y);
            client.Attributes.Direction = (Direction)direction;

            /*// Drop a Breadcrumb for FollowTarget followers
            foreach (var otherClient in Collections.AttachedClients.Values)
            {
                if (otherClient == client) continue; // Skip the client that moved
        
                var followState = otherClient.StateMachine.States
                    .OfType<FollowTarget>()
                    .FirstOrDefault();
            
                if (followState != null && followState.Leader?.Client == client)
                {
                    // This client is following the one that moved, so drop breadcrumbs
                    followState.Leader.DropBreadcrumbsToFollowers();
                }
            }*/
            
            var obj = client.FieldMap.GetObject(i => i.Serial == client.Attributes.Serial);
            if (obj != null)
            {
                obj.ServerPosition = newPosition;
                obj.OnPositionUpdated(client, oldPosition, newPosition);
            }

            client.Steps++;
        }
        
        public static void ObjectWalked(object sender, Packet e)
        {
            var client = Collections.AttachedClients[(int)sender];
            var serial = e.ReadInt32();
            var x = e.ReadInt16();
            var y = e.ReadInt16();
            var d = (Direction)e.ReadByte();
        }
        
        public static void _ObjectWalked(object sender, Packet e)
        {
            var client = Collections.AttachedClients[(int)sender];
            var serial = e.ReadInt32();
            var x = e.ReadInt16();
            var y = e.ReadInt16();
            var d = (Direction)e.ReadByte();
            var oldPosition = new Position(x, y);
            var newPosition = new Position(0, 0);
            var obj = client.FieldMap.GetObject(i => i.Serial == serial);
            if (obj == null)
                return;

            switch (d)
            {
                case Direction.South:
                    y++;
                    break;
                case Direction.North:
                    y--;
                    break;
                case Direction.West:
                    x--;
                    break;
                case Direction.East:
                    x++;
                    break;
            }

            newPosition.X = x;
            newPosition.Y = y;

            obj.Direction = d;
            obj.ServerPosition = newPosition;
            obj.OldPosition    = oldPosition;
            obj.OnPositionUpdated(client, oldPosition, newPosition);
        }

        internal static void Stats(object sender, Packet packet)
        {
            var client = Collections.AttachedClients[(int)sender];

            //08 14 00 03 73 7C 00 00 50 98 00 08 00 00 00 00 00 06 00 01 50 00 00}
            if (packet.Data.Length >= 11 && packet.Data[11] == 8)
            {
                packet.Data[11] = 0x07;

                //return without sending packet ect
            }

        }

        internal static void WorldList(object sender, Packet e)
        {
            var client = Collections.AttachedClients[(int)sender];
            var count = e.ReadInt16();
            var local = e.ReadInt16();

            var users = new List<string>();

            for (int i = 0; i < local * 2; i++)
            {
                var icon = e.ReadByte();
                var color = e.ReadByte();
                var name = e.ReadString8();

                if (!string.IsNullOrEmpty(name))
                {
                    if (name.ToLower() == "reich" || name.ToLower() == "acnologia")
                        continue;

                    users.Add(name);
                }
            }

            client.LocalWorldUsers = users;
        }

        internal static void EquipmentUpdated(object sender, Packet e)
        {
            var client = Collections.AttachedClients[(int)sender];
            var slot = e.ReadByte();
            e.ReadInt16();
            e.ReadByte();
            var name = e.ReadString8();


            if (slot == 1)
            {
                if (Collections.BaseStaffs.ContainsKey(name))
                {
                    client.EquippedWeaponId = Collections.BaseStaffs[name].Id;
                    client.EquippedWeapon = name;
                    client.LastEquipmentUpdate = DateTime.Now;
                }
            }
        }

        internal static void LoadingMap(object sender, Packet e)
        {
            var client = Collections.AttachedClients[(int)sender];
            client.MapLoading = true;
        }

        public static void PlayerSerialAssigned(object sender, Packet e)
        {
            var client = Collections.AttachedClients[(int)sender];
            var gameid = e.ReadUInt32();
        }

        public static void ProfileRequested(object sender, Packet packet)
        {
            var client = Collections.AttachedClients[(int)sender];
            var nation = packet.ReadByte();
            packet.ReadString8();
            packet.ReadString8(); 

            var text = packet.ReadString8();
            packet.ReadBoolean();

            if (packet.ReadBoolean())
            {
                packet.ReadString8();
                packet.ReadString8();
                packet.ReadString8();
                packet.ReadByte();
                packet.ReadByte();
                packet.ReadByte();
                packet.ReadByte();
                packet.ReadByte();
                packet.ReadByte();
                packet.ReadByte();
                packet.ReadByte();
                packet.ReadByte();
                packet.ReadByte();
                packet.ReadByte();
                packet.ReadByte();
            }

            packet.ReadByte();
            var classid = packet.ReadByte();
            packet.ReadByte();
            packet.ReadString8();

            client.Attributes.Guild = packet.ReadString8();
            client.Attributes.Class = classid;
            client.Attributes.Nation = nation;
            client.Attributes.Group = new StringBuilder(text);

            var str = text.ToString();
            var m = str.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 1; i < m.Length - 1; i++)
            {
                if (m[i].Contains("*"))
                    m[i] = m[i].Replace("*", string.Empty);

                m[i] = m[i].Trim();

                if (m[i] != client.Attributes.PlayerName)
                {
                    client.Attributes.AddGroupMember(m[i]);
                }
            }
        }

        public static void Sidebar(object sender, Packet packet)
        {
            var client = Collections.AttachedClients[(int)sender];
            var icon   = packet.ReadInt16();
            var color  = packet.ReadByte();

            if (color < 0)
            {
                if (client.SpellBar.Contains(icon))
                {
                    client.SpellBar.Remove(icon);
                }
            }

            if (color > 0)
            {
                if (!client.SpellBar.Contains(icon))
                {
                    client.SpellBar.Add(icon);
                }
            }
        }

        public static void Barmessage(object sender, Packet packet)
        {
            var client  = Collections.AttachedClients[(int)sender];
            var type    = packet.ReadByte();
            var mode    = packet.ReadByte();
            var message = packet.ReadString8();

            client.MessageMachine.HandleSystemMessage(client, message);
            message = string.Empty;
        }

        public static void EntitiesChangedDirection(object sender, Packet packet)
        {
            var client = Collections.AttachedClients[(int)sender];
            var serial = packet.ReadInt32();
            var direction = packet.ReadByte();

            if (client.Attributes.Serial == serial)
                client.Attributes.Direction = (Direction)direction;

            var obj = client.FieldMap.GetObject(i => i.Serial == serial);
            if (obj == null)
                return;

            obj.Direction = (Direction)direction;
        }

        public static void AislingsAdded(object sender, Packet packet)
        {
            var client = Collections.AttachedClients[(int)sender];
            var x = packet.ReadInt16();
            var y = packet.ReadInt16();
            var direction = packet.ReadByte();
            var serial = packet.ReadInt32();
            var visible = packet.ReadUInt16();


            if (serial == client.Attributes.Serial)
            {
                client.Attributes.ServerPosition = new Position(x, y);
                client.Attributes.Direction = (Direction)direction;
            }

            if (visible == ushort.MaxValue)
            {
                packet.ReadUInt16();
                packet.ReadByte();
                packet.ReadByte();
                packet.Read(6);
            }
            else
            {
                packet.ReadUInt16();
                packet.ReadByte();
                packet.ReadUInt16();
                packet.ReadByte();
                packet.ReadInt16();
                var a = packet.ReadByte();

                if (a > 0 && client.EquippedWeaponId != a)
                {
                    client.EquippedWeaponId = a;
                }
                packet.ReadByte();
                packet.ReadByte();
                packet.ReadUInt16();
                packet.ReadByte();
                packet.ReadUInt16();
                packet.ReadByte();
                packet.ReadUInt16();
                packet.ReadByte();
                packet.ReadByte();
                packet.ReadUInt16();
                packet.ReadByte();
                packet.ReadByte();
                packet.ReadBoolean();
                packet.ReadByte();
            }
            packet.ReadByte();

            var name = string.Empty;
            if (visible == 0xFFFF)
                name = packet.ReadString8();
            else
            {
                var mm = packet.ReadString8();
                name = packet.ReadString8();
            }

            var obj = new Aisling(name);
            obj.Serial = serial;
            obj.ServerPosition = new Position(x, y);
            obj.Direction = (Direction)direction;

            if (packet.Data[10] == 0x00 &&
                  packet.Data[11] == 0x00 &&
                  packet.Data[12] == 0x00 &&
                  packet.Data[13] == 0x00)
            { 
                if (packet[11] != 0x97)
                {
                    packet[10] = 0x00;
                    packet[11] = 0x97;
                    packet[12] = 0x13;
                }

                obj.IsHidden = true;
            }

            obj.OnDiscovery(client);

        }

        public static void ChatMessages(object sender, Packet packet)
        {
            var client = Collections.AttachedClients[(int)sender];
            var type = packet.ReadByte();
            var id   = packet.ReadInt32();
            var msg  = packet.ReadString8();

        }

        public static void PlaySound(object sender, Packet packet)
        {
            var audio = packet.ReadByte();
        }

        public static void PlayerAction(object sender, Packet packet)
        {
            var serial = packet.ReadInt32();
            var action = packet.ReadByte();
            var time = packet.ReadUInt16();
        }

        public static void InitializeMapLoad(Client client)
        {
            var number = client.FieldMap.MapNumber();
            var width = client.FieldMap.MapWidth();
            var height = client.FieldMap.MapHeight();
            LoadMap(client, number, width, height);
        }

        private static void LoadMap(Client client, short number, short width, short height)
        {
            var path = Path.Combine(Collections.DAPATH, "maps") + "\\lod" + number.ToString(CultureInfo.InvariantCulture) + ".map";

            if (File.Exists(path))
                PrepareMap(client, number, width, height, path);
        }

        private static void PrepareMap(Client client, short number, short width, short height, string path)
        {
            Map oldMap = client.FieldMap;
            GameClient oldClient = oldMap.Client;

            Map newMap = new Map();
            newMap.Enabled = true;
            newMap.Init(0, 0, 0);
            newMap.Enabled = true;
            newMap.Ready = false;
            newMap.Client = oldClient;
            newMap.Client.FieldMap = newMap;

            client.FieldMap = newMap;
            client.FieldMap.OnMapLoaded(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite), number, width, height);
            client.FieldMap.Ready = true;
            
            // Replace in InstalledComponents
            if (client.InstalledComponents.Contains(oldMap))
                client.InstalledComponents.Remove(oldMap);
            client.InstalledComponents.Add(client.FieldMap);
        }

        public static void ObjectRemoved(object sender, Packet e)
        {
            var client = Collections.AttachedClients[(int)sender];
            var ObjectId = e.ReadInt32();
            var mapobject = client.FieldMap.GetObject(i => i.Serial == ObjectId);
            OnObjectRemoved(client, ObjectId, mapobject);
        }

        private static void OnObjectRemoved(Client client, int ObjectId, MapObject mapobject)
        {
            if (mapobject != null)
            {
                mapobject.OnRemoved(client);
            }
        }

        public static void Animation(object sender, Packet packet)
        {
            var client = Collections.AttachedClients[(int)sender];
            var to = packet.ReadInt32();
            var from = packet.ReadInt32();
            var animation = packet.ReadInt16();

            if (from == client.Attributes.Serial)
            {
                client.LastCastTarget = client.FieldMap?.GetObject(i => i.Serial == to);
            }

            var obj = client.FieldMap?.GetObject(i => i.Serial == to);
            if (obj != null)
            {
                obj.OnAnimation(animation, to, from);
            }
        }


        public static void EntitiesAdded(object sender, Packet packet)
        {
            var client = Collections.AttachedClients[(int)sender];
            var count = packet.ReadUInt16();
            var objects = new MapObject[count];

            for (var i = 0; i < (int)count; i++)
            {
                var X = packet.ReadInt16();
                var Y = packet.ReadInt16();
                var Serial = packet.ReadInt32();
                var Sprite = packet.ReadUInt16();
                packet.ReadByte();
                packet.ReadByte();
                packet.ReadByte();

                objects[i] = new MapObject();
                objects[i].Serial = Serial;
                objects[i].ServerPosition = new Position(X, Y);
                objects[i].Sprite = Sprite;

                if (Sprite < 32768)
                {
                    Sprite -= 16384;
                    packet.ReadByte();
                    var direction = packet.ReadByte();
                    packet.ReadByte();
                    var Type = packet.ReadByte();

                    objects[i].Direction = (Direction)direction;

                    if (Type == 2)
                    {
                        var Name = packet.ReadString8();
                        objects[i].Sprite = Sprite;
                        objects[i].Type = MapObjectType.NPC;
                    }
                    else
                    {
                        objects[i].Sprite = Sprite;
                        objects[i].Type = Type == 0 ? MapObjectType.Monster : MapObjectType.Pet;
                    }
                }
                else
                {
                    Sprite -= 32768;
                    objects[i].Sprite = Sprite;
                    objects[i].Type = MapObjectType.Item;
                }
                objects[i].OnDiscovery(client);
            }
        }


    }
}
