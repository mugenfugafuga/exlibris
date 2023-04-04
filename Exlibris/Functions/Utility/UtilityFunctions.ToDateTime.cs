using ExcelDna.Integration;
using Exlibris.Excel;

namespace Exlibris.Functions.Utility;

partial class UtilityFunctions
{
    [ExcelFunction(
        Category = Category,
        Name = $"{Category}.{nameof(ToDateTime)}",
        Description = "treating the specified cells as DateTime type.")]
    public static object ToDateTime(
        [ExcelArgument(AllowReference = true, Description = "optional argument. target cells")] object cells1,
        [ExcelArgument(AllowReference = true, Description = "optional argument. target cells")] object cells2,
        [ExcelArgument(AllowReference = true, Description = "optional argument. target cells")] object cells3,
        [ExcelArgument(AllowReference = true, Description = "optional argument. target cells")] object cells4,
        [ExcelArgument(AllowReference = true, Description = "optional argument. target cells")] object cells5)
        => ExlibrisAddin.GetFunctionSupport().ErrorValueIfThrown(support =>
        {
            var aOrDtds = GetAddressOrDateTimeDetectors(
                support,
                ExlibrisUtil.GetRawExcel(cells1, nameof(cells1)),
                ExlibrisUtil.GetRawExcel(cells2, nameof(cells2)),
                ExlibrisUtil.GetRawExcel(cells3, nameof(cells3)),
                ExlibrisUtil.GetRawExcel(cells4, nameof(cells4)),
                ExlibrisUtil.GetRawExcel(cells5, nameof(cells5)));

            return support.ObserveObjectRegistration(() =>
            {
                return aOrDtds.Aggregate(new DateTimeDetector(), (dtd, aOrDtd) => dtd.Add(aOrDtd));
            }, cells1, cells2, cells3, cells4, cells5);
        });

    public static IEnumerable<object?> GetAddressOrDateTimeDetectors(ExlibrisExcelFunctionSupport support, params RawExcel[] cells)
    {
        foreach (var cell in cells)
        {
            if (cell.Value is string st && support.ObjectRegistry.TryGetObject<DateTimeDetector>(st, out var dt))
            {
                yield return dt;
            }
            else
            {
                yield return cell.Address;
            }
        }
    }
}
