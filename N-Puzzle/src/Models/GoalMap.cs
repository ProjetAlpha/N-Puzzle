using System;
using System.Collections.Generic;

namespace NPuzzle.src.Models
{
    public class GoalMap
    {
        public int[][] Board { get; set; }

        public Dictionary<int, Vector2> IndexMap { get; set; }

        public Dictionary<int, string> DigitStrRepresentation { get; set; }
    }
}
