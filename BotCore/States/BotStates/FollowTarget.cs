using System;
using System.Collections.Generic;
using System.Threading;
using BotCore.Actions;
using BotCore.PathFinding;
using BotCore.Types;

namespace BotCore.States
{

    public class FollowTarget : GameState
    {

        private Leader _leader;
        
        public Breadcrumbs Breadcrumbs = new Breadcrumbs();
        
        public int Distance { get; set; } = 2;
        private short TargetMap = 0;
        private Position TargetPosition = null;
        private bool TargetIsBreadcrumb = false;
        
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
                    {
                        _leader.RemoveFollower(Client);
                        _leader = null;
                    }
                    
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
                if (InTransition
                    || Client.FieldMap == null
                    || !Client.IsInGame()
                    || Client.MapId == 0
                    || !Client.Utilities.CanWalk()
                    || Client.ObjectSearcher == null
                    || Leader == null)
                {
                    Console.WriteLine("" + Client.Attributes.PlayerName + " cannot follow target, missing prerequisites.");
                    return false;
                }

                if (Client.MapId == Leader.Client.MapId
                    && Client.Attributes.ServerPosition.IsNearby(Leader.Client.Attributes.ServerPosition, 2))
                {
                    TargetPosition = null;
                    TargetIsBreadcrumb = false;
                    return false;
                }
                
                if (Client.MapId == Leader.Client.MapId)
                {
                    Breadcrumbs.ClearAllBreadcrumbs();
                    TargetPosition = Leader.Client.Attributes.ServerPosition;
                    TargetIsBreadcrumb = false;
                    return true;
                }
                
                var peekTargetPosition = Breadcrumbs.GetBreadcrumb(Client.MapId);
                if (peekTargetPosition != null)
                {
                    TargetMap = Client.MapId;
                    TargetPosition = peekTargetPosition;
                    TargetIsBreadcrumb = true;
                    Console.WriteLine(Client.Attributes.PlayerName + " is following breadcrumb " + TargetMap + " @ (" + TargetPosition.X + "," + TargetPosition.Y + ")");
                    return true;
                }
                
                TargetPosition = null;
                TargetIsBreadcrumb = false;
                return false;
            }
            set
            {

            }
        }

        public override int Priority { get; set; }
        
        public override void Run(TimeSpan elapsed)
        {
            if (Enabled 
                && !InTransition)
            {
                InTransition = true;

                if (TargetPosition == null)
                {
                    Console.WriteLine(Client.Attributes.PlayerName + " has no target position to follow.");
                    InTransition = false;
                    return;
                }
                

                while (!DoneWalking())
                {
                    var path = Client.FieldMap.Search(Client.Attributes.ServerPosition, TargetPosition);
                    if (path == null || path.Count == 0)
                    {
                        Console.WriteLine(Client.Attributes.PlayerName + " NO PATH " + Client.Attributes.ServerPosition + "->" + TargetPosition);
                        InTransition = false;
                        return;
                    }
                    
                    if (!TargetIsBreadcrumb && Client.MapId != Leader.Client.MapId)
                    {
                        var peekTargetPosition = Breadcrumbs.GetBreadcrumb(Client.MapId);
                        if (peekTargetPosition != null)
                        {
                            TargetMap = Client.MapId;
                            TargetPosition = peekTargetPosition;
                            TargetIsBreadcrumb = true;
                            Console.WriteLine(Client.Attributes.PlayerName + " is following breadcrumb " + TargetMap + " @ (" + TargetPosition.X + "," + TargetPosition.Y + ")");
                            continue;
                        }
                        
                        Console.WriteLine(@"{0} is lost and doesn't know where to go.", Client.Attributes.PlayerName);
                        InTransition = false;
                        return;
                    }
                    WalkPath(path);
                    Thread.Sleep(200);
                }
            }
            Client.TransitionTo(this, elapsed);
        }

        private bool DoneWalking(int distance = 2)
        {
            if (TargetIsBreadcrumb && Client.MapId == Leader.Client.MapId)
            {
                var breadcrumbs = Breadcrumbs.GetBreadcrumb(TargetMap);
                Console.WriteLine("" + Client.Attributes.PlayerName + " picked up breadcrumb (" + breadcrumbs.X + "," + breadcrumbs.Y + ")");
                Breadcrumbs.ClearBreadcrumb(TargetMap);
                return true;
            }
            
            if (!TargetIsBreadcrumb)
            {
                return Client.Attributes.ServerPosition.IsNearby(TargetPosition, distance);
            }
            
            return false;
        }

        private void WalkPath(List<PathSolver.PathFinderNode> path, int distance = 1)
        {
            if (path == null || path.Count == 0)
                return;
            
            if (distance >= path.Count)
            {
                WalkPath(path, --distance);
                return;
            }
            
            if (distance < 0)
                return;
            
            var pos = new Position(path[distance].X, path[distance].Y);
            var direction = GetDirection(Client.Attributes.ServerPosition, pos);
            GameActions.Walk(Client, direction);
        }
        
        private Direction GetDirection(Position from, Position to)
        {
            var dx = to.X - from.X;
            var dy = to.Y - from.Y;
    
            if (dx == 1 && dy == 0) return Direction.East;
            if (dx == -1 && dy == 0) return Direction.West;
            if (dx == 0 && dy == -1) return Direction.North;
            if (dx == 0 && dy == 1) return Direction.South;
    
            // Fallback for diagonal or invalid moves
            if (dx > 0) return Direction.East;
            if (dx < 0) return Direction.West;
            if (dy > 0) return Direction.South;
            return Direction.North;
        }
    }
}
