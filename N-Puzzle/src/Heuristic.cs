using System;
using System.Collections.Generic;
using System.Text;
using NPuzzle.src.Models;

namespace NPuzzle.src
{
    public class Heuristic
    {
        private static int FastAbs(int i)
        {
            return (i + (i >> 31)) ^ (i >> 31);
        }

        private static bool IsNextPosition(NextBoardPosition position, int y, int x)
        {
            return (position.NextDigitPosition.X == x && position.NextDigitPosition.Y == y)
                || (position.NextZeroPosition.X == x && position.NextZeroPosition.Y == y);
        }

        private static int GetNextPosition(NextBoardPosition position, int y, int x)
        {
            return position.NextZeroPosition.X == x && position.NextZeroPosition.Y == y
                ? 0 : position.NextDigitValue;
        }

        private static int CountCornerConflict(Vector2 adjacentLine, Vector2 adjacentCol, GoalMap map, Node prevNode, Node newNode)
        {
            int conflicts = 0;
            int lineNumber = prevNode.Board[adjacentLine.Y][adjacentLine.X];

            if (IsNextPosition(newNode.nextBoardPosition, adjacentLine.Y, adjacentLine.X))
                lineNumber = GetNextPosition(newNode.nextBoardPosition, adjacentLine.Y, adjacentLine.X);

            map.IndexMap.TryGetValue(lineNumber, out Vector2 goalPos);

            if (goalPos.X == adjacentLine.X && goalPos.Y == adjacentLine.Y)
                return conflicts;
            ++conflicts;

            int colNumber = prevNode.Board[adjacentCol.Y][adjacentCol.X];

            if (IsNextPosition(newNode.nextBoardPosition, adjacentCol.Y, adjacentCol.X))
                colNumber = GetNextPosition(newNode.nextBoardPosition, adjacentCol.Y, adjacentCol.X);

            map.IndexMap.TryGetValue(colNumber, out Vector2 goalPos2);

            if (goalPos2.X == adjacentCol.X && goalPos2.Y == adjacentCol.Y)
                return conflicts;

            return ++conflicts;
        }

        private static int GetCornerConflicts(Vector2 pos, int value, GoalMap map, Node prevNode, Node newNode)
        {
            int cornerIndex = -1;

            // Up-left corner.
            if (pos.X == map.Corners[0].Position.X && pos.Y == map.Corners[0].Position.Y && map.Corners[0].Value == value)
            {
                cornerIndex = 0;
            }

            // Down-left corner.
            if (pos.X == map.Corners[1].Position.X && pos.Y == map.Corners[1].Position.Y && map.Corners[1].Value == value)
            {
                cornerIndex = 1;
            }

            // Up-right corner.
            if (pos.X == map.Corners[2].Position.X && pos.Y == map.Corners[2].Position.Y && map.Corners[2].Value == value)
            {
                cornerIndex = 2;
            }

            // Down-right corner.
            if (pos.X == map.Corners[3].Position.X && pos.Y == map.Corners[3].Position.Y && map.Corners[3].Value == value)
            {
                cornerIndex = 3;
            }

            if (cornerIndex == -1) return 0;


            Vector2 adjacentLine = map.Corners[cornerIndex].AdjacentTileX;
            Vector2 adjacentCol = map.Corners[cornerIndex].AdjacentTileY;

            return CountCornerConflict(adjacentLine, adjacentCol, map, prevNode, newNode);
        }

        public static int GetGoalMap(int x, int y, Dictionary<int, int> indexMap)
        {
            int binaryCoord = MapGenerator.SetBinaryCoords(x, y);
            indexMap.TryGetValue(binaryCoord, out int arrayValue);
            return arrayValue;
        }

        public static void HammingDistance(GoalMap map, Node prevNode, Node newNode)
        {
            int[][] dst = new int[prevNode.Board.Length][];
            int nMatch = 0;
            int countWrongTiles = 0;
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < prevNode.Board.Length; i++)
            {
                dst[i] = new int[prevNode.Board[i].Length];

                for (int j = 0; j < prevNode.Board[i].Length; j++)
                {
                    int number = prevNode.Board[i][j];

                    if (IsNextPosition(newNode.nextBoardPosition, i, j))
                        number = GetNextPosition(newNode.nextBoardPosition, i, j);

                    dst[i][j] = number;

                    if (number > prevNode.maxNumber)
                        prevNode.maxNumber = number;

                    if (number == map.Board[i][j])
                        ++nMatch;

                    map.DigitStrRepresentation.TryGetValue(number, out string numberToString);
                    sb.Append(numberToString);

                    if (number == 0) continue;

                    map.IndexMap.TryGetValue(number, out Vector2 vec2);
                    if (vec2.X != j && vec2.Y != i)
                        ++countWrongTiles;
                }
            }

            newNode.Id = sb.ToString();
            newNode.maxNumber = prevNode.maxNumber;
            newNode.Board = dst;
            newNode.IsGoal = newNode.Board.Length * newNode.Board.Length == nMatch;
            newNode.Heuristic = countWrongTiles;
        }

        public static void MHC(GoalMap map, Node prevNode, Node newNode)
        {
            int distance = 0;
            int nMatch = 0;
            int countWrongCorner = 0;
            int countWrongTiles = 0;

            int[][] dst = new int[prevNode.Board.Length][];
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < prevNode.Board.Length; i++)
            {
                dst[i] = new int[prevNode.Board[i].Length];

                for (int j = 0; j < prevNode.Board[i].Length; j++)
                {
                    int number = prevNode.Board[i][j];

                    if (IsNextPosition(newNode.nextBoardPosition, i, j))
                        number = GetNextPosition(newNode.nextBoardPosition, i, j);

                    dst[i][j] = number;

                    if (number > prevNode.maxNumber)
                        prevNode.maxNumber = number;

                    if (number == map.Board[i][j])
                        ++nMatch;

                    map.DigitStrRepresentation.TryGetValue(number, out string numberToString);
                    sb.Append(numberToString);

                    if (number == 0) continue;

                    map.IndexMap.TryGetValue(number, out Vector2 vec2);
                    distance += FastAbs(vec2.Y - i) + FastAbs(vec2.X - j);

                    countWrongCorner+= GetCornerConflicts(new Vector2 { X = j, Y = i }, number, map, prevNode, newNode);

                    if (vec2.X != j && vec2.Y != i)
                        ++countWrongTiles;
                }
            }

            newNode.Id = sb.ToString();
            newNode.maxNumber = prevNode.maxNumber;
            newNode.Board = dst;
            newNode.IsGoal = newNode.Board.Length * newNode.Board.Length == nMatch;
            newNode.Heuristic = countWrongCorner > 0 ? distance + 2 * countWrongCorner + countWrongTiles : distance;
        }

        public static void ManhattanDistance(GoalMap map, Node prevNode, Node newNode)
        {
            int distance = 0;
            int nMatch = 0;
            int[][] dst = new int[prevNode.Board.Length][];
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < prevNode.Board.Length; i++)
            {
                dst[i] = new int[prevNode.Board[i].Length];

                for (int j = 0; j < prevNode.Board[i].Length; j++)
                {
                    int number = prevNode.Board[i][j];

                    if (IsNextPosition(newNode.nextBoardPosition, i, j))
                        number = GetNextPosition(newNode.nextBoardPosition, i, j);

                    dst[i][j] = number;

                    if (number > prevNode.maxNumber)
                        prevNode.maxNumber = number;

                    if (number == map.Board[i][j])
                        ++nMatch;

                    map.DigitStrRepresentation.TryGetValue(number, out string numberToString);
                    sb.Append(numberToString);

                    if (number == 0) continue;

                    map.IndexMap.TryGetValue(number, out Vector2 vec2);
                    distance += FastAbs(vec2.Y - i) + FastAbs(vec2.X - j);
                }
            }

            newNode.Id = sb.ToString();
            newNode.maxNumber = prevNode.maxNumber;
            newNode.Board = dst;
            newNode.IsGoal = newNode.Board.Length * newNode.Board.Length == nMatch;
            newNode.Heuristic = distance;
        }

        public static void EuclideanDistance(GoalMap map, Node prevNode, Node newNode)
        {
            int distance = 0;
            int nMatch = 0;
            int[][] dst = new int[prevNode.Board.Length][];
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < prevNode.Board.Length; i++)
            {
                dst[i] = new int[prevNode.Board[i].Length];

                for (int j = 0; j < prevNode.Board[i].Length; j++)
                {
                    int number = prevNode.Board[i][j];

                    if (IsNextPosition(newNode.nextBoardPosition, i, j))
                        number = GetNextPosition(newNode.nextBoardPosition, i, j);

                    dst[i][j] = number;

                    if (number > prevNode.maxNumber)
                        prevNode.maxNumber = number;

                    if (number == map.Board[i][j])
                        ++nMatch;

                    map.DigitStrRepresentation.TryGetValue(number, out string numberToString);
                    sb.Append(numberToString);

                    if (number == 0) continue;

                    map.IndexMap.TryGetValue(number, out Vector2 vec2);
                    int h = FastAbs(vec2.Y - i);
                    int v = FastAbs(vec2.X - j);

                    distance += (int)Math.Sqrt(v * v + h * h);
                }
            }

            newNode.Id = sb.ToString();
            newNode.maxNumber = prevNode.maxNumber;
            newNode.Board = dst;
            newNode.IsGoal = newNode.Board.Length * newNode.Board.Length == nMatch;
            newNode.Heuristic = distance;
        }

        public static void ManathanLinearConflict(GoalMap map, Node prevNode, Node newNode)
        {
            int conflicts = 0;
            int distance = 0;
            int nMatch = 0;
            int[][] dst = new int[prevNode.Board.Length][];
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < prevNode.Board.Length; i++)
            {
                dst[i] = new int[prevNode.Board[i].Length];

                for (int j = 0; j < prevNode.Board[i].Length; j++)
                {
                    int number = prevNode.Board[i][j];

                    if (IsNextPosition(newNode.nextBoardPosition, i, j))
                        number = GetNextPosition(newNode.nextBoardPosition, i, j);

                    dst[i][j] = number;

                    if (number > prevNode.maxNumber)
                        prevNode.maxNumber = number;

                    if (number == map.Board[i][j])
                        ++nMatch;

                    map.DigitStrRepresentation.TryGetValue(number, out string numberToString);
                    sb.Append(numberToString);

                    if (number == 0) continue;

                    map.IndexMap.TryGetValue(number, out Vector2 vec2);
                    distance += FastAbs(vec2.Y - i) + FastAbs(vec2.X - j);

                    if (j == vec2.Y && i == vec2.X) continue;

                    if (j == vec2.Y)
                    {
                        // check down for conflicts
                        for (int l = 0; l < prevNode.Board.Length; l++)
                        {
                            if (prevNode.Board[l][j] == 0) continue;

                            int n2 = prevNode.Board[l][j];

                            if (IsNextPosition(newNode.nextBoardPosition, l, j))
                                n2 = GetNextPosition(newNode.nextBoardPosition, l, j);

                            map.IndexMap.TryGetValue(n2, out Vector2 vect);
                            if (vec2.X > vect.X && vect.Y == j)
                            {
                                ++conflicts;
                            }
                        }
                    }

                    if (i == vec2.X)
                    {
                        // check right for conflicts
                        for (int k = 0; k < prevNode.Board[i].Length; k++)
                        {
                            if (prevNode.Board[i][k] == 0) continue;

                            int n2 = prevNode.Board[i][k];

                            if (IsNextPosition(newNode.nextBoardPosition, i, k))
                                n2 = GetNextPosition(newNode.nextBoardPosition, i, k);

                            map.IndexMap.TryGetValue(n2, out Vector2 vect);
                            if (vec2.Y > vect.Y && vect.X == i)
                            {
                                ++conflicts;
                            }
                        }
                    }
                }
            }

            newNode.Id = sb.ToString();
            newNode.maxNumber = prevNode.maxNumber;
            newNode.Board = dst;
            newNode.IsGoal = newNode.Board.Length * newNode.Board.Length == nMatch;
            newNode.Heuristic = conflicts > 0 ? distance + 2 * conflicts : distance;
        }
    }
}
