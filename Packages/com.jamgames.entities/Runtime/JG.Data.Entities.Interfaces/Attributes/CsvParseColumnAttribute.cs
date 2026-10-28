using System;

namespace JG.Data.Entities.Interfaces.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CsvParseColumnAttribute : Attribute
    {
        public string CsvFileName;
        public string ColumnTitle;

        public CsvParseColumnAttribute(string csvFileName, string columnTitle)
        {
            CsvFileName = csvFileName;
            ColumnTitle = columnTitle;
        }
    }
}
