using System.Collections.Concurrent;

namespace Exlibris.Excel;
public class ThrownExceptions
{
    private readonly ConcurrentDictionary<ExcelAddress, Exception> exceptions = new();

    public void Remove(ExcelAddress address)
        => exceptions.TryRemove(address, out _);

    public Exception? this[ExcelAddress address]
    {
        get => exceptions.TryGetValue(address, out var exception) ? exception : null;
        set
        {
            if (value != null)
            {
                exceptions[address] = value;
            }
        }
    }

    public ExceptionOfReference ExceptionOf(ExcelAddress address)
    => new(this, address, true);

    public ExceptionOfReference ExceptionOf(ExcelAddress address, bool removeLastException)
        => new(this, address, removeLastException);
       
    public class ExceptionOfReference
    {
        private readonly ThrownExceptions exceptions;
        private readonly ExcelAddress address;

        public ExceptionOfReference(ThrownExceptions exceptions, ExcelAddress address, bool removeLastException)
        {
            this.exceptions = exceptions;
            this.address = address;

            if (removeLastException)
            {
                exceptions.Remove(address);
            }
        }

        public Exception Is
        {
            set => exceptions[address] = value;
        }
    }
}