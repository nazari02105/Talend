using System.Collections.Generic;

namespace ETLLibrary.Model.Pipeline.Nodes.Transformations.Filters.Conditions
{
    public abstract class Condition
    {
        public abstract bool Evaluate(IDictionary<string , object> dictionary);
    }
}