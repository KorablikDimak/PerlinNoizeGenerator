using System;
using System.Collections.Generic;
using UnityEngine;

namespace PerlinNoiseGenerator
{
    public class River
    {
        public List<Vector2Int> Positions { get; } = new List<Vector2Int>();
        public bool[,] IsRiverHere { get; private set; }

        public bool RiverGen(Vector2Int startPosition, float[,] noiseMap, float waterLevel)
        {
            if (startPosition.x == 0 || 
                startPosition.y == 0 || 
                startPosition.x == noiseMap.GetLength(0) - 1 || 
                startPosition.y == noiseMap.GetLength(1) - 1)
            {
                return false;
            }

            IsRiverHere = new bool[noiseMap.GetLength(0), noiseMap.GetLength(1)];
            Positions.Add(startPosition);
            IsRiverHere[startPosition.x, startPosition.y] = true;
            
            Vector2Int preferPosition = FindLowest(startPosition.x, startPosition.y, noiseMap);
            FindPathToWater(startPosition, preferPosition, noiseMap, waterLevel);

            return Positions.Count > 30;
        }

        public static void DigRiver(River river)
        {
            int countBeforeDigging = river.Positions.Count;
            for (int i = countBeforeDigging - 1; i >= countBeforeDigging / 1.6; i--)
            {
                Vector2Int direction = river.Positions[i] - river.Positions[i - 1];

                if (direction == Vector2Int.down || direction == Vector2Int.up)
                {
                    river.Positions.Add(new Vector2Int(river.Positions[i].x + 1, river.Positions[i].y));
                    river.IsRiverHere[river.Positions[i].x + 1, river.Positions[i].y] = true;
                    river.Positions.Add(new Vector2Int(river.Positions[i].x - 1, river.Positions[i].y));
                    river.IsRiverHere[river.Positions[i].x - 1, river.Positions[i].y] = true;
                }
                else if (direction == Vector2Int.left || direction == Vector2Int.right)
                {
                    river.Positions.Add(new Vector2Int(river.Positions[i].x, river.Positions[i].y + 1));
                    river.IsRiverHere[river.Positions[i].x, river.Positions[i].y + 1] = true;
                    river.Positions.Add(new Vector2Int(river.Positions[i].x, river.Positions[i].y - 1));
                    river.IsRiverHere[river.Positions[i].x, river.Positions[i].y - 1] = true;
                }
            }

            for (int i = (int) (countBeforeDigging / 1.6); i >= countBeforeDigging / 3; i--)
            {
                Vector2Int direction = river.Positions[i] - river.Positions[i - 1];
                
                if (direction == Vector2Int.down)
                {
                    river.Positions.Add(new Vector2Int(river.Positions[i].x + 1, river.Positions[i].y));
                    river.IsRiverHere[river.Positions[i].x + 1, river.Positions[i].y] = true;
                }
                else if (direction == Vector2Int.up)
                {
                    river.Positions.Add(new Vector2Int(river.Positions[i].x - 1, river.Positions[i].y));
                    river.IsRiverHere[river.Positions[i].x - 1, river.Positions[i].y] = true;
                }
                else if (direction == Vector2Int.left)
                {
                    river.Positions.Add(new Vector2Int(river.Positions[i].x, river.Positions[i].y + 1));
                    river.IsRiverHere[river.Positions[i].x, river.Positions[i].y + 1] = true;
                }
                else if (direction == Vector2Int.right)
                {
                    river.Positions.Add(new Vector2Int(river.Positions[i].x, river.Positions[i].y - 1));
                    river.IsRiverHere[river.Positions[i].x, river.Positions[i].y - 1] = true;
                }
            }
        }

        private void FindPathToWater(Vector2Int startPosition, Vector2Int preferPosition, float[,] noiseMap, float waterLevel)
        {
            const int maxIter = 200;
            int iter = 0;
            while (iter < maxIter)
            {
                Vector2Int direction = startPosition - preferPosition;

                float rightWeight = noiseMap[startPosition.x + 1, startPosition.y];
                float leftWeight = noiseMap[startPosition.x - 1, startPosition.y];
                float topWeight = noiseMap[startPosition.x, startPosition.y + 1];
                float downWeight = noiseMap[startPosition.x, startPosition.y - 1];

                if (direction == Vector2Int.down)
                {
                    downWeight = 0;
                }
                else if (direction == Vector2Int.up)
                {
                    topWeight = 0;
                }
                else if (direction == Vector2Int.left)
                {
                    leftWeight = 0;
                }
                else if (direction == Vector2Int.right)
                {
                    rightWeight = 0;
                }

                if (IsRiverHere[startPosition.x, startPosition.y - 1])
                {
                    downWeight = float.MaxValue;
                }
                if (IsRiverHere[startPosition.x, startPosition.y + 1])
                {
                    topWeight = float.MaxValue;
                }
                if (IsRiverHere[startPosition.x - 1, startPosition.y])
                {
                    leftWeight = float.MaxValue;
                }
                if (IsRiverHere[startPosition.x + 1, startPosition.y])
                {
                    rightWeight = float.MaxValue;
                }

                if (direction == Vector2Int.down && Mathf.Abs(topWeight - downWeight) < 0.1f)
                {
                    downWeight = float.MaxValue;
                }
                if (direction == Vector2Int.up && Mathf.Abs(downWeight - topWeight) < 0.1f)
                {
                    topWeight = float.MaxValue;
                }
                if (direction == Vector2Int.left && Mathf.Abs(rightWeight - leftWeight) < 0.1f)
                {
                    leftWeight = float.MaxValue;
                }
                if (direction == Vector2Int.right && Mathf.Abs(leftWeight - rightWeight) < 0.1f)
                {
                    rightWeight = float.MaxValue;
                }

                float min = Mathf.Min(topWeight, downWeight, leftWeight, rightWeight);

                if (Math.Abs(min - float.MaxValue) < float.Epsilon)
                {
                    return;
                }

                if (Math.Abs(min - leftWeight) < float.Epsilon)
                {
                    preferPosition = new Vector2Int(startPosition.x - 1, startPosition.y);
                }
                else if (Math.Abs(min - topWeight) < float.Epsilon)
                {
                    preferPosition = new Vector2Int(startPosition.x, startPosition.y + 1);
                }
                else if (Math.Abs(min - rightWeight) < float.Epsilon)
                {
                    preferPosition = new Vector2Int(startPosition.x - 1, startPosition.y);
                }
                else if (Math.Abs(min - downWeight) < float.Epsilon)
                {
                    preferPosition = new Vector2Int(startPosition.x, startPosition.y - 1);
                }

                if (noiseMap[preferPosition.x, preferPosition.y] < waterLevel)
                {
                    return;
                }
                
                Positions.Add(preferPosition);
                IsRiverHere[preferPosition.x, preferPosition.y] = true;
                startPosition = preferPosition;
                
                if (startPosition.x == 0 || 
                    startPosition.y == 0 || 
                    startPosition.x == noiseMap.GetLength(0) - 1 || 
                    startPosition.y == noiseMap.GetLength(1) - 1)
                {
                    return;
                }
                
                preferPosition = FindLowest(preferPosition.x, preferPosition.y, noiseMap);
                iter++;
            }
        }

        private static Vector2Int FindLowest(int x, int y, float[,] noiseMap)
        {
            float minNoise = float.MaxValue;
            Vector2Int minNoisePosition = new Vector2Int();
            Vector2Int[] neighbors = GetNeighbors(x, y);
            foreach (var neighbor in neighbors)
            {
                if (!(noiseMap[neighbor.x, neighbor.y] < minNoise)) continue;
                minNoisePosition = neighbor;
                minNoise = noiseMap[neighbor.x, neighbor.y];
            }

            return minNoisePosition;
        }

        private static Vector2Int[] GetNeighbors(int x, int y)
        {
            return new[]
            {
                new Vector2Int(x + 1, y),
                new Vector2Int(x - 1, y),
                new Vector2Int(x, y + 1),
                new Vector2Int(x, y - 1)
            };
        }
    }
}