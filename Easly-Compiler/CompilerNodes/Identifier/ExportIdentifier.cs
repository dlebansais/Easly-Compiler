﻿namespace CompilerNode
{
    /// <summary>
    /// Specialization of <see cref="IIdentifier"/> used to ensure a separate static constructor for <see cref="EaslyCompiler.IRuleTemplate"/>.
    /// </summary>
    public interface IExportIdentifier : IIdentifier
    {
    }

    /// <summary>
    /// Specialization of <see cref="IIdentifier"/> used to ensure a separate static constructor for <see cref="EaslyCompiler.IRuleTemplate"/>.
    /// </summary>
    public class ExportIdentifier : Identifier, IExportIdentifier
    {
    }
}
