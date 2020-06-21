using System;
using System.Collections.Generic;
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

        public static int GetGoalMap(int x, int y, Dictionary<int, int> indexMap)
        {
            int binaryCoord = MapGenerator.SetBinaryCoords(x, y);
            indexMap.TryGetValue(binaryCoord, out int arrayValue);
            return arrayValue;
        }

        public static void ManhattanDistance(GoalMap map, Node prevNode, Node newNode)
        {
            int distance = 0;
            int nMatch = 0;
            int[][] dst = new int[prevNode.Board.Length][];

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
                    newNode.Id += numberToString;

                    if (number == 0) continue;

                    map.IndexMap.TryGetValue(number, out Vector2 vec2);
                    // distance += Math.Abs(vec2.Y - i) + Math.Abs(vec2.X - j);
                    distance += FastAbs(vec2.Y - i) + FastAbs(vec2.X - j);
                }
            }

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
                    newNode.Id += numberToString;

                    if (number == 0) continue;

                    map.IndexMap.TryGetValue(number, out Vector2 vec2);
                    int h = FastAbs(vec2.Y - i);
                    int v = FastAbs(vec2.X - j);

                    distance += (int)Math.Sqrt(v * v + h * h);
                }
            }

            newNode.maxNumber = prevNode.maxNumber;
            newNode.Board = dst;
            newNode.IsGoal = newNode.Board.Length * newNode.Board.Length == nMatch;
            newNode.Heuristic = distance;
        }

        public static void ManathanLinearConflict(GoalMap map, Node prevNode, Node newNode)
        {
            int conflict = 0;
            int distance = 0;
            int nMatch = 0;
            int[][] dst = new int[prevNode.Board.Length][];

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
                    newNode.Id += numberToString;

                    if (number == 0) continue;

                    map.IndexMap.TryGetValue(number, out Vector2 vec2);
                    distance += FastAbs(vec2.Y - i) + FastAbs(vec2.X - j);

                    if (vec2.X == j && vec2.Y == i) continue;

                    if (i == vec2.Y)
                    {
                        for (int k = j + 1; k < prevNode.Board[i].Length; k++)
                        {
                            if (number > prevNode.Board[i][k])
                                conflict++;
                        }
                    }
                    else if (j == vec2.X)
                    {
                        for (int l = i + 1; l < prevNode.Board.Length; l++)
                        {
                            if (number > prevNode.Board[l][j])
                                conflict++;
                        }
                    }
                }
            }

            newNode.maxNumber = prevNode.maxNumber;
            newNode.Board = dst;
            newNode.IsGoal = newNode.Board.Length * newNode.Board.Length == nMatch;
            newNode.Heuristic = distance + 2 * conflict;
        }
    }
}
