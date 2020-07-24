using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Linediff
{
    public static class LineDiffConsole
    {
        public static void PrintDiffToConsole(string file)
        {
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
