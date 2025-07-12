using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace BotCore.Types
{
    public class Breadcrumbs
    {
        private const int MAX_BREADCRUMBS_PER_MAP = 50;
        private const int REDUCTION_FACTOR = 5; // Keep every 5th breadcrumb when reducing

        public Breadcrumbs()
        {
            Trail = new ConcurrentDictionary<int, ConcurrentQueue<Position>>();
        }

        public ConcurrentDictionary<int, ConcurrentQueue<Position>> Trail { get; set; }

        public void DropBreadcrumb(int mapId, Position pos)
        {
            var queue = Trail.GetOrAdd(mapId, _ => new ConcurrentQueue<Position>());
            queue.Enqueue(pos);

            // Reduce breadcrumbs if we have too many on this map
            if (queue.Count > MAX_BREADCRUMBS_PER_MAP)
            {
                ReduceBreadcrumbs(mapId);
            }
        }

        public Position PeekNextBreadcrumb(int mapId)
        {
            if (Trail.TryGetValue(mapId, out var queue) && queue.TryPeek(out var position))
            {
                return position;
            }
            return null;
        }
        
        public Position GetNextBreadcrumb(int mapId)
        {
            if (Trail.TryGetValue(mapId, out var queue) && queue.TryDequeue(out var position))
            {
                return position;
            }
            return null;
        }

        public bool HasBreadcrumbs(int mapId)
        {
            return Trail.TryGetValue(mapId, out var queue) && !queue.IsEmpty;
        }

        public void ClearBreadcrumbs(int mapId)
        {
            if (Trail.TryGetValue(mapId, out var queue))
            {
                while (queue.TryDequeue(out _)) { }
            }
        }

        private void ReduceBreadcrumbs(int mapId)
        {
            if (!Trail.TryGetValue(mapId, out var queue)) return;

            var breadcrumbArray = queue.ToArray();
            var newQueue = new ConcurrentQueue<Position>();

            // Keep every nth breadcrumb
            for (int i = 0; i < breadcrumbArray.Length; i += REDUCTION_FACTOR)
            {
                newQueue.Enqueue(breadcrumbArray[i]);
            }

            Trail[mapId] = newQueue;
        }
    }
}