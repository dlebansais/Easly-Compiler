namespace Test
{
    public interface ICoverageSingleClass
    {
    }

    public class CoverageSingleClass
    {
        #region Init
        static CoverageSingleClass()
        {
            _Singleton = new OnceReference<CoverageSingleClass>();
        }

        public static CoverageSingleClass Singleton
        {
            get
            {
                if (!_Singleton.IsAssigned)
                    _Singleton.Item = new CoverageSingleClass();

                return _Singleton.Item;
            }
        }
        private static OnceReference<CoverageSingleClass> _Singleton;
        #endregion
    }
}
