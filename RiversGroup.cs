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
            List<RiversGroup> riversGroups = new List<RiversGroup>();
            
            while (rivers.Count > 0)
            {
                RiversGroup riversGroup = new RiversGroup();
                riversGroup.GroupOfRivers.Add(rivers[0]);
                rivers.RemoveAt(0);

                if (rivers.Count > 0)
                {
                    for (int i = rivers.Count - 1; i >= 0; i--)
                    {
                        foreach (var riverInGroup in riversGroup.GroupOfRivers)
                        {
                            foreach (var position in rivers[i].Positions)
                            {
                                if (riverInGroup.Positions.Any(positionInGroup => IsNeighbors(position, positionInGroup)))
                                {
                                    riversGroup.GroupOfRivers.Add(rivers[i]);
                                    rivers.RemoveAt(i);
                                }

                                break;
                            }
                            break;
                        }
                    }
                }
                else
                {
                    riversGroups.Add(riversGroup);
                    return riversGroups;
                }
                riversGroups.Add(riversGroup);
            }

            return riversGroups;
        }

        private static bool IsNeighbors(Vector2Int position, Vector2Int positionInGroup)
        {
            return Vector2Int.Distance(position, positionInGroup) <= 4f;
        }
    }
}