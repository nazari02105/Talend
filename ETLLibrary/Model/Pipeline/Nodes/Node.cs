
using System.Dynamic;
using ETLBox.DataFlow;

namespace ETLLibrary.Model.Pipeline.Nodes
{
    public abstract class Node
    {
        public string Id { get; set; }
        protected string Name;
    }
}