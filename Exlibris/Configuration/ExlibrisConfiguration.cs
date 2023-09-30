using Exlibris.Core.DI;

namespace Exlibris.Configuration
{
    public class ExlibrisConfiguration
    {
        public ExcelValueConversionConfiguration ExcelValueConversion { get; set; } = new ExcelValueConversionConfiguration();

        public DIConfiguration DIConfiguration { get; set; } = new DIConfiguration();
    }
}
