using System;
using System.Collections.Generic;
using NPuzzle.src.Models;

namespace NPuzzle.src
{
    public class MapGenerator
    {
        private static bool IsVisited(int n)
        {
            return n > 0;
        }

        public static Vector2 ConvertBinaryCoords(int XY)
        {
            return new Vector2{ X = XY & 0xffff, Y = XY >> 16 };
        }

        public static int SetBinaryCoords(int x, int y)
        {
            return x << 16 | y;
        }

        /* [c0] 2 [c2]
         *  8   0   4
         * [c1] 6 [c3]
         */        
        private static void SetCorner(int col, int row, int size, CornerTile[] corners, int n)
        {
            // X = row, Y = col
            if (col == 0 && row == 0)
            {
                corners[0] = new CornerTile
                {
                    Value = n,
                    Position = new Vector2{ X = 0, Y = 0 },
                    AdjacentTileX = new Vector2{ X = 0 + 1, Y = 0},
                    AdjacentTileY = new Vector2{ X = 0, Y = 0 + 1 }
                };
            }

            if (col == size - 1 && row == 0)
            {
                corners[1] = new CornerTile
                {
                    Value = n,
                    Position = new Vector2{ X = 0, Y = size - 1 },
                    AdjacentTileX = new Vector2{ X = 0 + 1, Y = size - 1 },
                    AdjacentTileY = new Vector2{ X = 0, Y = size - 1 - 1}
                };
            }

            if (col == 0 && row == size - 1)
            {
                corners[2] = new CornerTile
                {
                    Value = n,
                    Position = new Vector2{ X = size - 1, Y = 0 },
                    AdjacentTileX = new Vector2{ X = size - 1 - 1, Y = 0},
                    AdjacentTileY = new Vector2{ X = size - 1, Y = 0 + 1}
                };
            }

            if (col == size - 1 && row == size - 1)
            {
                corners[3] = new CornerTile
                {
                    Value = n,
                    Position = new Vector2{ X = size - 1, Y = size - 1 },
                    AdjacentTileX = new Vector2 { X = size - 1 - 1, Y = size - 1},
                    AdjacentTileY = new Vector2 { X = size - 1, Y = size - 1 - 1}
                };
            }
        }

        public static int[][] GetGoalMap(int size,
            out Dictionary<int, Vector2> IndexMap,
            out Dictionary<int, string> MapStrRepresentation,
            out CornerTile[] corners)
        {
            IndexMap = new Dictionary<int, Vector2>();
            MapStrRepresentation = new Dictionary<int, string>();
            corners = new CornerTile[4];

            int[][] tab = new int[size][];
            for (int i = 0; i < size; i++)
            {
                tab[i] = new int[size];
            }

            int turnCounter = 0;
            int row = 0, col = 0;
            int n = 1;
            int totalSize = size * size;

            for (int i = 0; i < totalSize; i++)
            {
                tab[col][row] = n == totalSize ? 0 : n;

                IndexMap.Add(tab[col][row], new Vector2 { X = row, Y = col });
                MapStrRepresentation.Add(tab[col][row], tab[col][row].ToString());

                SetCorner(col, row, size, corners, n);

                n++;
                if (turnCounter == 4)
                    turnCounter = 0;
                if (turnCounter == 0 && row < size)
                {
                    if (row == size - 1 || (row + 1 < size && IsVisited(tab[col][row + 1])))
                    {
                        turnCounter++;
                        col++;
                    }
                    else
                    {
                        row++;
                    }
                }
                else if (turnCounter == 3 && col >= 0)
                {
                    if (col == 0 || (col - 1 >= 0 && IsVisited(tab[col - 1][row])))
                    {
                        turnCounter++;
                        row++;
                    }
                    else
                    {
                        col--;
                    }
                }
                else if (turnCounter == 2 && row >= 0)
                {
                    if (row == 0 || (row - 1 >= 0 && IsVisited(tab[col][row - 1])))
                    {
                        turnCounter++;
                        col--;
                    }
                    else
                    {
                        row--;
                    }
                }
                else if (turnCounter == 1 && col < size)
                {
                    if (col == size - 1 || (col + 1 < size && IsVisited(tab[col + 1][row])))
                    {
                        turnCounter++;
                        row--;
                    }
                    else
                    {
                        col++;
                    }
                }
            }
            return tab;
        }
    }
}
