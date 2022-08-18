namespace ETLLibrary.Database.Utils
{
    public class CsvInfo
    {
        public string Name { get; set; }
        public string ColDelimiter { get; set; }
        public string RowDelimiter { get; set; }
        public bool HasHeader { get; set; }
    }
}