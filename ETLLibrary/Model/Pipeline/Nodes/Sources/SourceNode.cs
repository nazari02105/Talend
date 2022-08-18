using System;
using System.Dynamic;
using ETLBox.DataFlow;
using ETLLibrary.Model.Pipeline.Nodes.Destinations;
using ETLLibrary.Model.Pipeline.Nodes.Transformations;

namespace ETLLibrary.Model.Pipeline.Nodes.Sources
{
    public abstract class SourceNode : Node
    {
        public DataFlowSource<ExpandoObject> DataFlow { get; set; }
        
    }
}