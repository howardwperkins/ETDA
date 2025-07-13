using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace BotCore.Types
{
    public class Breadcrumbs
    {

        public ConcurrentDictionary<int, Position> Trail { get; set; }
        
        public Breadcrumbs()
        {
            Trail = new ConcurrentDictionary<int, Position>();
        }

        public void DropBreadcrumb(int mapId, Position pos)
        {
            Trail.TryAdd(mapId, pos);
        }
        
        public Position GetBreadcrumb(int mapId)
        {
            if (Trail.TryGetValue(mapId, out var pos))
            {
                return pos;
            }
            return null;
        }

        public void ClearAllBreadcrumbs()
        {
            if (Trail.IsEmpty)
            {
                return;
            }
            
            Trail.Clear();
            Console.WriteLine(@"Cleared all Breadcrumbs.");
        }
        
        public void ClearBreadcrumb(int mapId)
        {
            if (Trail.IsEmpty)
            {
                return;
            }

            if (!Trail.TryGetValue(mapId, out var pos))
            {
                return;
            }
            
            Trail.TryRemove(mapId, out _);
            Console.WriteLine(@"Cleared Breadcrumb {0}  @ {1}", mapId, pos);
        }
    }
}