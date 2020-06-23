using System;
namespace NPuzzle.src.Models
{
    [Flags]
    public enum ArgumentType
    {
        None = 0,
        Euclidian = 1 << 1,
        Manathan = 1 << 2,
        LinearConflict = 1 << 3,
        Hamming = 1 << 4,
        InputFile = 1 << 5
    }
}
