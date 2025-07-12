using System;
using System.ComponentModel;
using BotCore.Types;

namespace BotCore.States
{

    public class FollowTarget : GameState
    {

        private Leader _leader;
        
        public Breadcrumbs Breadcrumbs = new Breadcrumbs();
        
        public int Distance { get; set; } = 2;
        private int runDistance;
        
        public Leader Leader
        {
            get
            {
                return _leader;
            }
            set
            {
                if (value == null)
                {
                    if (_leader != null)
                        _leader.RemoveFollower(Client);

                    _leader = null;
                    Enabled = false;
                    Console.WriteLine(Client.Attributes.PlayerName + " is no longer following anyone.");
                }
                else
                {
                    _leader = value;
                    _leader.AddFollower(Client);
                    Enabled = true;
                    Console.WriteLine(Client.Attributes.PlayerName + " is now following " + value.Name);
                }
            }
        }

        public override bool NeedToRun
        {
            get
            {
                if (Client.FieldMap == null
                    || !Client.IsInGame()
                    || Client.MapId == 0
                    || !Client.Utilities.CanWalk()
                    || Client.ObjectSearcher == null
                    || Leader == null)
                {
                    Console.WriteLine("" + Client.Attributes.PlayerName + " cannot follow target, missing prerequisites.");
                    return false;
                }
                
                var followDistance = Distance;
                if (Client.MapId != Leader.Client.MapId)
                {
                    followDistance = 0;
                }
                
                var myPosition = Client.Attributes?.ServerPosition;
                var peekTargetPosition = Breadcrumbs.PeekNextBreadcrumb(Client.MapId);

                if (peekTargetPosition != null 
                    && followDistance != 0 
                    && peekTargetPosition.DistanceFrom(Leader.Client.Attributes.ServerPosition) > followDistance)
                {
                    runDistance = 0;
                    return true;
                }
                
                if (peekTargetPosition == null
                    || !(myPosition?.DistanceFrom(peekTargetPosition) > followDistance)) return false;
                runDistance = followDistance;
                return true;

            }
            set
            {

            }
        }

        public override int Priority { get; set; }
        
        private Position m_targetLastKnownPosition = null;
        private Direction m_targetLastKnownDirection = Direction.None;
        private short m_targetLastKnownMap = 0;
        private string m_targetLastKnownMapName = null;
        
        public override void Run(TimeSpan Elapsed)
        {
            if (Enabled 
                && !InTransition)
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
                
                Position peekTargetPosition = Breadcrumbs.PeekNextBreadcrumb(Client.MapId);
                var path = Client.FieldMap.Search(Client.Attributes.ServerPosition, peekTargetPosition);
                
                if (path != null)
                {
                    Breadcrumbs.GetNextBreadcrumb(Client.MapId);
                    Client.Utilities.ComputeStep(path, runDistance);
                }
            }
            Client.TransitionTo(this, Elapsed);
        }
    }
}
