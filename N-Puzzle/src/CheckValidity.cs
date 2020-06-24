using System;
using System.Collections;
using NPuzzle.src.Models;

namespace NPuzzle.src
{
    public static class CheckValidity
    {
        // Inversion : tell how far (or close) the array is from being sorted.
        public static bool IsSolvable(int[][] puzzle, int size, GoalMap map)
        {
            int[] arrayMap = new int[size * size];
            int[] goalMap = new int[size * size];

            int zeroInitialY = 0;
            int zeroGoalY = 0;
            int initialInvertCount = 0;
            int goalInvertCount = 0;

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    map.IndexMap.TryGetValue(puzzle[i][j], out Vector2 vec);
                    goalMap[size * vec.Y + vec.X] = puzzle[i][j];

                    arrayMap[size * i + j] = puzzle[i][j];

                    if (puzzle[i][j] == 0)
                    {
                        zeroInitialY = size - i;
                        zeroGoalY = size - vec.Y;
                    }
                }
            }

            for (int i = 0; i < size * size - 1; i++)
            {
                for (int j = i + 1; j < size * size; j++)
                {
                    if (arrayMap[i] != 0 && arrayMap[j] != 0 && arrayMap[i] > arrayMap[j])
                    {
                        initialInvertCount++;
                    }

                    if (goalMap[i] != 0 && goalMap[j] != 0 && goalMap[i] > goalMap[j])
                    {
                        goalInvertCount++;
                    }
                }
            }

            // even grid
            if (size % 2 == 0)
            { 
                return (initialInvertCount + zeroInitialY) % 2 == (goalInvertCount + zeroGoalY) % 2;
            }

            // odd grid
            return initialInvertCount % 2 == goalInvertCount % 2;
        }
    }
}
