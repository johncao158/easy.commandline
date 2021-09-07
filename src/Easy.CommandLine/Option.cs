// Copyright (c) John Cao
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static Easy.CommandLine.Resources.Localization;

namespace Easy.CommandLine
{
    /// <summary>
    /// Represents an command option.
    /// </summary>
    public class Option : IOption
    {
        #region fields

        private static readonly HashSet<char> s_invalidShortNames = new() { ' ', '-' };
        private static readonly Regex s_validLongNameRegex =
            new("^[^\\s=]+$", RegexOptions.Compiled);

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Option"/> class
        /// with specified short name and a <see cref="bool"/> value
        /// to indicate whether the option requires an argument.
        /// </summary>
        /// <param name="shortName">The short name of the option.</param>
        /// <param name="isArgumentRequired">Whether the option requires an argument.</param>
        /// <exception cref="OptionException">
        /// The <paramref name="shortName"/> is invalid. 
        /// (<paramref name="shortName"/> should not be ' ' or '-')
        /// </exception>
        public Option(char shortName, bool isArgumentRequired)
        {
            if (s_invalidShortNames.Contains(shortName))
            {
                throw InvalidOptionException(shortName);
            }

            ShortName = shortName;
            IsArgumentRequired = isArgumentRequired;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Option"/> class
        /// with specified long name and a <see cref="bool"/> value
        /// to indicate whether the option requires an argument.
        /// </summary>
        /// <param name="longName">The long name of the option.</param>
        /// <param name="isArgumentRequired">Whether the option requires an argument.</param>
        /// <exception cref="OptionException">
        /// The <paramref name="longName"/> is invalid. 
        /// (<paramref name="longName"/> should not contain any ' ' or '=')
        /// </exception>
        public Option(string longName, bool isArgumentRequired)
        {
            if (!s_validLongNameRegex.IsMatch(longName))
            {
                throw InvalidOptionException(longName);
            }

            LongName = longName;
            IsArgumentRequired = isArgumentRequired;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Option"/> class
        /// with specified short name, long name, and a <see cref="bool"/> value
        /// to indicate whether the option requires an argument.
        /// </summary>
        /// <param name="shortName">The short name of the option.</param>
        /// <param name="longName">The long name of the option.</param>
        /// <param name="isArgumentRequired">Whether the option requires an argument.</param>
        /// <exception cref="OptionException">
        /// The <paramref name="shortName"/> or the <paramref name="longName"/> is invalid.
        /// (<paramref name="shortName"/> should not be ' ' or '-';
        /// <paramref name="longName"/> should not contain any ' ' or '=')
        /// </exception>
        public Option(char shortName, string longName, bool isArgumentRequired)
        {
            if (s_invalidShortNames.Contains(shortName))
            {
                throw InvalidOptionException(shortName);
            }

            if (!s_validLongNameRegex.IsMatch(longName))
            {
                throw InvalidOptionException(longName);
            }

            ShortName = shortName;
            LongName = longName;
            IsArgumentRequired = isArgumentRequired;
        }

        #region properties

        /// <inheritdoc/>
        public virtual char? ShortName { get; set; }

        /// <inheritdoc/>
        public virtual string? LongName { get; set; }

        /// <inheritdoc/>
        public virtual bool IsSpecified { get; set; }

        /// <inheritdoc/>
        public virtual string? Argument { get; set; }

        /// <inheritdoc/>
        public virtual bool IsArgumentRequired { get; }

        /// <inheritdoc/>
        public virtual Func<string?, bool>? ArgumentValidator { get; set; }

        #endregion

        #region methods

        /// <summary>
        /// <see cref="OptionException"/> to throw when the specified <paramref name="shortName"/> is invalid.
        /// </summary>
        /// <param name="shortName">The invalid option short name.</param>
        /// <returns>An <see cref="OptionException"/>.</returns>
        protected virtual OptionException InvalidOptionException(char shortName)
        {
            return new OptionException(
                shortName,
                string.Format(InvalidOptionExceptionMessageFormat, shortName));
        }

        /// <summary>
        /// <see cref="OptionException"/> to throw when the specified <paramref name="longName"/> is invalid.
        /// </summary>
        /// <param name="longName">The invalid option long name.</param>
        /// <returns>An <see cref="OptionException"/>.</returns>
        protected virtual OptionException InvalidOptionException(string longName)
        {
            return new OptionException(
                longName,
                string.Format(InvalidOptionExceptionMessageFormat2, longName));
        }

        #endregion
    }
}
