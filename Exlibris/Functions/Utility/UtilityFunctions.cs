namespace Exlibris.Functions.Utility;

public static partial class UtilityFunctions
{
    private const string Category = $"{nameof(Exlibris)}.{nameof(Utility)}";

    public enum DisplayTimeType
    {
        Double = 0,
        String = 1,
    }

    private static Func<object> GetCurrentTimeFunc(DisplayTimeType timeType)
    => timeType switch
    {
        DisplayTimeType.String => () => DateTime.Now.ToString(),
        _ => () => DateTime.Now,
    };
}
