using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using  Pastel;

namespace Linediff
{
    public class ConsoleDiffFormatter
    {
        public string FormatDiff(Diff diff)
        {
            var formattedLineBuilder = new StringBuilder();

            foreach (var block in diff.Blocks)
            {
                formattedLineBuilder.Append(Format(block as dynamic));
            }

            return formattedLineBuilder.ToString();
        }

        private string Format(TextBlock block)
        {
            return block.Text;
        }

        private string Format(UnchangedTextBlock block)
        {
            return block.Text;
        }

        private string Format(ChangedTextBlock block)
        {
            return block.Text.PastelBg(Color.Magenta);
        }

        private string Format(AddedTextBlock block)
        {
            return block.Text.PastelBg(Color.Green);
        }
        private string Format(RemovedTextBlock block)
        {
            return block.Text.PastelBg(Color.Red);
        }
    }
}
