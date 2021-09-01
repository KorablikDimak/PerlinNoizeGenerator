using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PerlyNoizeGenerator
{
    public class RiversGroup
    {
        public List<River> GroupOfRivers { get; } = new List<River>();

        public static List<RiversGroup> BuildRiversGroups(List<River> rivers)
        {
            var riversGroups = new List<RiversGroup>();

            while (rivers.Count > 0)
            {
                var riversGroup = new RiversGroup();
                var river = rivers[rivers.Count - 1];
                riversGroup.GroupOfRivers.Add(river);
                rivers.Remove(river);
                ContainRivers(riversGroup, rivers, river);
                riversGroups.Add(riversGroup);
            }

            return riversGroups;
        }

        private static void ContainRivers(RiversGroup riversGroup, List<River> rivers, River river)
        {
            foreach (var firstPosition in river.Positions)
            {
                for (int i = rivers.Count - 1; i >= 0; i--)
                {
                    if (!rivers[i].Positions.Any(secondPosition => IsNeighbors(firstPosition, secondPosition)))
                        continue;
                    var newRiver = rivers[i];
                    riversGroup.GroupOfRivers.Add(newRiver);
                    rivers.Remove(newRiver);
                    ContainRivers(riversGroup, rivers, newRiver);
                    return;
                }
            }
        }

        private static bool IsNeighbors(Vector2Int firstPosition, Vector2Int secondPosition)
        {
            return Vector2Int.Distance(firstPosition, secondPosition) <= 1.5f;
        }
    }
}