namespace EaslyCompiler
{
    /// <summary>
    /// Represents a type of constant number that can be ordered and compared.
    /// </summary>
    public interface IOrderedLanguageConstant : ILanguageConstant
    {
        /// <summary>
        /// Checks if another constant is greater than this instance.
        /// </summary>
        /// <param name="other">The other instance.</param>
        //bool IsConstantGreater(IOrderedLanguageConstant other);
    }
}
