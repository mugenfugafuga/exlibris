namespace Exlibris.Core;

public static class CallOnce
{
    private const int uncalled = 0;
    private const int called = 1;

    public static Action New(Action action)
    {
        var callstatus = uncalled;

        return () =>
        {
            if (Interlocked.CompareExchange(ref callstatus, called, uncalled) == uncalled)
            {
                action();
            }
        };
    }

    public static Action<T> New<T>(Action<T> action)
    {
        var callstatus = uncalled;

        return (T arg) =>
        {
            if (Interlocked.CompareExchange(ref callstatus, called, uncalled) == uncalled)
            {
                action(arg);
            }
        };
    }
}
