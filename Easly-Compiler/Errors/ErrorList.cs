namespace EaslyCompiler
{
    using System.Collections.Generic;

    /// <summary>
    /// A list of errors.
    /// </summary>
    public interface IErrorList : IList<IError>
    {
    }

    /// <summary>
    /// A list of errors.
    /// </summary>
    public class ErrorList : List<IError>, IErrorList
    {
    }
}
