// Copyright(c) John Cao
// Licensed under the Apache License, Version 2.0.

using System;

namespace Easy.CommandLine
{
    /// <summary>
    /// The exception that is thrown when an option is not valid.
    /// </summary>
    public class OptionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OptionException"/> class.
        /// </summary>
        public OptionException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionException"/> class with
        /// specified error message.
        /// </summary>
        /// <param name="message">The error message.</param>
        public OptionException(string? message = null) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionException"/> class
        /// with specified option short name and error message.
        /// </summary>
        /// <param name="shortOptionName">The option short name.</param>
        /// <param name="message">The error message.</param>
        public OptionException(char shortOptionName, string? message = null) : base(message)
        {
            ShortOptionName = shortOptionName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionException"/> class
        /// with specified option long name and error message.
        /// </summary>
        /// <param name="longOptionName">The option long name.</param>
        /// <param name="message">The error message.</param>
        public OptionException(string longOptionName, string? message = null) : base(message)
        {
            LongOptionName = longOptionName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionException"/> class
        /// with specified option short name, option long name, and error message.
        /// </summary>
        /// <param name="shortOptionName">The option short name.</param>
        /// <param name="longOptionName">The option long name.</param>
        /// <param name="message">The error message.</param>
        public OptionException(char shortOptionName, string longOptionName, string? message = null) : base(message)
        {
            ShortOptionName = shortOptionName;
            LongOptionName = longOptionName;
        }

        /// <summary>
        /// Gets the option short name.
        /// </summary>
        public virtual char? ShortOptionName { get; }

        /// <summary>
        /// /Gets the option long name.
        /// </summary>
        public virtual string? LongOptionName { get; }
    }
}
