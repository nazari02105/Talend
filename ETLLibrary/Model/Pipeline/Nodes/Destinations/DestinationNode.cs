using System.Dynamic;
using ETLBox.DataFlow;

namespace ETLLibrary.Model.Pipeline.Nodes.Destinations
{
    public abstract class DestinationNode : Node
    {
        public Node Parent { get; set; }
        public IDataFlowDestination<ExpandoObject> DataFlow;
    }
}