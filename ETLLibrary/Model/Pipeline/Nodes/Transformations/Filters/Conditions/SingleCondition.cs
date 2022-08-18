using System;
using System.Collections.Generic;
using ETLLibrary.Model.Pipeline.Nodes.Transformations.Filters.Enums;
using Type = ETLLibrary.Model.Pipeline.Nodes.Transformations.Filters.Enums.Type;

namespace ETLLibrary.Model.Pipeline.Nodes.Transformations.Filters.Conditions
{
    public class SingleCondition : Condition
    {
        private string _columnName;
        private Operator _operator;
        private string _desiredValue;
        private Type _type;

        public SingleCondition(string columnName, Operator conditionOperator, string desiredValue, Type type)
        {
            _columnName = columnName;
            _operator = conditionOperator;
            _desiredValue = desiredValue;
            _type = type;
        }
        
        public override bool Evaluate(IDictionary<string , object> dictionary)
        {
            if (_operator == Operator.Equals)
                return dictionary[_columnName].ToString() == _desiredValue;
            else if (_operator == Operator.GreaterThan)
                return IsGreaterThan(dictionary);
            else if (_operator == Operator.LessThan)
                return IsLessThan(dictionary);
            throw new NotImplementedException("operator not supported");
        }

        private bool IsGreaterThan(IDictionary<string, object> dictionary)
        {
            if (_type == Type.String)
                throw new Exception("greater than is not defined for strings");
            else if (_type == Type.Integer)
                return Int32.Parse(dictionary[_columnName].ToString() ?? string.Empty) > Int32.Parse(_desiredValue);
            else if(_type == Type.Double)
                return Double.Parse(dictionary[_columnName].ToString() ?? string.Empty) > Double.Parse(_desiredValue);
            throw new NotImplementedException("type not supported");
        }
        private bool IsLessThan(IDictionary<string, object> dictionary)
        {
            if (_type == Type.String)
                throw new Exception("less than is not defined for strings");
            else if (_type == Type.Integer)
                return Int32.Parse(dictionary[_columnName].ToString() ?? string.Empty) < Int32.Parse(_desiredValue);
            else if(_type == Type.Double)
                return Double.Parse(dictionary[_columnName].ToString() ?? string.Empty) < Double.Parse(_desiredValue);
            throw new NotImplementedException("type not supported");
        }
    }
}