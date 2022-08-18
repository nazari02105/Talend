using System;
using System.Dynamic;
using ETLBox.DataFlow;
using ETLLibrary.Model.Pipeline.Nodes.Destinations;

namespace ETLLibrary.Model.Pipeline.Nodes.Transformations
{
    public abstract class TransformationNode : Node
    {
        public IDataFlowTransformation<ExpandoObject,ExpandoObject> DataFlow;
        public Node Parent { get; set; }
        
    }
}