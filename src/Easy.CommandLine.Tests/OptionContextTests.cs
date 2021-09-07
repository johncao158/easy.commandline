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

        [Fact]
        public void Test_short_name_without_argument()
        {
            var option = _context.GetOption('a');
            Assert.NotNull(option);
            Assert.False(option.IsArgumentRequired);
            option.IsSpecified = false;

            _context.Parse("-a");
            Assert.True(option.IsSpecified);
        }

        [Fact]
        public void Test_short_name_with_argument()
        {
            var option = _context.GetOption('B');
            Assert.NotNull(option);
            Assert.Equal('B', option.ShortName);
            Assert.True(option.IsArgumentRequired);
            option.IsSpecified = false;
            option.Argument = null;

            option.IsSpecified = false;
            _context.Parse("-BG");
            Assert.True(option.IsSpecified);
            Assert.Equal("G", option.Argument);

            option.IsSpecified = false;
            _context.Parse("-B", "p");
            Assert.True(option.IsSpecified);
            Assert.Equal("p", option.Argument);

            option.IsSpecified = false;
            _context.Parse("-BB", "-B", "m");
            Assert.True(option.IsSpecified);
            Assert.Equal("m", option.Argument);
        }

        [Fact]
        public void Test_long_name_without_argument()
        {
            var option = _context.GetOption("all");
            Assert.NotNull(option);
            Assert.False(option.IsArgumentRequired);
            option.IsSpecified = false;

            _context.Parse("--all");
            Assert.True(option.IsSpecified);
        }

        [Fact]
        public void Test_long_name_with_argument()
        {
            var option = _context.GetOption("block-size");
            Assert.NotNull(option);
            Assert.True(option.IsArgumentRequired);
            option.IsSpecified = false;

            _context.Parse("--block-size", "G");
            Assert.True(option.IsSpecified);
            Assert.Equal("G", option.Argument);

            option.IsSpecified = false;
            _context.Parse("--block-size=g");
            Assert.True(option.IsSpecified);
            Assert.Equal("g", option.Argument);

            option.IsSpecified = false;
            _context.Parse("--block-size=b", "--block-size", "M", "--block-size=P");
            Assert.True(option.IsSpecified);
            Assert.Equal("P", option.Argument);
        }

        [Fact]
        public void Test_short_name_long_name_without_argument()
        {
            var option = _context.GetOption('a');
            Assert.Equal(option, _context.GetOption("all"));
            Assert.NotNull(option);
            Assert.False(option.IsArgumentRequired);
            option.IsSpecified = false;

            _context.Parse("-a");
            Assert.True(option.IsSpecified);

            option.IsSpecified = false;
            _context.Parse("--all");
            Assert.True(option.IsSpecified);
        }

        [Fact]
        public void Test_short_name_long_name_with_argument()
        {
            var option = _context.GetOption('B');
            Assert.Equal(option, _context.GetOption("block-size"));
            Assert.NotNull(option);
            Assert.True(option.IsArgumentRequired);
            option.IsSpecified = false;

            _context.Parse("-BG");
            Assert.True(option.IsSpecified);
            Assert.Equal("G", option.Argument);

            option.IsSpecified = false;
            _context.Parse("--block-size=M");
            Assert.True(option.IsSpecified);
            Assert.Equal("M", option.Argument);

            option.IsSpecified = false;
            _context.Parse("--block-size", "P");
            Assert.True(option.IsSpecified);
            Assert.Equal("P", option.Argument);

            option.IsSpecified = false;
            _context.Parse("--block-size=b", "-BK");
            Assert.True(option.IsSpecified);
            Assert.Equal("K", option.Argument);
        }

        [Fact]
        public void Test_arguments()
        {
            _context.Parse("a1");
            _context.Parse("a2", "a3");

            var arguments = _context.GetArguments();
            Assert.NotNull(arguments);
            Assert.Contains("a1", arguments);
            Assert.Contains("a2", arguments);
            Assert.Contains("a3", arguments);

            _context.Reset();
        }

        [Fact]
        public void Test_short_name_unknown_option_exception()
        {
            var exception = Record.Exception(() => _context.Parse("-x")) as OptionException;
            Assert.NotNull(exception);
            Assert.Equal('x', exception.ShortOptionName);
        }

        [Fact]
        public void Test_long_name_unknown_option_exception()
        {
            var exception = Record.Exception(() => _context.Parse("--X")) as OptionException;
            Assert.NotNull(exception);
            Assert.Equal("X", exception.LongOptionName);
        }

        [Fact]
        public void Test_long_name_ambiguous_option_exception()
        {
            var exception = Record.Exception(() => _context.Parse("--b")) as AmbiguousOptionException;
            Assert.NotNull(exception);
            Assert.Equal("b", exception.LongOptionName);
            Assert.NotEmpty(exception.PossibleOptionNames);
            Assert.Contains("block-size", exception.PossibleOptionNames);
            Assert.Contains("both", exception.PossibleOptionNames);
        }

        [Fact]
        public void Test_short_name_option_already_exists_exception()
        {
            var exception = Record.Exception(() => _context.AddOption(new Option('a', false))) as OptionException;
            Assert.NotNull(exception);
            Assert.Equal('a', exception.ShortOptionName);
        }

        [Fact]
        public void Test_long_name_option_already_exists_exception()
        {
            var exception = Record.Exception(() => _context.AddOption(new Option("all", false))) as OptionException;
            Assert.NotNull(exception);
            Assert.Equal("all", exception.LongOptionName);
        }

        [Fact]
        public void Test_long_name_option_no_argument_required_exception()
        {
            var exception = Record.Exception(() => _context.Parse("--all=X")) as OptionException;
            Assert.NotNull(exception);
            Assert.Equal("all", exception.LongOptionName);
        }

        [Fact]
        public void Test_short_name_invlid_option_argument_exception()
        {
            var exception = Record.Exception(() => _context.Parse("-B=X")) as OptionException;
            Assert.NotNull(exception);
            Assert.Equal('B', exception.ShortOptionName);

            exception = Record.Exception(() => _context.Parse("-BX")) as OptionException;
            Assert.NotNull(exception);
            Assert.Equal('B', exception.ShortOptionName);

            exception = Record.Exception(() => _context.Parse("-B", "X")) as OptionException;
            Assert.NotNull(exception);
            Assert.Equal('B', exception.ShortOptionName);
        }

        [Fact]
        public void Test_long_name_invalid_option_argument_exception()
        {
            var exception = Record.Exception(() => _context.Parse("--block-size=Z")) as OptionException;
            Assert.NotNull(exception);
            Assert.Equal("block-size", exception.LongOptionName);

            exception = Record.Exception(() => _context.Parse("--block-size", "=Z")) as OptionException;
            Assert.NotNull(exception);
            Assert.Equal("block-size", exception.LongOptionName);

            exception = Record.Exception(() => _context.Parse("--block-size", "Z")) as OptionException;
            Assert.NotNull(exception);
            Assert.Equal("block-size", exception.LongOptionName);
        }

        [Fact]
        public void Test_short_name_argument_required_exception()
        {
            var exception = Record.Exception(() => _context.Parse("-B")) as OptionException;
            Assert.NotNull(exception);
            Assert.Equal('B', exception.ShortOptionName);

            exception = Record.Exception(() => _context.Parse("-aB")) as OptionException;
            Assert.NotNull(exception);
            Assert.Equal('B', exception.ShortOptionName);
        }

        [Fact]
        public void Test_long_name_argument_required_exception()
        {
            var exception = Record.Exception(() => _context.Parse("--block-size")) as OptionException;
            Assert.NotNull(exception);
            Assert.Equal("block-size", exception.LongOptionName);

            exception = Record.Exception(() => _context.Parse("-a", "--all", "--block-size")) as OptionException;
            Assert.NotNull(exception);
            Assert.Equal("block-size", exception.LongOptionName);
        }
    }
}
