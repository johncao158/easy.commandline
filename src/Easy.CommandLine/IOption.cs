// Copyright (c) John Cao
// Licensed under the Apache License, Version 2.0.

using System;

namespace Easy.CommandLine
{
    /// <summary>
    /// Defines a command option.
    /// </summary>
    public interface IOption
    {
        /// <summary>
        /// Short name of the option. (e.g. -a, -B)
        /// </summary>
        char? ShortName { get; set; }

        /// <summary>
        /// Long name of the option. (e.g. --all, --block-size)
        /// </summary>
        string? LongName { get; set; }

        /// <summary>
        /// Whether the option has been specified from the command line.
        /// </summary>
        bool IsSpecified { get; set; }

        /// <summary>
        /// The argument of the option. It's valid only when 
        /// <see cref="IsArgumentRequired"/> is true.
        /// </summary>
        string? Argument { get; set; }

        /// <summary>
        /// Determines whether this option requires an argument.
        /// </summary>
        bool IsArgumentRequired { get; }

        /// <summary>
        /// A method to validate the argument.
        /// </summary>
        Func<string?, bool>? ArgumentValidator { get; set; }
    }
}
