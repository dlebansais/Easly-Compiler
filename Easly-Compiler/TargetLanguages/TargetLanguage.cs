﻿namespace EaslyCompiler
{
    using System;

    /// <summary>
    /// An interface to write down source code from compiled Easly nodes.
    /// </summary>
    public interface ITargetLanguage
    {
        /// <summary>
        /// The compiler object to translate.
        /// </summary>
        ICompiler Compiler { get; }

        /// <summary>
        /// Folder where to output the result.
        /// </summary>
        string OutputRootFolder { get; }

        /// <summary>
        /// Guid of the class where to single out a feature. Can be <see cref="Guid.Empty"/>.
        /// </summary>
        Guid SingledGuid { get; set; }

        /// <summary>
        /// Name of the class where to single out a feature. Null for the indexer.
        /// </summary>
        string SingledName { get; set; }

        /// <summary>
        /// Errors in last translation.
        /// </summary>
        IErrorList ErrorList { get; }

        /// <summary>
        /// Translates nodes from the compiler to the target language.
        /// </summary>
        void Translate();
    }

    /// <summary>
    /// A class to write down source code from compiled Easly nodes.
    /// </summary>
    public abstract class TargetLanguage : ITargetLanguage
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="TargetLanguage"/> class.
        /// </summary>
        /// <param name="compiler">The compiler object to translate.</param>
        public TargetLanguage(ICompiler compiler)
        {
            Compiler = compiler;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The compiler object to translate.
        /// </summary>
        public ICompiler Compiler { get; }

        /// <summary>
        /// Errors in last translation.
        /// </summary>
        public IErrorList ErrorList { get; } = new ErrorList();

        /// <summary>
        /// Folder where to output the result.
        /// </summary>
        public string OutputRootFolder { get; set; }

        /// <summary>
        /// Guid of the class where to single out a feature. Can be <see cref="Guid.Empty"/>.
        /// </summary>
        public Guid SingledGuid { get; set; }

        /// <summary>
        /// Name of the class where to single out a feature. Null for the indexer.
        /// </summary>
        public string SingledName { get; set; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Translates nodes from the compiler to the target language.
        /// </summary>
        public abstract void Translate();
        #endregion
    }
}
