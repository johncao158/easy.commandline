// Copyright (c) John Cao
// Licensed under the Apache License, Version 2.0.

using System.Collections.Generic;

namespace Easy.CommandLine
{
    /// <summary>
    /// Defines an option context which contains all the option 
    /// information of a command line.
    /// </summary>
    public interface IOptionContext
    {
        /// <summary>
        /// Returns all the options.
        /// </summary>
        /// <returns>The options.</returns>
        IEnumerable<IOption> GetOptions();

        /// <summary>
        /// Returns the option with specified short name.
        /// </summary>
        /// <param name="shortName">The option's short name.</param>
        /// <returns>The found option.</returns>
        IOption GetOption(char shortName);

        /// <summary>
        /// Returns the option with specified long name.
        /// </summary>
        /// <param name="longName">The option's long name.</param>
        /// <returns>The found option.</returns>
        IOption GetOption(string longName);

        /// <summary>
        /// Adds a new option to the context.
        /// </summary>
        /// <param name="option">The option to be added.</param>
        void AddOption(IOption option);

        /// <summary>
        /// Returns all the arguments that are part of the command line but
        /// they don't belong to any option.
        /// </summary>
        /// <returns>The arguments collection.</returns>
        IEnumerable<string> GetArguments();

        /// <summary>
        /// Parses from a collection of string arguments.
        /// </summary>
        /// <param name="args">The collection of string arguments.</param>
        void Parse(IEnumerable<string> args);

        /// <summary>
        /// Parses from an array of string arguments.
        /// </summary>
        /// <param name="args">The array of string arguments.</param>
        void Parse(params string[] args);

        /// <summary>
        /// Resets the context. This will clear all the information that has 
        /// been parsed from any command line before.
        /// </summary>
        void Reset();
    }
}
