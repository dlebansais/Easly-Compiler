namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A C# instruction.
    /// </summary>
    public interface ICSharpCommandInstruction : ICSharpInstruction
    {
        /// <summary>
        /// The Easly instruction from which the C# instruction is created.
        /// </summary>
        new ICommandInstruction Source { get; }

        /// <summary>
        /// The path to the called feature.
        /// </summary>
        ICSharpQualifiedName Command { get; }

        /// <summary>
        /// The feature call.
        /// </summary>
        ICSharpFeatureCall FeatureCall { get; }

        /// <summary>
        /// True if the call should skip the last identifier in the path.
        /// </summary>
        bool SkipLastInPath { get; }
    }

    /// <summary>
    /// A C# instruction.
    /// </summary>
    public class CSharpCommandInstruction : CSharpInstruction, ICSharpCommandInstruction
    {
        #region Init
        /// <summary>
        /// Creates a new C# instruction.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="parentFeature">The parent feature.</param>
        /// <param name="source">The Easly instruction from which the C# instruction is created.</param>
        public static ICSharpCommandInstruction Create(ICSharpContext context, ICSharpFeature parentFeature, ICommandInstruction source)
        {
            return new CSharpCommandInstruction(context, parentFeature, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpCommandInstruction"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="parentFeature">The parent feature.</param>
        /// <param name="source">The Easly instruction from which the C# instruction is created.</param>
        protected CSharpCommandInstruction(ICSharpContext context, ICSharpFeature parentFeature, ICommandInstruction source)
            : base(context, parentFeature, source)
        {
            Command = CSharpQualifiedName.Create(context, (IQualifiedName)source.Command, parentFeature, null, false);
            FeatureCall = new CSharpFeatureCall(context, source.FeatureCall.Item);

            ICompiledTypeWithFeature ResolvedBaseType = Source.CommandFinalType.Item.ResolvedBaseType.Item;
            ICSharpTypeWithFeature FinalType = CSharpType.Create(context, ResolvedBaseType) as ICSharpTypeWithFeature;
            Debug.Assert(FinalType != null);

            IList<ICSharpClassType> ConformingClassTypeList = FinalType.ConformingClassTypeList;

            bool InheritFromDotNetEvent = false;
            bool IsNumberGuid = false;
            foreach (ICSharpClassType Item in ConformingClassTypeList)
            {
                ICSharpClass CallClass = Item.Class;
                InheritFromDotNetEvent |= CallClass.InheritFromDotNetEvent;

                IsNumberGuid = CallClass.Source.ClassGuid == LanguageClasses.Number.Guid;
            }

            SkipLastInPath = InheritFromDotNetEvent;
            IsCallingNumberFeature = IsNumberGuid;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly instruction from which the C# instruction is created.
        /// </summary>
        public new ICommandInstruction Source { get { return (ICommandInstruction)base.Source; } }

        /// <summary>
        /// The path to the called feature.
        /// </summary>
        public ICSharpQualifiedName Command { get; }

        /// <summary>
        /// The feature call.
        /// </summary>
        public ICSharpFeatureCall FeatureCall { get; }

        /// <summary>
        /// True if the call should skip the last identifier in the path.
        /// </summary>
        public bool SkipLastInPath { get; }

        /// <summary>
        /// True if calling a feature of the Number class.
        /// </summary>
        public bool IsCallingNumberFeature { get; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Writes down the C# instruction.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        public override void WriteCSharp(ICSharpWriter writer)
        {
            string CommandText;

            if (IsCallingNumberFeature)
            {
                CommandText = Command.CSharpText(writer, 0);
                IList<IIdentifier> ValidPath = ((IQualifiedName)Source.Command).ValidPath.Item;
                IIdentifier FinalFeatureIdentifier = ValidPath[ValidPath.Count - 1];

                if (FinalFeatureIdentifier.ValidText.Item == "Increment")
                {
                    CommandText = CommandText.Substring(0, CommandText.Length - 10);
                    writer.WriteIndentedLine($"{CommandText}++;");
                    return;
                }

                else if (FinalFeatureIdentifier.ValidText.Item == "Decrement")
                {
                    CommandText = CommandText.Substring(0, CommandText.Length - 10);
                    writer.WriteIndentedLine($"{CommandText}--;");
                    return;
                }
            }

            CommandText = Command.CSharpText(writer, SkipLastInPath ? 1 : 0);
            string ArgumentListText = CSharpArgument.CSharpArgumentList(writer, FeatureCall, new List<ICSharpQualifiedName>());

            writer.WriteIndentedLine($"{CommandText}({ArgumentListText});");
        }
        #endregion
    }
}
