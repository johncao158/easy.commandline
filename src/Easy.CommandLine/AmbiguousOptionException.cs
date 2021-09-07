// Copyright (c) John Cao
// Licensed under the Apache License, Version 2.0.

using System.Collections.Generic;

namespace Easy.CommandLine
{
    /// <summary>
    /// The exception that is thrown when the input symbol matches more than one option.
    /// </summary>
    public class AmbiguousOptionException : OptionException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AmbiguousOptionException"/> class.
        /// </summary>
        public AmbiguousOptionException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AmbiguousOptionException"/> 
        /// class with specified long name, possible option names, and error message.
        /// </summary>
        /// <param name="longName">The input option long name.</param>
        /// <param name="possibleOptionNames">Matched possible option names.</param>
        /// <param name="message">The error message.</param>
        public AmbiguousOptionException(
            string longName,
            IEnumerable<string>? possibleOptionNames = null,
            string? message = null) : base(longName, message)
        {
            PossibleOptionNames = possibleOptionNames;
        }

        /// <summary>
        /// Gets the matched possible option names.
        /// </summary>
        public IEnumerable<string>? PossibleOptionNames { get; }
    }
}
