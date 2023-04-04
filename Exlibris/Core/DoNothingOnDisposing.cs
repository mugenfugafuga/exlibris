namespace Exlibris.Core;

public class DoNothingOnDisposing : IDisposable
{
    public static readonly IDisposable Instance = new DoNothingOnDisposing();

    public void Dispose() { }
}
