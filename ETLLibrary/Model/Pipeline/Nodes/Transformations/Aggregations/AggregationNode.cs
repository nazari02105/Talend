using System;
using System.Collections.Generic;
using ETLBox.DataFlow;
using ETLBox.DataFlow.Transformations;

namespace ETLLibrary.Model.Pipeline.Nodes.Transformations.Aggregations
{
    public class AggregationNode : TransformationNode
    {
        private AggregationType _aggregationType;
        private string _aggregateColumnName;
        private string _newColumnName;
        private List<string> _groupByColumnNames;

        private AggregationMethod GetAggregationMethodByType()
        {
            
            if (_aggregationType == AggregationType.Sum)
                return AggregationMethod.Sum;
            else if (_aggregationType == AggregationType.Count)
                return AggregationMethod.Count;
            else if (_aggregationType == AggregationType.Average)
            {
                
                throw new NotImplementedException(
                    "ETLBox doesn't contain average as predefined function so we need to implement it manually later");
            }
            else if (_aggregationType == AggregationType.Max)
                return AggregationMethod.Max;
            else if (_aggregationType == AggregationType.Min)
                return AggregationMethod.Min;
            throw new NotImplementedException("aggregation not supported");
        }

        public AggregationNode(string id , string name , AggregationType aggregationType, string aggregateColumnName, string newColumnName , 
            List<string> groupByColumnNames)
        {
            Id = id;
            Name = name;
            _aggregationType = aggregationType;
            _aggregateColumnName = aggregateColumnName;
            _newColumnName = newColumnName;
            _groupByColumnNames = groupByColumnNames;
            CreateAggregationsAndGroups();
        }

        private void CreateAggregationsAndGroups()
        {
            var aggregation = new Aggregation
                {AggregateColumns = new List<AggregateColumn>(), GroupColumns = new List<GroupColumn>()};
            CreateAggregations(aggregation);
            CreateGroups(aggregation);
            DataFlow = aggregation;
        }

        private void CreateGroups(Aggregation aggregation)
        {
            foreach (var groupName in _groupByColumnNames)
            {
                var groupColumn = new GroupColumn
                {
                    GroupPropNameInInput = groupName, GroupPropNameInOutput = groupName
                };
                aggregation.GroupColumns.Add(groupColumn);
            }
        }

        private void CreateAggregations(Aggregation aggregation)
        {

            var aggregateColumn = new AggregateColumn
            {
                InputValuePropName = _aggregateColumnName,
                AggregatedValuePropName = _newColumnName,
                AggregationMethod = GetAggregationMethodByType()
            };
            aggregation.AggregateColumns.Add(aggregateColumn);
        }
    }
}