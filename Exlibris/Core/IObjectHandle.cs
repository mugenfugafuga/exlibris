namespace Exlibris.Core;

public interface IObjectHandle : IDisposable
{
    string Key { get; }

    object Value { get; }

    object? Misc { get; }

    T? GetValue<T>();

    T ValueOrThrow<T>();
}
