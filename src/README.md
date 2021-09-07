Easy.CommandLine
==================

Easy.CommandLine is an open-source and cross-platform package that can help us to define and parse the command line options and arguments. 

## Just make things easier ~

- We want to make things easier.  
- We just want it to do the job that it needs to do.  
- We don't want to add any features that are way beyond its core task.

## Get Started

```cs

using System.Collections.Generic;
using Xunit;

namespace Easy.CommandLine.Tests
{
    public class OptionContextTests
    {
        private readonly IOptionContext _context;

        public OptionContextTests()
        {
            _context = new OptionContext();

            // -a, -all
            _context.AddOption(new Option('a', "all", false));

            // -B, --block-size=SIZE
            var set = new HashSet<string>(new List<string>
            {
                "m", "M",
                "b", "B",
                "k", "K",
                "g", "G",
                "p", "P"
            });
            _context.AddOption(new Option('B', "block-size", true)
            {
                ArgumentValidator = a => set.Contains(a)
            });

            // --both
            _context.AddOption(new Option("both", false));
        }

        [Fact]
        public void Test()
        {
            _context.Reset();
            _context.Parse("-aBG", "--block-size=M", "--block-size=B", "--both", "hello", "world");

            var option = _context.GetOption('a');
            Assert.NotNull(option);
            Assert.True(option.IsSpecified);
            Assert.False(option.IsArgumentRequired);

            option = _context.GetOption("all");
            Assert.NotNull(option);
            Assert.True(option.IsSpecified);
            Assert.False(option.IsArgumentRequired);

            option = _context.GetOption('B');
            Assert.NotNull(option);
            Assert.True(option.IsSpecified);
            Assert.True(option.IsArgumentRequired);
            Assert.Equal("B", option.Argument);

            option = _context.GetOption("block-size");
            Assert.NotNull(option);
            Assert.True(option.IsSpecified);
            Assert.True(option.IsArgumentRequired);
            Assert.Equal("B", option.Argument);

            option = _context.GetOption("both");
            Assert.NotNull(option);
            Assert.True(option.IsSpecified);
            Assert.False(option.IsArgumentRequired);

            var arguments = _context.GetArguments();
            Assert.NotNull(arguments);
            Assert.Contains("hello", arguments);
            Assert.Contains("world", arguments);
            Assert.DoesNotContain("x", arguments);
        }
    }
}

```
