using System.Diagnostics.CodeAnalysis;

namespace Exlibris.Excel;

public interface IScalar
{
    ExcelAddress? Address { get; }

    object? Value { get; }

    bool IsNull { get; }

    bool TryGetValue<T>([MaybeNullWhen(false)] out T value);

    T? TryGetValue<T>() where T : class;

    T GetValueOrThrow<T>();

    T GetValueOrDefault<T>(T defaultValue);
}
