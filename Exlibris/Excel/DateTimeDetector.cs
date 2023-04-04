namespace Exlibris.Excel;

public class DateTimeDetector
{
    private readonly List<ExcelAddress> addresses = new();
    private readonly List<DateTimeDetector> detectors = new();

    public DateTimeDetector Add(object? obj)
    {
        if (obj != null)
        {
            if (obj is ExcelAddress a) { addresses.Add(a); }
            if (obj is DateTimeDetector dt) { detectors.Add(dt); }
        }

        return this;
    }

    public bool IsDateTime(ExcelAddress? address)
    {
        if (address == null) { return false; }

        return
            addresses.Any(a => a.Contains(address)) ||
            detectors.Any(d => d.IsDateTime(address));
    }
}
