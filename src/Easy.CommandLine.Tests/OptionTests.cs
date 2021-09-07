using Xunit;

namespace Easy.CommandLine.Tests
{
    public class OptionTests
    {
        [Fact]
        public void Test_short_name()
        {
            var option = new Option('c', false);
            Assert.Equal('c', option.ShortName);
            Assert.Null(option.LongName);
            Assert.Null(option.Argument);
            Assert.False(option.IsArgumentRequired);
            Assert.False(option.IsSpecified);
        }

        [Fact]
        public void Test_long_name()
        {
            var option = new Option("call", true);
            Assert.Equal("call", option.LongName);
            Assert.Null(option.ShortName);
            Assert.Null(option.Argument);
            Assert.True(option.IsArgumentRequired);
            Assert.False(option.IsSpecified);
        }

        [Fact]
        public void Test_short_name_long_name()
        {
            var option = new Option('c', "call", false);
            Assert.Equal('c', option.ShortName);
            Assert.Equal("call", option.LongName);
            Assert.Null(option.Argument);
            Assert.False(option.IsArgumentRequired);
            Assert.False(option.IsSpecified);
        }

        [Fact]
        public void Test_short_name_invalid_option_exception()
        {
            var exception = Record.Exception(() => new Option(' ', false)) as OptionException;
            Assert.NotNull(exception);
            Assert.Equal(' ', exception.ShortOptionName);

            exception = Record.Exception(() => new Option('-', true)) as OptionException;
            Assert.NotNull(exception);
            Assert.Equal('-', exception.ShortOptionName);
        }

        [Fact]
        public void Test_long_name_invalid_option_exception()
        {
            var exception = Record.Exception(() => new Option(" ", false)) as OptionException;
            Assert.NotNull(exception);
            Assert.Equal(" ", exception.LongOptionName);

            exception = Record.Exception(() => new Option("=", false)) as OptionException;
            Assert.NotNull(exception);
            Assert.Equal("=", exception.LongOptionName);

            exception = Record.Exception(() => new Option("a b", false)) as OptionException;
            Assert.NotNull(exception);
            Assert.Equal("a b", exception.LongOptionName);

            exception = Record.Exception(() => new Option("a=b", false)) as OptionException;
            Assert.NotNull(exception);
            Assert.Equal("a=b", exception.LongOptionName);
        }
    }
}
