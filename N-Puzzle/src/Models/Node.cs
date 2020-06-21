using System;
using Priority_Queue;

namespace NPuzzle.src.Models
{
    public class Node
    {
        public Node Parent { get; set; }

        public int Heuristic;

        public int GoalHeuristic;

        public int Cost;

        public Vector2 ZeroPosition;

        public Vector2 Size;

        public string Id { get; set; }

        public bool IsGoal { get; set; }

        public int[][] Board;

        public int maxNumber;

        public NextBoardPosition nextBoardPosition;
    }
}
