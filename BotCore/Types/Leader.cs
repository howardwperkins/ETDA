using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BotCore.States;

namespace BotCore.Types
{
    public class Leader
    {
        [Browsable(false)]
        public GameClient Client { get; set; }
        public string Name { get; set; }
        public int Serial { get; set; }
        
        private List<GameClient> _followers = new List<GameClient>();

        public List<GameClient> Followers
        {
            get
            {
                return _followers;
            }
            private set
            {
                
            }
        }
            
        public bool IsActive => Client != null && Client.IsInGame() && Client.MapId != 0 && Client.Attributes != null && Client.Attributes.ServerPosition != null;

        public void AddFollower(GameClient follower)
        {
            if (follower == null || Followers.Contains(follower))
                return;
            
            Followers.Add(follower);
        }

        public void RemoveFollower(GameClient follower)
        {
            if (follower == null || !Followers.Contains(follower))
                return;
            
            Followers.Remove(follower);
        }
            
        public void DropBreadcrumbsToFollowers(int mapId = 0, Position position = null)
        {
            if (!IsActive)
                return;
            
            if (position == null)
                position = Client.Attributes.ServerPosition;
            
            if (mapId == 0)
                mapId = Client.MapId;
            
            foreach (var follower in Followers)
            {
                var followState = follower.StateMachine.States.OfType<FollowTarget>().FirstOrDefault();
                if (followState != null && followState.Enabled) 
                    followState.Breadcrumbs.DropBreadcrumb(mapId, position);
                    
            }
            
            Console.WriteLine($@"{Name} dropped breadcrumbs {mapId} @ {position}");
        }
        public override string ToString()
        {
            return Name;
        }
    }

}