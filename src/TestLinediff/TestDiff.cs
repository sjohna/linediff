using NUnit.Framework;
using Linediff;

namespace TestLinediff
{
    public class TestDiff
    {
        [Test]
        public void ConstructedWithOneString()
        {
            var diff = new Diff("abcde");

            Assert.That(diff.Blocks.Count, Is.EqualTo(1));
            Assert.That(diff.Blocks[0], Is.InstanceOf(typeof(UnchangedTextBlock)));
            Assert.That(diff.Blocks[0].Text, Is.EqualTo("abcde"));
        }

        [Test]
        public void ConstructedWithTwoIdenticalStrings()
        {
            var diff = new Diff("abcde", "abcde");

            Assert.That(diff.Blocks.Count, Is.EqualTo(1));
            Assert.That(diff.Blocks[0], Is.InstanceOf(typeof(UnchangedTextBlock)));
            Assert.That(diff.Blocks[0].Text, Is.EqualTo("abcde"));
        }

        [Test]
        public void DifferentStringsOfSameLength_OneDifference()
        {
            var diff = new Diff("abcde", "abCde");

            Assert.That(diff.Blocks.Count, Is.EqualTo(3));

            Assert.That(diff.Blocks[0], Is.InstanceOf(typeof(UnchangedTextBlock)));
            Assert.That(diff.Blocks[0].Text, Is.EqualTo("ab"));

            Assert.That(diff.Blocks[1], Is.InstanceOf(typeof(ChangedTextBlock)));
            Assert.That(diff.Blocks[1].Text, Is.EqualTo("C"));

            Assert.That(diff.Blocks[2], Is.InstanceOf(typeof(UnchangedTextBlock)));
            Assert.That(diff.Blocks[2].Text, Is.EqualTo("de"));
        }

        [Test]
        public void DifferentStringsOfSameLength_TwoDifferences()
        {
            var diff = new Diff("abcde", "aBcDe");

            Assert.That(diff.Blocks.Count, Is.EqualTo(5));

            Assert.That(diff.Blocks[0], Is.InstanceOf(typeof(UnchangedTextBlock)));
            Assert.That(diff.Blocks[0].Text, Is.EqualTo("a"));

            Assert.That(diff.Blocks[1], Is.InstanceOf(typeof(ChangedTextBlock)));
            Assert.That(diff.Blocks[1].Text, Is.EqualTo("B"));

            Assert.That(diff.Blocks[2], Is.InstanceOf(typeof(UnchangedTextBlock)));
            Assert.That(diff.Blocks[2].Text, Is.EqualTo("c"));

            Assert.That(diff.Blocks[3], Is.InstanceOf(typeof(ChangedTextBlock)));
            Assert.That(diff.Blocks[3].Text, Is.EqualTo("D"));

            Assert.That(diff.Blocks[4], Is.InstanceOf(typeof(UnchangedTextBlock)));
            Assert.That(diff.Blocks[4].Text, Is.EqualTo("e"));
        }

        [Test]
        public void DifferentStringsOfSameLength_OneDifference_MultipleCharacters()
        {
            var diff = new Diff("abcde", "aBCDe");

            Assert.That(diff.Blocks.Count, Is.EqualTo(3));

            Assert.That(diff.Blocks[0], Is.InstanceOf(typeof(UnchangedTextBlock)));
            Assert.That(diff.Blocks[0].Text, Is.EqualTo("a"));

            Assert.That(diff.Blocks[1], Is.InstanceOf(typeof(ChangedTextBlock)));
            Assert.That(diff.Blocks[1].Text, Is.EqualTo("BCD"));

            Assert.That(diff.Blocks[2], Is.InstanceOf(typeof(UnchangedTextBlock)));
            Assert.That(diff.Blocks[2].Text, Is.EqualTo("e"));
        }

        [Test]
        public void DifferentStringsOfSameLength_AllDifferent()
        {
            var diff = new Diff("abcde", "ABCDE");

            Assert.That(diff.Blocks.Count, Is.EqualTo(1));

            Assert.That(diff.Blocks[0], Is.InstanceOf(typeof(ChangedTextBlock)));
            Assert.That(diff.Blocks[0].Text, Is.EqualTo("ABCDE"));
        }

        [Test]
        public void FromStringLonger()
        {
            var diff = new Diff("abcdefghi", "abcde");

            Assert.That(diff.Blocks.Count, Is.EqualTo(2));

            Assert.That(diff.Blocks[0], Is.InstanceOf(typeof(UnchangedTextBlock)));
            Assert.That(diff.Blocks[0].Text, Is.EqualTo("abcde"));

            Assert.That(diff.Blocks[1], Is.InstanceOf(typeof(RemovedTextBlock)));
        }

        [Test]
        public void ToStringLonger()
        {
            var diff = new Diff("abcde", "abcdefghi");

            Assert.That(diff.Blocks.Count, Is.EqualTo(2));

            Assert.That(diff.Blocks[0], Is.InstanceOf(typeof(UnchangedTextBlock)));
            Assert.That(diff.Blocks[0].Text, Is.EqualTo("abcde"));

            Assert.That(diff.Blocks[1], Is.InstanceOf(typeof(AddedTextBlock)));
            Assert.That(diff.Blocks[1].Text, Is.EqualTo("fghi"));
        }

        [Test]
        public void DifferentLengthStringsWithDifferences_1()
        {
            var diff = new Diff("test test test", "test TEST");

            Assert.That(diff.Blocks.Count, Is.EqualTo(3));

            Assert.That(diff.Blocks[0], Is.InstanceOf(typeof(UnchangedTextBlock)));
            Assert.That(diff.Blocks[0].Text, Is.EqualTo("test "));

            Assert.That(diff.Blocks[1], Is.InstanceOf(typeof(ChangedTextBlock)));
            Assert.That(diff.Blocks[1].Text, Is.EqualTo("TEST"));

            Assert.That(diff.Blocks[2], Is.InstanceOf(typeof(RemovedTextBlock)));
        }

        [Test]
        public void DifferentLengthStringsWithDifferences_2()
        {
            var diff = new Diff("test TEST", "test test test");

            Assert.That(diff.Blocks.Count, Is.EqualTo(3));

            Assert.That(diff.Blocks[0], Is.InstanceOf(typeof(UnchangedTextBlock)));
            Assert.That(diff.Blocks[0].Text, Is.EqualTo("test "));

            Assert.That(diff.Blocks[1], Is.InstanceOf(typeof(ChangedTextBlock)));
            Assert.That(diff.Blocks[1].Text, Is.EqualTo("test"));

            Assert.That(diff.Blocks[2], Is.InstanceOf(typeof(AddedTextBlock)));
            Assert.That(diff.Blocks[2].Text, Is.EqualTo(" test"));
        }
    }
}