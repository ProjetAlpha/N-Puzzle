using System;
using System.Collections.Generic;
using System.IO;
using NPuzzle.src.Models;

namespace NPuzzle.src
{
    public class Parser
    {
        static string HelpMessage = "Usage" + Environment.NewLine + "./N-Puzzle [-h][TYPE][-i][PATH]." + Environment.NewLine +
        "-h : heuristic, -i : input file" + Environment.NewLine +
            "TYPE : ML (manathan and linear conflict), M (manathan), E (euclidian)" + Environment.NewLine +
            "PATH : input file" + Environment.NewLine;

        static private int LineCounter = 0;
        static private int PuzzleSize = 0;
        static private Vector2 ZeroStartPosition;
        static private string PuzzleId;
        static public int[][] Puzzle = null;

        private static bool IsInteger(char c)
        {
            return c >= '0' && c <= '9';
        }

        private static void GetNextDigit(string input, int ix, out int nextDigitPosition)
        {
            while (input[ix] == ' ' || input[ix] == '\t')
            {
                ix++;
            }
            nextDigitPosition = ix;
        }

        private static int ExtractNumber(string input, int ix, out int nextCharIndex)
        {
            string result = "";
            int i = ix;
            for (; i < input.Length; i++)
            {
                if (input[i] == '#' || !IsInteger(input[i])) break;

                result += input[i];
            }
            PuzzleId += result;
            nextCharIndex = i;

            int convertNumber = -1;
            try { convertNumber = int.Parse(result); } catch { return -1; }
            return convertNumber;
        }

        public static bool ProcessLine(string line)
        {
            if (string.IsNullOrEmpty(line))
                return true;
                
            int digitCounter = 0;
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (c.Equals('#'))
                {
                    break;
                }

                Parser.GetNextDigit(line, i, out int nextDigitPosition);

                if (nextDigitPosition == line.Length && digitCounter == 0)
                    return false;

                c = line[nextDigitPosition];
                if (!IsInteger(c))
                {
                    return false;
                }

                i = nextDigitPosition > 0 ? nextDigitPosition : i;

                if (LineCounter == 0 && PuzzleSize == 0)
                {
                    PuzzleSize = ExtractNumber(line, nextDigitPosition, out int unused);

                    if (PuzzleSize < 3)
                    {
                        return false;
                    }
                    else
                    { 
                        break;
                    }
                }

                // Initialize puzzle array.
                if (Puzzle == null)
                {
                    Puzzle = new int[PuzzleSize][];
                    for (int j = 0; j < PuzzleSize; j++)
                    {
                        Puzzle[j] = new int[PuzzleSize];
                    }
                }

                if (digitCounter > PuzzleSize - 1) return false;

                // Store integer.
                Puzzle[LineCounter][digitCounter] = ExtractNumber(line, nextDigitPosition, out int nexCharIndex);
                if (Puzzle[LineCounter][digitCounter] == 0)
                    ZeroStartPosition = new Vector2 { X = digitCounter, Y = LineCounter };

                i = nexCharIndex > 0 ? nexCharIndex : i;

                digitCounter++;
            }

            if (digitCounter > 0)
            {
                LineCounter++;
            }

            return true;
        }

        public static void GetOptions(string[] argv)
        {
            ArgumentType type = ArgumentType.None;

            if (argv == null || argv.Length == 0)
            {
                Console.WriteLine("No argument error. {0}", HelpMessage);
                System.Environment.Exit(-1);
            }

            if (argv.Length > 1)
            {
                if (argv[0] == "-h" && argv[1] == "M")
                {
                    type |= ArgumentType.Manathan;
                }

                if (argv[0] == "-h" && argv[1] == "E")
                {
                    type |= ArgumentType.Euclidian;
                }

                if (argv[0] == "-h" && argv[1] == "ML")
                {
                    type |= ArgumentType.LinearConflict;
                }
            }
            else
            {
                Console.WriteLine("Missing heuristic argument. {0}", HelpMessage);
                System.Environment.Exit(-1);
            }

            if (argv.Length > 2)
            {
                if (argv[2] == "-i" && argv[3] != "")
                {
                    if (!File.Exists(argv[3]))
                    {
                        Console.WriteLine("No input file error.");
                        System.Environment.Exit(-1);
                    }

                    type |= ArgumentType.InputFile;

                    try
                    {
                        // Parse file.

                        foreach (string line in File.ReadLines(argv[3]))
                        {
                            if (!ProcessLine(line))
                            {
                                Console.WriteLine("Parsing error.");
                                System.Environment.Exit(-1);
                            }
                        }

                        // Check puzzle validity.

                        // bug 3*3 puzzle.
                        /*if (!CheckValidity.IsSolvable(Puzzle, PuzzleSize))
                        {
                            Console.WriteLine("Unsolvable N-puzzle.");
                            System.Environment.Exit(-1);
                        }*/

                        // Generate puzzle solution.

                        int[][] goalMap = MapGenerator.GetGoalMap(PuzzleSize,
                            out Dictionary<int, Vector2> IndexMap,
                            out Dictionary<int, string> MapStrRepresentation
                            );
                        GoalMap map = new GoalMap() { Board = goalMap, IndexMap = IndexMap, DigitStrRepresentation = MapStrRepresentation };

                        // Initialize start node puzzle.

                        Node startNode = new Node();
                        startNode.ZeroPosition = ZeroStartPosition;
                        startNode.Board = Puzzle;
                        startNode.Id = PuzzleId;

                        // Solve N-Puzzle with A* pathfinding algorithm.

                        Solver puzzleSolver = new Solver(PuzzleSize, type, map);

                        try
                        {
                            puzzleSolver.AStar(startNode);
                        }
                        catch (Exception ex)
                        {
                            Console.Write(ex);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        System.Environment.Exit(-1);
                    }
                }
            }
            else
            {
                Console.WriteLine("Missing file argument. {0}", HelpMessage);
                System.Environment.Exit(-1);
            }
        }
    }
}
