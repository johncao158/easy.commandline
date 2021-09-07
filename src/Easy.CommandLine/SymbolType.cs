// Copyright (c) John Cao
// Licensed under the Apache License, Version 2.0.

namespace Easy.CommandLine
{
    /// <summary>
    /// Symbol types.
    /// </summary>
    internal enum SymbolType
    {
        /// <summary>
        /// Unknown symbol type.
        /// </summary>
        Unkonwn,

        /// <summary>
        /// The symbol is a short name.
        /// </summary>
        ShortName,

        /// <summary>
        /// The symbol is a long name.
        /// </summary>
        LongName,

        /// <summary>
        /// The symbol is an argument.
        /// </summary>
        Argument
    }
}
