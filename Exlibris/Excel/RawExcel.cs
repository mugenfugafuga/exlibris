namespace Exlibris.Excel
{
    public readonly struct RawExcel
    {
        public ExcelAddress? Address { get; }
        public object Value { get; }
        public string Name { get; }

        public RawExcel(ExcelAddress? address, object value, string name)
        {
            Address = address;
            Value = value;
            Name = name;
        }
    }
}