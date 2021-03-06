﻿namespace EaslyCompiler
{
    /// <summary>
    /// Pattern already used.
    /// </summary>
    public interface IErrorPatternAlreadyUsed : IError
    {
        /// <summary>
        /// The pattern.
        /// </summary>
        string Pattern { get; }
    }

    /// <summary>
    /// Pattern already used.
    /// </summary>
    internal class ErrorPatternAlreadyUsed : Error, IErrorPatternAlreadyUsed
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorPatternAlreadyUsed"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        /// <param name="pattern">The pattern.</param>
        public ErrorPatternAlreadyUsed(ISource source, string pattern)
            : base(source)
        {
            Pattern = pattern;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The pattern.
        /// </summary>
        public string Pattern { get; }

        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return $"Pattern '{Pattern}' already used."; } }
        #endregion
    }
}
