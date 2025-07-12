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
            Console.WriteLine("FollowTarget initialized for " + Client.Attributes.PlayerName);
        }

        // add get/set to this
        public Leader m_target = null;

        public void setTarget(Leader leader)
        {
            if (leader == null)
            {
                Console.WriteLine(Client.Attributes.PlayerName + " is no longer following anyone.");
                
            }
            else
            {
                Console.WriteLine(Client.Attributes.PlayerName + " is now following " + leader.Name);
            }
            m_target = leader;
        }

        public class Leader
        {
            public string Name { get; set; }
            public int Serial { get; set; }

            [Browsable(false)]
            public GameClient Client { get; set; }
            
            public override string ToString()
            {
                return Name;
            }
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
                if (Client.FieldMap != null && Client.IsInGame()
                    && Client.MapLoaded && Client.Utilities.CanWalk())
                {

                    if (Client.ObjectSearcher != null)
                    {
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
            if (Enabled 
                && !InTransition 
                && m_target != null 
                && m_target.Client != null 
                && m_target.Client.Attributes != null
                && m_target.Client.Attributes.ServerPosition != null)
            {
                InTransition = true;
                
                /*
                 While m_target walks around a given map their client updates their position in real-time.
                 This game state runs in a separate thread under the context of a separate client and continuously
                 checks their current position.
                 If both clients are on the same map then this can simply follow their coordinates and remain
                 at the specified distance.
                 However once the target client changes maps their current position is not a valid location for this
                 client to move to and this clcient needs to move to exactly where the target client was last seen on
                 their previous map ignoring follow distance.
                */
                
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
                
                if (path != null)
                {
                    Client.Utilities.ComputeStep(path, Distance);
                }
            }
            Client.TransitionTo(this, Elapsed);
        }
    }
}
