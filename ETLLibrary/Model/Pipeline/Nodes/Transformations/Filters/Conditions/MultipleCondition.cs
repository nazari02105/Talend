using System;
using System.Collections.Generic;
using ETLLibrary.Model.Pipeline.Nodes.Transformations.Filters.Enums;

namespace ETLLibrary.Model.Pipeline.Nodes.Transformations.Filters.Conditions
{
    public class MultipleCondition : Condition
    {
        private Condition _leftCondition;
        private Condition _rightCondition;
        private LogicOperator _logicOperator;

        public MultipleCondition(Condition leftCondition, Condition rightCondition, LogicOperator logicOperator)
        {
            _leftCondition = leftCondition;
            _rightCondition = rightCondition;
            _logicOperator = logicOperator;
        }
        public override bool Evaluate(IDictionary<string, object> dictionary)
        {
            bool leftResult = _leftCondition.Evaluate(dictionary);
            bool rightResult = _rightCondition.Evaluate(dictionary);
            if (_logicOperator == LogicOperator.And)
                return leftResult & rightResult;
            else if (_logicOperator == LogicOperator.Or)
                return leftResult | rightResult;
            throw new NotImplementedException("logic operator not supported");
        }
    }
}