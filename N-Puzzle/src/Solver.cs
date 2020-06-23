using System;
using NPuzzle.src.Models;
using Priority_Queue;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NPuzzle.src
{
    public class Solver
    {
        private int _puzzleSize;
        private ArgumentType _type;
        private GoalMap _goalMap;
        private bool _isGoalReach;
        private int _goalNodeIndex;
        private Dictionary<string, Node> _closedListHash = new Dictionary<string, Node>();
        private Dictionary<string, Node> _openListHash = new Dictionary<string, Node>();

        public Solver(int puzzleSize, ArgumentType type, GoalMap map)
        {
            this._puzzleSize = puzzleSize;
            this._type = type;
            this._goalMap = map;
        }

        public void GetHeuristic(Node prevNode, Node newNode)
        {
            if ((_type & ArgumentType.Euclidian) != 0)
                Heuristic.EuclideanDistance(_goalMap, prevNode, newNode);
            if ((_type & ArgumentType.Manathan) != 0)
                Heuristic.ManhattanDistance(_goalMap, prevNode, newNode);
            if ((_type & ArgumentType.LinearConflict) != 0)
                Heuristic.ManathanLinearConflict(_goalMap, prevNode, newNode);
            if ((_type & ArgumentType.Hamming) != 0)
                Heuristic.HammingDistance(_goalMap, prevNode, newNode);
        }

        private int[][] Swap(int[][] Board, Vector2 v1, Vector2 v2)
        {
            int tmp = Board[v2.Y][v2.X];
            Board[v2.Y][v2.X] = 0;
            Board[v1.Y][v1.X] = tmp;
            return Board;
        }

        private NextBoardPosition GetNextBoardPosition(int[][] Board, Vector2 v1, Vector2 v2)
        {
            return new NextBoardPosition
            {
                NextZeroPosition = new Vector2 { X = v2.X, Y = v2.Y },
                NextDigitPosition = new Vector2 { X = v1.X, Y = v1.Y },
                NextDigitValue = Board[v2.Y][v2.X]
            };
        }

        private int[][] DeepCopy(int[][] src)
        {
            int[][] dst = new int[src.Length][];

            for (int i = 0; i < src.Length; i++)
            {
                dst[i] = new int[src[i].Length];
                for (int j = 0; j < src[i].Length; j++)
                {
                    dst[i][j] = src[i][j];
                }
            }

            return dst;
        }

        private Node[] GetNodes(Node node)
        {
            Node[] nodes = new Node[4];
            Vector2 newZeroPosition = null;

            int i = 0;
            // Opti'++ : - faire une seul boucle qui calcul les 4 heuristiques (reduit de X4 les boucles).
            //           - tableau de 16 * 4 bits / case (reduit de X4 les boucles).
            // 
            if (node.ZeroPosition != null)
            {
                Vector2 pos = node.ZeroPosition;
                if (pos.X + 1 < _puzzleSize)
                {
                    newZeroPosition = new Vector2() { Y = pos.Y, X = pos.X + 1 };
                    nodes[i] = new Node()
                    {
                        nextBoardPosition = GetNextBoardPosition(node.Board, new Vector2() { Y = pos.Y, X = pos.X }, newZeroPosition),
                        Cost = node.Cost + 1,
                        Parent = node,
                        ZeroPosition = newZeroPosition
                    };
                    GetHeuristic(node, nodes[i]);
                    nodes[i].GoalHeuristic = nodes[i].Cost + nodes[i].Heuristic;
                    if (nodes[i].IsGoal)
                        _isGoalReach = true;
                    i++;
                }

                if (_isGoalReach)
                {
                    _goalNodeIndex = i - 1;
                    return nodes;
                }

                if (pos.Y + 1 < _puzzleSize)
                {
                    newZeroPosition = new Vector2() { Y = pos.Y + 1, X = pos.X };
                    nodes[i] = new Node()
                    {
                        nextBoardPosition = GetNextBoardPosition(node.Board, new Vector2() { Y = pos.Y, X = pos.X }, newZeroPosition),
                        Cost = node.Cost + 1,
                        Parent = node,
                        ZeroPosition = newZeroPosition
                    };
                    GetHeuristic(node, nodes[i]);
                    nodes[i].GoalHeuristic = nodes[i].Cost + nodes[i].Heuristic;
                    if (nodes[i].IsGoal)
                        _isGoalReach = true;
                    i++;
                }

                if (_isGoalReach)
                {
                    _goalNodeIndex = i - 1;
                    return nodes;
                }

                if (pos.Y - 1 >= 0)
                {
                    newZeroPosition = new Vector2() { Y = pos.Y - 1, X = pos.X };
                    nodes[i] = new Node()
                    {
                        nextBoardPosition = GetNextBoardPosition(node.Board, new Vector2() { Y = pos.Y, X = pos.X }, newZeroPosition),
                        Cost = node.Cost + 1,
                        Parent = node,
                        ZeroPosition = newZeroPosition
                    };
                    GetHeuristic(node, nodes[i]);
                    nodes[i].GoalHeuristic = nodes[i].Cost + nodes[i].Heuristic;
                    if (nodes[i].IsGoal)
                        _isGoalReach = true;
                    i++;
                }

                if (_isGoalReach)
                {
                    _goalNodeIndex = i - 1;
                    return nodes;
                }

                if (pos.X - 1 >= 0)
                {
                    newZeroPosition = new Vector2() { Y = pos.Y, X = pos.X - 1 };
                    nodes[i] = new Node()
                    {
                        nextBoardPosition = GetNextBoardPosition(node.Board, new Vector2() { Y = pos.Y, X = pos.X }, newZeroPosition),
                        Cost = node.Cost + 1,
                        Parent = node,
                        ZeroPosition = newZeroPosition
                    };
                    GetHeuristic(node, nodes[i]);
                    nodes[i].GoalHeuristic = nodes[i].Cost + nodes[i].Heuristic;
                    if (nodes[i].IsGoal)
                        _isGoalReach = true;
                    i++;
                }

                if (_isGoalReach)
                {
                    _goalNodeIndex = i - 1;
                }
            }
            return nodes;
        }

        private string padOutput(string s, string pad, int nLength, char c, int col)
        {
            string extraPaddingRight = "";
            int dx = nLength - s.Length;

            if (dx > 0)
            {
                for (int i = 0; i < dx; i++)
                {
                    extraPaddingRight += c;
                }
            }

            return col == 0 ? "|" + pad + s + pad + extraPaddingRight + "|" : pad + s + pad + extraPaddingRight + "|";
        }

        private void printSolution(Node node)
        {
            Console.Clear();

            int nLines = 2 * ((node.Cost * _puzzleSize) + _puzzleSize) + 2 * (node.Cost + 1);
            int totalMoves = 0;

            string[] result = new string[nLines];
            Node[] nodes = new Node[node.Cost + 1];

            int maxTremaSize = (3 * node.maxNumber.ToString().Length * _puzzleSize) + _puzzleSize;
            int maxNumberLength = node.maxNumber.ToString().Length;

            string trema = " ";
            while(maxTremaSize-- >= 0)
            {
                trema += "-";
            }

            string padding = "";
            for (int i = 0; i < maxNumberLength; i++)
            {
                padding += " ";
            }

            nLines--;

            int iNode = 0;
            while (node != null)
            {
                nodes[iNode++] = node;
                node = node.Parent;
            }

            totalMoves = iNode;

            while (iNode-- > 0)
            {
                for (int i = 0; i < nodes[iNode].Board.Length; i++)
                {
                    string tmp = " ";
                    for (int j = 0; j < nodes[iNode].Board[i].Length; j++)
                    {
                        string n = padOutput(nodes[iNode].Board[i][j].ToString(), padding, maxNumberLength, ' ', j);
                        tmp += n;
                    }
                    Console.WriteLine(trema);
                    Console.WriteLine(tmp);
                }
                Console.WriteLine(trema);
                Console.WriteLine();
            }

            Console.WriteLine();
            Console.WriteLine("A solution has been found in {0}", totalMoves);
            Console.WriteLine("Complexity in time: {0} opened states in total", _openListHash.Count);

            // TODO: complexity time / space.
            // space:len(open_set), time=len(closed_set)
            // A solution has been found in 14 move(s) and 0.002s.
            // Complexity in time: 52 opened states in total.
            // Complexity in size: 31 maximum opened states at once.
        }

        public void AStar(Node start)
        {
            SimplePriorityQueue<Node> priorityQueue = new SimplePriorityQueue<Node>();

            priorityQueue.Enqueue(start, 0.0f);

            while (priorityQueue.Count > 0)
            {
                Node headNode = priorityQueue.Dequeue();

                if (_closedListHash.ContainsKey(headNode.Id)) continue;

                _closedListHash.Add(headNode.Id, headNode);

                Node[] nodes = GetNodes(headNode);

                if (_isGoalReach)
                {
                    printSolution(nodes[_goalNodeIndex]);
                    break;
                }

                for (int i = 0; i < nodes.Length; i++)
                {
                    if (nodes[i] == null || _closedListHash.ContainsKey(nodes[i].Id)) continue;

                    if (_openListHash.TryGetValue(nodes[i].Id, out Node node))
                    {
                        if (node.Heuristic > nodes[i].Heuristic && node.Cost >= nodes[i].Cost)
                        {
                            priorityQueue.Enqueue(nodes[i], nodes[i].Heuristic);
                        }
                    }
                    else
                    {
                        priorityQueue.Enqueue(nodes[i], nodes[i].Heuristic);
                        _openListHash.Add(nodes[i].Id, nodes[i]);
                    }
                }
            }
        }
    }
}
