namespace Test
{
    public interface ICoverageSingleClass2 : ICoverageSingleClass
    {
    }

    public class CoverageSingleClass2 : CoverageSingleClass
    {
        #region Init
        static CoverageSingleClass2()
        {
            _Singleton = new OnceReference<CoverageSingleClass2>();
        }

        public static CoverageSingleClass2 Singleton
        {
            get
            {
                if (!_Singleton.IsAssigned)
                    _Singleton.Item = new CoverageSingleClass2();

                return _Singleton.Item;
            }
        }
        private static OnceReference<CoverageSingleClass2> _Singleton;
        #endregion
    }
}
