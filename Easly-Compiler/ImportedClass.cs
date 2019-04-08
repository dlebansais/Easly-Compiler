namespace EaslyCompiler
{
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// Specifications of an imported class in a library.
    /// </summary>
    public interface IImportedClass
    {
        /// <summary>
        /// The imported class.
        /// </summary>
        IClass Item { get; }

        /// <summary>
        /// The class importing.
        /// </summary>
        IClass ParentSource { get; }

        /// <summary>
        /// The import type.
        /// </summary>
        BaseNode.ImportType ImportType { get; set; }

        /// <summary>
        /// True if <see cref="ImportType"/> is valid.
        /// </summary>
        bool IsTypeAssigned { get; }

        /// <summary>
        /// The import location.
        /// </summary>
        IImport ImportLocation { get; set; }

        /// <summary>
        /// True if <see cref="ImportLocation"/> is valid.
        /// </summary>
        bool IsLocationAssigned { get; }

        /// <summary>
        /// Sets the parent source.
        /// </summary>
        /// <param name="parentSource">The parent source.</param>
        void SetParentSource(IClass parentSource);
    }

    /// <summary>
    /// Specifications of an imported class in a library.
    /// </summary>
    public class ImportedClass : IImportedClass
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ImportedClass"/> class.
        /// </summary>
        /// <param name="item">The imported class.</param>
        public ImportedClass(IClass item)
        {
            Item = item;
            _ImportType = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportedClass"/> class.
        /// </summary>
        /// <param name="item">The imported class.</param>
        /// <param name="importType">The import type.</param>
        public ImportedClass(IClass item, BaseNode.ImportType importType)
        {
            Item = item;
            _ImportType = importType;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The imported class.
        /// </summary>
        public IClass Item { get; private set; }

        /// <summary>
        /// The class importing.
        /// </summary>
        public IClass ParentSource { get; private set; }

        /// <summary>
        /// The import type.
        /// </summary>
        public BaseNode.ImportType ImportType
        {
            get
            {
                Debug.Assert(_ImportType.HasValue);
                return _ImportType.Value;
            }
            set
            {
                _ImportType = value;
            }
        }
        private BaseNode.ImportType? _ImportType;

        /// <summary>
        /// True if <see cref="ImportType"/> is valid.
        /// </summary>
        public bool IsTypeAssigned { get { return _ImportType != null; } }

        /// <summary>
        /// The import location.
        /// </summary>
        public IImport ImportLocation
        {
            get
            {
                Debug.Assert(_ImportLocation != null);
                return _ImportLocation;
            }
            set
            {
                Debug.Assert(value != null);
                _ImportLocation = value;
            }
        }
        private IImport _ImportLocation;

        /// <summary>
        /// True if <see cref="ImportLocation"/> is valid.
        /// </summary>
        public bool IsLocationAssigned { get { return _ImportLocation != null; } }
        #endregion

        #region Client Interface
        /// <summary>
        /// Sets the parent source.
        /// </summary>
        /// <param name="parentSource">The parent source.</param>
        public virtual void SetParentSource(IClass parentSource)
        {
            Debug.Assert(parentSource != null);
            Debug.Assert(ParentSource == null);

            ParentSource = parentSource;
        }
        #endregion
    }
}
