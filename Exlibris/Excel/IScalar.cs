using System.Diagnostics.CodeAnalysis;

namespace Exlibris.Excel;

public interface IScalar
{
    ExcelAddress? Address { get; }

    object? Value { get; }

    bool TryGetValue<T>([MaybeNullWhen(false)] out T value);

    T GetValueOrThrow<T>();

    T GetValueOrDefault<T>(T defaultValue);
}
