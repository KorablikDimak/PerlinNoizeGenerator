using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace PerlinNoiseGenerator
{
    public class RiversGroup
    {
        public List<River> GroupOfRivers { get; } = new List<River>();
        private static int[,] Count { get; set; }
        private static List<Vector2Int> CrossPoints { get; set; }

        public static List<RiversGroup> BuildRiversGroups(List<River> rivers, int mapSizeX, int mapSizeY)
        {
            var riversGroups = new List<RiversGroup>();

            Count = new int[mapSizeX, mapSizeY];
            Parallel.ForEach(rivers, river =>
            {
                for (int x = 0; x < mapSizeX; x++)
                {
                    for (int y = 0; y < mapSizeY; y++)
                    {
                        if (!river.IsRiverHere[x, y]) continue;
                        lock (Count)
                        {
                            Count[x, y]++;
                        }
                    }
                }
            });

            CrossPoints = new List<Vector2Int>();
            for (int x = 0; x < mapSizeX; x++)
            {
                for (int y = 0; y < mapSizeY; y++)
                {
                    if (Count[x, y] > 1)
                    {
                        CrossPoints.Add(new Vector2Int(x, y));
                    }
                }
            }

            while (rivers.Count > 0)
            {
                var riversGroup = new RiversGroup();
                var river = rivers[rivers.Count - 1];
                riversGroup.GroupOfRivers.Add(river);
                rivers.Remove(river);
                ContainRivers(riversGroup, rivers, river);
                riversGroups.Add(riversGroup);
            }

            CrossPoints.Clear();
            return riversGroups;
        }

        private static void ContainRivers(RiversGroup riversGroup, List<River> rivers, River river)
        {
            foreach (var crossPoint in CrossPoints)
            {
                for (int i = rivers.Count - 1; i >= 0; i--)
                {
                    if (!river.IsRiverHere[crossPoint.x, crossPoint.y]) continue;
                    if (!rivers[i].IsRiverHere[crossPoint.x, crossPoint.y]) continue;
                    Count[crossPoint.x, crossPoint.y]--;
                    if (Count[crossPoint.x, crossPoint.y] < 2)
                    {
                        CrossPoints.Remove(crossPoint);
                    }
                    var newRiver = rivers[i];
                    riversGroup.GroupOfRivers.Add(newRiver);
                    rivers.RemoveAt(i);
                    ContainRivers(riversGroup, rivers, newRiver);
                    return;
                }
            }
        }
    }
}