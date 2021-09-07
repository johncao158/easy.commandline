// Copyright (c) John Cao
// Licensed under the Apache License, Version 2.0.

using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Easy.CommandLine.Resources.Localization;

namespace Easy.CommandLine
{
    /// <summary>
    /// Represents an option context.
    /// </summary>
    public class OptionContext : IOptionContext
    {
        #region fields

        private readonly HashSet<string> _longNameSet = new();
        private readonly HashSet<char> _shortNameSet = new();

        private readonly Dictionary<char, IOption> _shortOptionsDict = new();
        private readonly List<IOption> _options = new();
        private readonly List<string> _arguments = new();

        #endregion

        #region methods

        /// <inheritdoc/>
        public virtual IEnumerable<IOption> GetOptions()
        {
            foreach (var option in _options)
            {
                yield return option;
            }
        }

        /// <inheritdoc/>
        /// <exception cref="OptionException">
        /// The <paramref name="shortName"/> is unknown. 
        /// <seealso cref="UnknownOptionException(char)"/>
        /// </exception>
        public virtual IOption GetOption(char shortName)
        {
            if (_shortOptionsDict.TryGetValue(shortName, out IOption? option))
            {
                return option;
            }

            throw UnknownOptionException(shortName);
        }

        /// <inheritdoc/>
        /// <exception cref="OptionException">
        /// The <paramref name="longName"/> is unknown.
        /// <seealso cref="UnknownOptionException(string)"/>
        /// </exception>
        /// <exception cref="CommandLine.AmbiguousOptionException">
        /// The <paramref name="longName"/> is ambiguous with more than one option long name.
        /// <seealso cref="AmbiguousOptionException(string, IEnumerable{string})"/>
        /// </exception>
        public virtual IOption GetOption(string longName)
        {
            if (!string.IsNullOrEmpty(longName))
            {
                var options = _options.Where(o => o.LongName != null && o.LongName!.StartsWith(longName)).ToList();
                if (options.Count == 1)
                {
                    return options[0];
                }

                if (options.Count > 1)
                {
                    var possibleOptionNames = options.Select(o => o.LongName!);
                    throw AmbiguousOptionException(longName, possibleOptionNames);
                }
            }

            throw UnknownOptionException(longName);
        }

        /// <inheritdoc/>
        /// <exception cref="OptionException">
        /// The context already contains an <see cref="IOption"/> that
        /// has the same <see cref="IOption.ShortName"/> or <see cref="IOption.LongName"/> as
        /// the specified <paramref name="option"/>.
        /// <seealso cref="OptionAlreadyExistsException(char)"/>;
        /// <seealso cref="OptionAlreadyExistsException(string)"/>
        /// </exception>
        public virtual void AddOption(IOption option)
        {
            var shortNameAdded = false;
            char shortName = ' ';
            if (option.ShortName.HasValue)
            {
                shortName = option.ShortName!.Value;
                if (_shortNameSet.Add(shortName))
                {
                    shortNameAdded = true;
                }
                else
                {
                    throw OptionAlreadyExistsException(shortName);
                }
            }

            var longNameAdded = false;
            if (option.LongName != null)
            {
                if (_longNameSet.Add(option.LongName!))
                {
                    longNameAdded = true;
                }
                else
                {
                    if (shortNameAdded)
                    {
                        _shortNameSet.Remove(shortName);
                    }

                    throw OptionAlreadyExistsException(option.LongName);
                }
            }

            if (shortNameAdded || longNameAdded)
            {
                _options.Add(option);

                if (shortNameAdded)
                {
                    _shortOptionsDict[shortName] = option;
                }
            }
        }

        /// <inheritdoc/>
        public virtual IEnumerable<string> GetArguments()
        {
            foreach (var arg in _arguments)
            {
                yield return arg;
            }
        }

        /// <inheritdoc/>
        /// <exception cref="OptionException">
        /// Found an unknown option name, or the option which requires an argument but
        /// not found in the specified <paramref name="args"/>.
        /// <seealso cref="UnknownOptionException(char)"/>;
        /// <seealso cref="UnknownOptionException(string)"/>;
        /// <seealso cref="ArgumentRequiredException(char)"/>;
        /// <seealso cref="ArgumentRequiredException(string)"/>
        /// </exception>
        public virtual void Parse(IEnumerable<string> args)
        {
            var type = SymbolType.Unkonwn;
            IOption? option = null;
            char shortName = ' ';
            string longName = string.Empty;

            foreach (var arg in args)
            {
                if ((type == SymbolType.ShortName || type == SymbolType.LongName) &&
                    option != null &&
                    option!.IsArgumentRequired)
                {
                    ValidateArgument(arg);
                    option!.Argument = arg;

                    Reset();
                    continue;
                }

                var index = 0;
                var sb = new StringBuilder(arg.Length);
                foreach (var c in arg)
                {
                    if (index == 0)
                    {
                        if (c == '-')
                        {
                            // might by a short name
                            type = SymbolType.ShortName;
                        }
                        else
                        {
                            // it's an argument
                            type = SymbolType.Argument;
                            sb.Append(c);
                        }
                    }
                    else if (index == 1)
                    {
                        if (c == '-')
                        {
                            if (type == SymbolType.ShortName)
                            {
                                // change to long name
                                type = SymbolType.LongName;
                            }
                            else
                            {
                                // it's an argument
                                sb.Append(c);
                            }
                        }
                        else if (type == SymbolType.ShortName)
                        {
                            // get the option
                            shortName = c;
                            option = ResetArgument(GetOption(shortName));

                            // if the option is valid, continue to next
                            if (IsValid())
                            {
                                Reset(false);
                            }
                        }
                        else
                        {
                            // it's an argument
                            sb.Append(c);
                        }
                    }
                    else if (type == SymbolType.ShortName)
                    {
                        if (option == null)
                        {
                            // new option with a short name
                            shortName = c;
                            option = ResetArgument(GetOption(c));
                            if (IsValid())
                            {
                                Reset(false);
                            }
                        }
                        else
                        {
                            // argument for the option begins
                            type = SymbolType.Argument;
                            sb.Append(c);
                        }
                    }
                    else if (index > 2 && c == '=' && type == SymbolType.LongName)
                    {
                        longName = sb.ToString();
                        sb.Clear();

                        option = ResetArgument(GetOption(longName));
                        if (option.IsArgumentRequired)
                        {
                            // argument for the option begins
                            type = SymbolType.Argument;
                        }
                        else
                        {
                            throw OptionNoArgumentRequiredException(longName);
                        }
                    }
                    else
                    {
                        sb.Append(c);
                    }

                    ++index;
                }

                var value = sb.ToString();
                if (value.Length == 0)
                {
                    continue;
                }

                switch (type)
                {
                    case SymbolType.Unkonwn:
                        break;
                    case SymbolType.ShortName:
                        break;
                    case SymbolType.LongName:
                        longName = value;
                        option = ResetArgument(GetOption(longName));
                        if (IsValid())
                        {
                            Reset();
                        }
                        break;
                    case SymbolType.Argument:
                        if (option != null && option.IsArgumentRequired)
                        {
                            ValidateArgument(value);
                            option.Argument = value;

                            if (IsValid())
                            {
                                Reset();
                            }
                        }
                        else
                        {
                            // argument doesn't belong to any option
                            _arguments.Add(value);
                        }
                        break;
                    default:
                        break;
                }
            }

            if ((type == SymbolType.ShortName || type == SymbolType.LongName) &&
                    option != null &&
                    option!.IsArgumentRequired)
            {
                if (shortName != ' ')
                {
                    throw ArgumentRequiredException(shortName);
                }

                throw ArgumentRequiredException(longName);
            }

            void Reset(bool resetType = true)
            {
                option.IsSpecified = true;

                option = null;
                shortName = ' ';
                longName = string.Empty;

                if (resetType)
                {
                    type = SymbolType.Unkonwn;
                }
            }

            IOption ResetArgument(IOption option)
            {
                option.Argument = null;
                return option;
            }

            void ValidateArgument(string? argument)
            {
                if (option!.ArgumentValidator != null && !option.ArgumentValidator(argument))
                {
                    throw InvalidOptionArgumentException(option, argument);
                }
            }

            bool IsValid()
            {
                return !option!.IsArgumentRequired || !string.IsNullOrEmpty(option.Argument);
            }
        }

        /// <inheritdoc/>
        public virtual void Parse(params string[] args)
        {
            Parse((IEnumerable<string>)args);
        }

        /// <inheritdoc/>
        public virtual void Reset()
        {
            // options
            foreach (var option in _options)
            {
                option.IsSpecified = false;

                if (option.IsArgumentRequired)
                {
                    option.Argument = null;
                }
            }

            // arguments
            _arguments.Clear();
        }

        /// <summary>
        /// <see cref="OptionException"/> to throw when a <paramref name="shortName"/> is invalid.
        /// </summary>
        /// <param name="shortName">The invalid option short name.</param>
        /// <returns>An <see cref="OptionException"/>.</returns>
        protected virtual OptionException UnknownOptionException(char shortName)
        {
            return new OptionException(shortName, string.Format(UnknownOptionExceptionMessageFormat, shortName));
        }

        /// <summary>
        /// <see cref="OptionException"/> to throw when a <paramref name="longName"/> is invalid.
        /// </summary>
        /// <param name="longName">The invalid option long name.</param>
        /// <returns>An <see cref="OptionException"/>.</returns>
        protected virtual OptionException UnknownOptionException(string longName)
        {
            return new OptionException(longName, string.Format(UnknownOptionExceptionMessageFormat2, longName));
        }

        /// <summary>
        /// <see cref="CommandLine.AmbiguousOptionException"/> to throw when
        /// <paramref name="longName"/> is ambiguous.
        /// </summary>
        /// <param name="longName">The ambiguous option long name.</param>
        /// <param name="possibleOptionNames">The matched possible option names.</param>
        /// <returns>An <see cref="CommandLine.AmbiguousOptionException"/>.</returns>
        protected virtual AmbiguousOptionException AmbiguousOptionException(
            string longName,
            IEnumerable<string> possibleOptionNames)
        {
            return new AmbiguousOptionException(
                longName,
                possibleOptionNames,
#if NETSTANDARD2_0
                string.Format(AmbiguousOptionExceptionMessageFormat, longName, string.Join(" ", possibleOptionNames.Select(n => $"' --{n}'"))));
#else
                string.Format(AmbiguousOptionExceptionMessageFormat, longName, string.Join(' ', possibleOptionNames.Select(n => $"' --{n}'"))));
#endif
        }

        /// <summary>
        /// <see cref="OptionException"/> to throw when the context already
        /// has an option with the specified <paramref name="shortName"/>.
        /// </summary>
        /// <param name="shortName">The option short name.</param>
        /// <returns>An <see cref="OptionException"/>.</returns>
        protected virtual OptionException OptionAlreadyExistsException(char shortName)
        {
            return new OptionException(
                shortName,
                string.Format(OptionAlreadyExistsExceptionMessageFormat, shortName));
        }

        /// <summary>
        /// <see cref="OptionException"/> to throw when the context already
        /// has an option with the specified <paramref name="longName"/>.
        /// </summary>
        /// <param name="longName">The option long name.</param>
        /// <returns>An <see cref="OptionException"/>.</returns>
        protected virtual OptionException OptionAlreadyExistsException(string longName)
        {
            return new OptionException(
                longName,
                string.Format(OptionAlreadyExistsExceptionMessageFormat2, longName));
        }

        /// <summary>
        /// <see cref="OptionException"/> to throw when the option with the specified
        /// <paramref name="longName"/> doesn't require an argument.
        /// </summary>
        /// <param name="longName">The option long name.</param>
        /// <returns>An <see cref="OptionException"/>.</returns>
        protected virtual OptionException OptionNoArgumentRequiredException(string longName)
        {
            return new OptionException(
                longName,
                string.Format(OptionNoArgumentRequiredExceptionFormat, longName));
        }

        /// <summary>
        /// <see cref="OptionException"/> to throw when the <paramref name="argument"/> is
        /// not valid for the <paramref name="option"/>.
        /// </summary>
        /// <param name="option">The option.</param>
        /// <param name="argument">The argument that is not valid.</param>
        /// <returns>An <see cref="OptionException"/>.</returns>
        protected virtual OptionException InvalidOptionArgumentException(
            IOption option,
            string? argument)
        {
            var hasShortName = option.ShortName.HasValue;
            var hasLongName = option.LongName != null;

            if (hasShortName && hasLongName)
            {
                return new OptionException(
                    option.ShortName!.Value,
                    option.LongName!,
                    string.Format(InvalidOptionArgumentExceptionFormat3,
                        option.ShortName!.Value, option.LongName, argument));
            }

            if (hasShortName)
            {
                return InvalidOptionArgumentException(option.ShortName!.Value, argument);
            }

            return InvalidOptionArgumentException(option.LongName!, argument);
        }

        /// <summary>
        /// <see cref="OptionException"/> to throw when the <paramref name="argument"/> is
        /// not valid for the option with the <paramref name="shortName"/>.
        /// </summary>
        /// <param name="shortName">The option short name.</param>
        /// <param name="argument">The argument that is not valid.</param>
        /// <returns>An <see cref="OptionException"/>.</returns>
        protected virtual OptionException InvalidOptionArgumentException(
            char shortName,
            string? argument)
        {
            return new OptionException(
                shortName,
                string.Format(InvalidOptionArgumentExceptionFormat, shortName, argument));
        }

        /// <summary>
        /// <see cref="OptionException"/> to throw when the <paramref name="argument"/> is
        /// not valid for the option with the <paramref name="longName"/>.
        /// </summary>
        /// <param name="longName">The option long name.</param>
        /// <param name="argument">The argument that is not valid.</param>
        /// <returns>An <see cref="OptionException"/>.</returns>
        protected virtual OptionException InvalidOptionArgumentException(
            string longName,
            string? argument)
        {
            return new OptionException(
                longName,
                string.Format(InvalidOptionArgumentExceptionFormat2, longName, argument));
        }

        /// <summary>
        /// <see cref="OptionException"/> to throw when the option with the
        /// <paramref name="shortName"/> requires an argument but not provided.
        /// </summary>
        /// <param name="shortName">The option short name.</param>
        /// <returns>An <see cref="OptionException"/>.</returns>
        protected virtual OptionException ArgumentRequiredException(char shortName)
        {
            return new OptionException(
                shortName,
                string.Format(ArgumentRequiredExceptionMessageFormat, shortName));
        }

        /// <summary>
        /// <see cref="OptionException"/> to throw when the option with the
        /// <paramref name="longName"/> requires an argument but not provided.
        /// </summary>
        /// <param name="longName">The option long name.</param>
        /// <returns>An <see cref="OptionException"/>.</returns>
        protected virtual OptionException ArgumentRequiredException(string longName)
        {
            return new OptionException(
                longName,
                string.Format(ArgumentRequiredExceptionMessageFormat, longName));
        }

#endregion
    }
}
