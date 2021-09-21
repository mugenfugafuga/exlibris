using ExcelDna.Integration;

namespace ExLibris.Core
{
    interface IObjectRegistrationHandle : IExcelObservable
    {
        string HandleKey { get; }
    }
}
