using System;
using System.IO;

namespace Linediff
{
    class Program
    {
        static void Main(string[] args)
        {
            string file = args[0];

            var fileLines = File.ReadAllLines(file);

            string prevLine = null;
            int lineNumber = 1;

            var formatter = new ConsoleDiffFormatter();

            foreach (var line in fileLines)
            {
                var diff = prevLine == null ? new Diff(line) : new Diff(prevLine, line);

                Console.WriteLine($"{LineNumberString(lineNumber)} {formatter.FormatDiff(diff)}");
                prevLine = line;
                ++lineNumber;
            }
        }

        static string LineNumberString(int lineNumber)
        {
            return String.Format("{0,6}", lineNumber);
        }
    }
}
