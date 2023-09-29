using System;

namespace Exlibris.Functions.Utility
{
    public static partial class UtilityFunctions
    {
        private const string Category = "Exlibris.Utility";

        public enum DisplayTimeType
        {
            Double = 0,
            String = 1,
        }

        private static Func<object> GetCurrentTimeFunc(DisplayTimeType timeType)
        {
            switch (timeType)
            {
                case DisplayTimeType.String: return () => DateTime.Now.ToString();
                default: return () => DateTime.Now;
            }
        }
    }
}