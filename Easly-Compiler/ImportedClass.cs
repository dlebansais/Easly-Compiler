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
        BaseNode.ImportType ImportType { get; }

        /// <summary>
        /// True if <see cref="ImportType"/> is valid.
        /// </summary>
        bool IsTypeAssigned { get; }

        /// <summary>
        /// The import location.
        /// </summary>
        IImport ImportLocation { get; }

        /// <summary>
        /// True if <see cref="ImportLocation"/> is valid.
        /// </summary>
        bool IsLocationAssigned { get; }

        /// <summary>
        /// Sets the parent source.
        /// </summary>
        /// <param name="parentSource">The parent source.</param>
        void SetParentSource(IClass parentSource);

        /// <summary>
        /// Sets the import type.
        /// </summary>
        /// <param name="importType">The import type.</param>
        void SetImportType(BaseNode.ImportType importType);

        /// <summary>
        /// Sets the import location.
        /// </summary>
        /// <param name="importLocation">The import location.</param>
        void SetImportLocation(IImport importLocation);
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
            IsTypeAssigned = true;
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
                Debug.Assert(IsTypeAssigned);
                return _ImportType;
            }
        }
        private BaseNode.ImportType _ImportType;

        /// <summary>
        /// True if <see cref="ImportType"/> is valid.
        /// </summary>
        public bool IsTypeAssigned { get; private set; }

        /// <summary>
        /// The import location.
        /// </summary>
        public IImport ImportLocation
        {
            get
            {
                Debug.Assert(IsLocationAssigned);
                return _ImportLocation;
            }
        }
        private IImport _ImportLocation;

        /// <summary>
        /// True if <see cref="ImportLocation"/> is valid.
        /// </summary>
        public bool IsLocationAssigned { get; private set; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Sets the parent source.
        /// </summary>
        /// <param name="parentSource">The parent source.</param>
        public virtual void SetParentSource(IClass parentSource)
        {
            Debug.Assert(parentSource != null);
            Debug.Assert(ParentSource == null || ParentSource == parentSource);

            ParentSource = parentSource;
        }

        /// <summary>
        /// Sets the import type.
        /// </summary>
        /// <param name="importType">The import type.</param>
        public virtual void SetImportType(BaseNode.ImportType importType)
        {
            Debug.Assert(!IsTypeAssigned || ImportType == importType);

            _ImportType = importType;
            IsTypeAssigned = true;
        }

        /// <summary>
        /// Sets the import location.
        /// </summary>
        /// <param name="importLocation">The import location.</param>
        public virtual void SetImportLocation(IImport importLocation)
        {
            Debug.Assert(importLocation != null);
            Debug.Assert(!IsLocationAssigned || ImportLocation == importLocation);

            _ImportLocation = importLocation;
            IsLocationAssigned = true;
        }
        #endregion
    }
}
