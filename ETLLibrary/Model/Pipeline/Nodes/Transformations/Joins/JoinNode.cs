using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using ETLBox.DataFlow.Transformations;

namespace ETLLibrary.Model.Pipeline.Nodes.Transformations.Joins
{
    public class JoinNode : TransformationNode
    {
        private JoinType _joinType;
        private string _firstTableKey;
        private string _secondTableKey;
        public MergeJoin<ExpandoObject, ExpandoObject, ExpandoObject> MergeJoin { get; set; }
        public Sort<ExpandoObject> leftSort { get; set; }
        public Sort<ExpandoObject> rightSort { get; set; }
        public JoinNode(string id, string name, JoinType joinType, string firstTableKey, string secondTableKey)
        {
            Id = id;
            Name = name;
            _joinType = joinType;
            _firstTableKey = firstTableKey;
            _secondTableKey = secondTableKey;
            CreateDataFlow();
        }

        private void CreateDataFlow()
        {
            CreateJoin();
            CreateComparison();
            CreateSortFunctions();
            DataFlow = null;
        }

        private void CreateSortFunctions()
        {
            Comparison<ExpandoObject> firstComparison = new Comparison<ExpandoObject>(
                (first, second) =>
                {
                    IDictionary<string, object> firstDictionary = first;
                    IDictionary<string, object> secondDictionary = second;
                    return firstDictionary[_firstTableKey].ToString()
                        .CompareTo(secondDictionary[_firstTableKey].ToString());
                });
            Comparison<ExpandoObject> secondComparison = new Comparison<ExpandoObject>(
                (first, second) =>
                {
                    IDictionary<string, object> firstDictionary = first;
                    IDictionary<string, object> secondDictionary = second;
                    return firstDictionary[_secondTableKey].ToString()
                        .CompareTo(secondDictionary[_secondTableKey].ToString());
                });
            leftSort = new Sort<ExpandoObject>(firstComparison);
            rightSort = new Sort<ExpandoObject>(secondComparison);
            leftSort.LinkTo(MergeJoin.LeftInput);
            rightSort.LinkTo(MergeJoin.RightInput);
        }

        private void CreateJoin()
        {
            MergeJoin = new MergeJoin<ExpandoObject, ExpandoObject, ExpandoObject>(
                (leftRow, rightRow) =>
                {
                    if (rightRow == null)
                        return null;
                    Dictionary<string, string> leftDictionary = leftRow.ToDictionary(k => k.Key,
                        v => v.Value != null ? v.Value.ToString() : (string) null);
                    Dictionary<string, string> rightDictionary = rightRow.ToDictionary(k => k.Key,
                        v => v.Value != null ? v.Value.ToString() : (string) null);
                    IDictionary<string, object> result = new ExpandoObject();
                    foreach (var key in rightDictionary.Keys)
                    {
                        result[key] = rightDictionary[key];
                    }

                    foreach (var key in leftDictionary.Keys)
                    {
                        result[key] = leftDictionary[key];
                    }

                    return (ExpandoObject) result;
                });
        }

        private void CreateComparison()
        {
            MergeJoin.ComparisonFunc = (firstRow, secondRow) =>
            {
                IDictionary<string, object> firstDictionary = firstRow;
                IDictionary<string, object> secondDictionary = secondRow;
                return firstDictionary[_firstTableKey].ToString()
                    .CompareTo(secondDictionary[_secondTableKey].ToString());
            };
        }
    }
}