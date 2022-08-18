
using System.Dynamic;
using ETLBox.DataFlow.Connectors;

namespace ETLLibrary.Model.Pipeline.Nodes.Sources.Csv
{
    public class CsvSource : SourceNode
    {
        private string _csvLocation;
        private string _colDelimiter;

        public CsvSource(string id, string name, string csvLocation , string colDelimiter)
        {
            Id = id;
            Name = name;
            _csvLocation = csvLocation;
            _colDelimiter = colDelimiter;
            CreateDataFlow();
        }

        private void CreateDataFlow()
        {
            CsvSource<ExpandoObject> csvSource = new CsvSource<ExpandoObject>(_csvLocation);
            csvSource.Configuration.Delimiter = _colDelimiter;
            DataFlow = csvSource;
        }
    }
}