using System;
using System.Collections.Generic;
using System.Text;

namespace Linediff
{
    // Represents a block of text in a diff
    // Not sure if this should be a class hierarchy, or a single class with an enum for type
    public abstract class TextBlock
    {
        public string Text { get; private set; }

        protected TextBlock(string Text)
        {
            this.Text = Text;
        }
    }

    public class UnchangedTextBlock : TextBlock
    {
        public UnchangedTextBlock(string Text) : base(Text) { }
    }

    public class ChangedTextBlock : TextBlock
    {
        public ChangedTextBlock(string Text) : base(Text) { }
    }

    public class AddedTextBlock : TextBlock
    {
        public AddedTextBlock(string Text) : base(Text) { }
    }

    public class RemovedTextBlock : TextBlock
    {
        public RemovedTextBlock(string Text) : base(Text) { }
    }
}
