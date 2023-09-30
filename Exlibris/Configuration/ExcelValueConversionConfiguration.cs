namespace Exlibris.Configuration
{
    public class ExcelValueConversionConfiguration
    {
        public ObjectType ExcelErrorTo { get; set; } = ObjectType.ExcelError;

        public ObjectType ExcelMissingTo { get; set; } = ObjectType.Null;

        public ObjectType ExcelEmptyTo { get; set; } = ObjectType.Null;

        public ObjectType ExcelStringEmptyTo { get; set; } = ObjectType.Null;

        public ExcelValueType NullTo { get; set; } = ExcelValueType.StringEmpty;

        public ExcelValueType StringEmptyTo { get; set; } = ExcelValueType.StringEmpty;
    }
}
