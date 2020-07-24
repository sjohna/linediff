using System;
using System.IO;

namespace Linediff
{
    class Program
    {
        static void Main(string[] args)
        {
            string file = args[0];

            LineDiffGui.DisplayLineDiffGui(file);
        }
    }
}
