using System;
namespace NPuzzle.src.Models
{
    public class NextBoardPosition
    {
        public Vector2 NextZeroPosition { get; set; }

        public Vector2 NextDigitPosition { get; set; }

        public int NextDigitValue { get; set; }
    }
}
