namespace Test
{
    public interface ICoverageSingleClass3 : ICoverageSingleClass2
    {
    }

    public class CoverageSingleClass3 : CoverageSingleClass2
    {
        #region Init
        static CoverageSingleClass3()
        {
            _Singleton = new OnceReference<CoverageSingleClass3>();
        }

        public static CoverageSingleClass3 Singleton
        {
            get
            {
                if (!_Singleton.IsAssigned)
                    _Singleton.Item = new CoverageSingleClass3();

                return _Singleton.Item;
            }
        }
        private static OnceReference<CoverageSingleClass3> _Singleton;
        #endregion

        #region Client Interface
        #endregion
    }
}
