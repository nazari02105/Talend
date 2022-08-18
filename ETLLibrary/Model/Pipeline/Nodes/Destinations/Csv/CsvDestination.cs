using System.Dynamic;
using ETLBox.DataFlow.Connectors;

namespace ETLLibrary.Model.Pipeline.Nodes.Destinations.Csv
{
    public class CsvDestination : DestinationNode
    {
        private string _fileLocation;
        private string _colDelimiter;
        public CsvDestination(string id, string name, string fileLocation , string colDelimiter)
        {
            Id = id;
            Name = name;
            _fileLocation = fileLocation;
            _colDelimiter = colDelimiter;
            CreateDataFlow();
        }

        private void CreateDataFlow()
        {
            CsvDestination<ExpandoObject> csvDestination = new CsvDestination<ExpandoObject>(_fileLocation);
            csvDestination.Configuration.Delimiter = _colDelimiter;
            DataFlow = csvDestination;
        }
    }
}