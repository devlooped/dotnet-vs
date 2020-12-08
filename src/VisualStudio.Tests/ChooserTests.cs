using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace VisualStudio.Tests
{
    public class ChooserTests
    {
        readonly TextWriter output;

        public ChooserTests(ITestOutputHelper output) =>
            this.output = new OutputHelperTextWriter(output);

        Chooser CreateChooser(Func<string> input) => new ChooserTest(input);

        [Fact]
        public void when_items_is_empty_then_returns_null()
        {
            var chooser = CreateChooser(() => throw new Exception("should not be asked"));

            var result = chooser.Choose(Enumerable.Empty<string>(), output);

            Assert.Null(result);
        }

        [Fact]
        public void when_items_is_empty_then_returns_empty_list()
        {
            var chooser = CreateChooser(() => throw new Exception("should not be asked"));

            var result = chooser.ChooseMany(Enumerable.Empty<string>(), output);

            Assert.Empty(result);
        }

        [Fact]
        public void when_items_contains_single_element_then_returns_item()
        {
            var chooser = CreateChooser(() => throw new Exception("should not be asked"));

            var result = chooser.Choose(new[] { "A" }, output);

            Assert.Equal("A", result);
        }

        [Fact]
        public void when_items_contains_single_element_then_returns_items()
        {
            var chooser = CreateChooser(() => throw new Exception("should not be asked"));

            var results = chooser.ChooseMany(new[] { "A" }, output);

            Assert.Single(results, "A");
        }

        [Theory]
        [InlineData("1", "A")]
        [InlineData("2", "B")]
        [InlineData("3", "C")]
        public void when_valid_index_is_entered_then_returns_item(string enteredValue, string expectedItem)
        {
            var chooser = CreateChooser(() => enteredValue);

            var result = chooser.Choose(new[] { "A", "B", "C" }, output);

            Assert.Equal(expectedItem, result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("one")]
        public void when_invalid_index_is_entered_then_returns_null(string enteredValue)
        {
            var chooser = CreateChooser(() => enteredValue);

            var result = chooser.Choose(new[] { "A", "B", "C" }, output);

            Assert.Null(result);
        }

        [Theory]
        [InlineData("1", "A")]
        [InlineData("2", "B")]
        [InlineData("3", "C")]
        public void when_valid_index_is_entered_then_returns_single_item(string enteredValue, string expectedItem)
        {
            var chooser = CreateChooser(() => enteredValue);

            var results = chooser.ChooseMany(new[] { "A", "B", "C" }, output).ToList();

            Assert.Single(results, expectedItem);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("one")]
        public void when_invalid_index_is_entered_then_returns_empty_list(string enteredValue)
        {
            var chooser = CreateChooser(() => enteredValue);

            var results = chooser.ChooseMany(new[] { "A", "B", "C" }, output);

            Assert.Empty(results);
        }

        [Fact]
        public void when_valid_inverse_index_is_entered_then_returns_items()
        {
            var chooser = CreateChooser(() => "!2");

            var results = chooser.ChooseMany(new[] { "A", "B", "C" }, output).ToList();

            Assert.Equal(2, results.Count);
            Assert.Equal("A", results[0]);
            Assert.Equal("C", results[1]);
        }

        [Theory]
        [InlineData("a")]
        [InlineData("A")]
        [InlineData("all")]
        [InlineData("All")]
        public void when_selecting_all_then_returns_all_items(string all)
        {
            var chooser = CreateChooser(() => all);

            var results = chooser.ChooseMany(new[] { "A", "B", "C" }, output).ToList();

            Assert.Equal(3, results.Count);
            Assert.Equal("A", results[0]);
            Assert.Equal("B", results[1]);
            Assert.Equal("C", results[2]);
        }

        class ChooserTest : Chooser
        {
            public ChooserTest(Func<string> input) : base("test", input) { }
        }
    }
}
