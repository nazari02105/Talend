using ETLBox.DataFlow.Transformations;
using ETLLibrary.Model.Pipeline.Nodes.Transformations.Filters.Conditions;

namespace ETLLibrary.Model.Pipeline.Nodes.Transformations.Filters
{
    public class FilterNode : TransformationNode
    {
        private Condition _condition;

        public FilterNode(string id , string name , Condition condition)
        {
            Id = id;
            Name = name;
            _condition = condition;
            CreateFilterTransformation();
        }

        private void CreateFilterTransformation()
        {
            var filterTransformation = new FilterTransformation();
            filterTransformation.FilterPredicate = row =>
            {
                return ! _condition.Evaluate(row);
            };
            DataFlow = filterTransformation;
        }
        
    }
}