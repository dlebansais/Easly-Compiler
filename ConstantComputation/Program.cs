namespace ConstantComputation
{
    using BaseNode;
    using System;
    using System.Globalization;
    using System.Threading;

    class Program
    {
        static void Main()
        {
            CultureInfo enUS = CultureInfo.CreateSpecificCulture("en-US");
            CultureInfo.DefaultThreadCurrentCulture = enUS;
            CultureInfo.DefaultThreadCurrentUICulture = enUS;
            Thread.CurrentThread.CurrentCulture = enUS;
            Thread.CurrentThread.CurrentUICulture = enUS;

            string Result = SpecialMain.Main();
            Console.WriteLine(Result);
        }
    }
}
