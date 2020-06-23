using System;
using System.Collections.Generic;
using System.Diagnostics;
using NPuzzle.src;
using NPuzzle.src.Models;
using Priority_Queue;

namespace NPuzzle
{
    class MainClass
    {
        public static void Main(string[] argv)
        {
            Stopwatch sw = Stopwatch.StartNew();
            Parser.GetOptions(argv);
            sw.Stop();

            Console.WriteLine("Time taken: {0} s", sw.Elapsed.TotalSeconds);
        }
    }
}
