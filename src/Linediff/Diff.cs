using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace Linediff
{
    public class Diff
    {
        public ImmutableList<TextBlock> Blocks { get; private set; }

        public int LineLength { get; private set;}

        public string FullText => string.Join("", Blocks.Select(b => b.Text));

        public Diff(string line)
        {
            var blocksBuilder = ImmutableList.CreateBuilder<TextBlock>();
            blocksBuilder.Add(new UnchangedTextBlock(line));
            Blocks = blocksBuilder.ToImmutable();
            LineLength = Blocks.Select(block => block.Text.Length).Sum();
        }

        public Diff(string lineDiffFrom, string lineDiffTo)
        {
            var blocksBuilder = ImmutableList.CreateBuilder<TextBlock>();

            int startIndex = 0;

            while (startIndex < Math.Min(lineDiffFrom.Length, lineDiffTo.Length))
            {
                blocksBuilder.Add(CreateBlock(lineDiffFrom, lineDiffTo, ref startIndex));
            }

            if (lineDiffFrom.Length > lineDiffTo.Length)
            {
                blocksBuilder.Add(new RemovedTextBlock(new string(' ', lineDiffFrom.Length - startIndex)));
            }
            else if (lineDiffFrom.Length < lineDiffTo.Length)
            {
                blocksBuilder.Add(new AddedTextBlock(lineDiffTo.Substring(startIndex)));
            }

            Blocks = blocksBuilder.ToImmutable();
            LineLength = Blocks.Select(block => block.Text.Length).Sum();
        }

        private TextBlock CreateBlock(string lineDiffFrom, string lineDiffTo, ref int startIndex)
        {
            var blockBuilder = new StringBuilder();

            bool firstCharsEqual = lineDiffFrom[startIndex] == lineDiffTo[startIndex];

            int blockBeginIndex = startIndex;
            ++startIndex;

            while (startIndex < Math.Min(lineDiffFrom.Length, lineDiffTo.Length) && (lineDiffFrom[startIndex] == lineDiffTo[startIndex]) == firstCharsEqual)
            {
                ++startIndex;
            }

            if (firstCharsEqual)
            {
                return new UnchangedTextBlock(lineDiffTo.Substring(blockBeginIndex, startIndex - blockBeginIndex));
            }
            else
            {
                return new ChangedTextBlock(lineDiffTo.Substring(blockBeginIndex, startIndex - blockBeginIndex));
            }
        }
    }
}
