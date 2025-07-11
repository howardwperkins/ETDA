using BotCore.States.BotStates;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using BotCore.Types;

namespace BotCore.States
{
    public class FollowCollectionEditor : CollectionEditor
    {
        public FollowCollectionEditor(Type type)
            : base(type)
        {
        }

        protected override string GetDisplayText(object value)
        {
            FollowTarget.Leader item = new FollowTarget.Leader();
            item = (FollowTarget.Leader)value;

            return base.GetDisplayText(string.Format("{0}, {1}", item.Name,
                item.Serial));
        }
    }

    [State(Author: "Dean", Desc: "Will follow a target at a distance specified.")]
    public class FollowTarget : GameState
    {
        [Editor(typeof(FollowCollectionEditor),
        typeof(System.Drawing.Design.UITypeEditor))]
        public List<Leader> Leaders { get; set; }

        public override void InitState()
        {
            Dictionary<int, Client> leaders = new Dictionary<int, Client>();
            leaders = Collections.AttachedClients;
            
            // loop through leaders and print their key and name and serial
            foreach (var leader in leaders)
            {
                Console.WriteLine($"Leader: {leader.Key}, Name: {leader.Value.Attributes.PlayerName}, Serial: {leader.Value.Attributes.Serial}");
            }
            
            /*
            Leaders.AddRange(Client.OtherClients.Select(i => new Leader()
            {
                Name = i.Attributes.PlayerName,
                Serial = i.Attributes.Serial,
                Client = i.Client
            }));*/
        }

        public Leader m_target = null;

        public class Leader
        {
            public string Name { get; set; }
            public int Serial { get; set; }

            [Browsable(false)]
            public GameClient Client { get; set; }
        }

        private int m_Followinstance = 2;
        [Description("Trail Distance"), Category("Following Conditions")]
        public int Distance
        {
            get { return m_Followinstance; }
            set { m_Followinstance = value; }
        }

        public override bool NeedToRun
        {
            get
            {
                var closest = (from v in Leaders
                               where v.Client.IsInGame() && v.Name != Client.Attributes.PlayerName
                               orderby Client.Attributes.ServerPosition.DistanceFrom(v.Client.Attributes.ServerPosition)
                               select v).FirstOrDefault();

                if (Client.FieldMap != null && Client.IsInGame()
                    && Client.MapLoaded && Client.Utilities.CanWalk())
                {

                    if (Client.ObjectSearcher != null)
                    {
                        m_target = closest;

                        if (m_target != null)
                        {
                            //determine if should follow?
                            if (Client.MapId != m_target.Client.MapId)
                            {
                                Console.WriteLine("-> " + m_target.Name + " in " + m_target.Client.MapName);
                                return true;
                            }
                            
                            if (Client.Attributes?
                                .ServerPosition?
                                .DistanceFrom(m_target?.Client.Attributes.ServerPosition) > Distance
                                && m_target?.Client.FieldMap.MapNumber() == Client.FieldMap.MapNumber())
                                return true;
                        }
                    }

                    return false;
                }
                else
                {
                    return false;
                }
            }
            set
            {

            }
        }

        public override int Priority { get; set; }

        public Random rnd = new Random();

        private Position m_targetLastKnownPosition = null;
        private Direction m_targetLastKnownDirection = Direction.None;
        private short m_targetLastKnownMap = 0;
        private string m_targetLastKnownMapName = null;
        
        public override void Run(TimeSpan Elapsed)
        {
            if (Enabled && !InTransition)
            {
                InTransition = true;

                if (m_target != null)
                {
                    var myMap = Client.MapId;
                    
                    var targetMap = m_target.Client.MapId;
                    string targetMapName = m_target.Client.MapName;
                    var targetX = m_target.Client.Attributes?.ServerPosition.X;
                    var targetY = m_target.Client.Attributes?.ServerPosition.Y;
                    Direction targetDirection = m_target.Client.Attributes.Direction;
                    
                    Console.WriteLine("-> " + m_target.Name + " to " + targetMapName + "[" + targetX + "," + targetY + "]");
                    
                    Position endPosition = null;
                    if (myMap != targetMap)
                    {
                        if (m_targetLastKnownPosition == null)
                        {
                            // if we don't know the last position we can't follow
                            Console.WriteLine("No last known position, cannot follow.");
                        }
                        else
                        {
                            endPosition = m_targetLastKnownPosition;
                            Console.WriteLine(m_target.Name + " last seen in " + m_targetLastKnownMapName + " @ " + m_targetLastKnownPosition + " facing " + m_targetLastKnownDirection);
                        }
                    }
                    else
                    {
                        // maps are the same, so remember this position
                        endPosition = m_target.Client.Attributes?.ServerPosition;
                        m_targetLastKnownPosition = endPosition;
                        m_targetLastKnownDirection = targetDirection;
                        m_targetLastKnownMap = targetMap;
                        m_targetLastKnownMapName = targetMapName;
                    }
                    
                    var path = Client.FieldMap.Search(Client.Attributes.ServerPosition, endPosition);
                 // loop through path and print the xy coordinates
                    if (path != null)
                    {
                        Console.WriteLine("Path found:");
                        foreach (var step in path)
                        {
                            Console.WriteLine($"Step to: {step.X}, {step.Y}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("No path found.");
                    }
                    
                    if (path != null)
                    {
                        Client.Utilities.ComputeStep(path, Distance);
                    }
                }
            }
            Client.TransitionTo(this, Elapsed);
        }
    }
}
