using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private Position TargetPosition = null;
        private bool ExactPosition = false;
        
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
                    return false;
                }
                
                if (Client.MapId == Leader.Client.MapId)
                {
                    TargetPosition = Leader.Client.Attributes.ServerPosition;
                    ExactPosition = false;
                    return true;
                }
                
                var peekTargetPosition = Breadcrumbs.GetNextBreadcrumb(Client.MapId);
                if (peekTargetPosition == null)
                    return false;
                
                TargetPosition = peekTargetPosition;
                ExactPosition = true;
                return true;
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
                        Console.WriteLine(Client.Attributes.PlayerName + " could not find a path to the target position.");
                        InTransition = false;
                        return;
                    }
                    
                    WalkPath(path);
                }
                
            }
            Client.TransitionTo(this, elapsed);
        }

        private bool DoneWalking()
        {
            var doneWalking = Client.Attributes.ServerPosition == TargetPosition;
            if (!ExactPosition)
            {
                doneWalking = Client.Attributes.ServerPosition.IsNearby(TargetPosition);
            }

            return doneWalking;
        }
        
        private void WalkPath(List<PathSolver.PathFinderNode> path)
        {
            // Start from index 1 (skip current position)
            for (int i = 1; i < path.Count; i++)
            {
                var currentPos = Client.Attributes.ServerPosition;
                var targetPos = new Position(path[i].X, path[i].Y);
        
                // Calculate direction to next step
                var direction = GetDirection(currentPos, targetPos);
                
                // Turn to face the correct direction with retry logic
                if (Client.Attributes.Direction != direction)
                {
                    if (!TryTurnToDirection(direction))
                    {
                        Console.WriteLine($"{Client.Attributes.PlayerName} failed to turn {direction}, skipping path");
                        return;
                    }
                }
        
                // Walk in the direction with retry logic
                if (!TryWalkToPosition(targetPos, direction))
                {
                    Console.WriteLine($"{Client.Attributes.PlayerName} failed to walk to ({targetPos.X},{targetPos.Y}), skipping path");
                    return;
                }
            }
        }
        
        private bool TryTurnToDirection(Direction direction, int maxRetries = 3, int timeoutMs = 2000)
        {
            for (int retry = 0; retry < maxRetries; retry++)
            {
                GameActions.Walk(Client, direction);
                Console.WriteLine($"{Client.Attributes.PlayerName} turning {direction} (attempt {retry + 1})");
                
                var startTime = DateTime.Now;
                while (Client.Attributes.Direction != direction)
                {
                    if ((DateTime.Now - startTime).TotalMilliseconds > timeoutMs)
                    {
                        Console.WriteLine($"{Client.Attributes.PlayerName} timeout turning {direction}");
                        break;
                    }
                    Thread.Sleep(50);
                }
                
                if (Client.Attributes.Direction == direction)
                {
                    return true;
                }
                
                // Wait before retry
                Thread.Sleep(100);
            }
            
            return false;
        }
        
        private bool TryWalkToPosition(Position targetPos, Direction direction, int maxRetries = 3, int timeoutMs = 3000)
        {
            for (int retry = 0; retry < maxRetries; retry++)
            {
                GameActions.Walk(Client, direction);
                Console.WriteLine($"{Client.Attributes.PlayerName} walking {direction} (attempt {retry + 1})");
                
                var startTime = DateTime.Now;
                while (Client.Attributes.ServerPosition.X != targetPos.X ||
                       Client.Attributes.ServerPosition.Y != targetPos.Y)
                {
                    if ((DateTime.Now - startTime).TotalMilliseconds > timeoutMs)
                    {
                        Console.WriteLine($"{Client.Attributes.PlayerName} timeout walking to ({targetPos.X},{targetPos.Y})");
                        break;
                    }
                    Thread.Sleep(50);
                }
                
                // Check if we reached the target
                if (Client.Attributes.ServerPosition.X == targetPos.X &&
                    Client.Attributes.ServerPosition.Y == targetPos.Y)
                {
                    return true;
                }
                
                // Wait before retry
                Thread.Sleep(200);
            }
            
            return false;
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
