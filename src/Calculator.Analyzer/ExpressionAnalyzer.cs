using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;

namespace Calculator.Analyzer
{
    public class ExpressionAnalyzer
    {
        private readonly IOptions<ExpressionAnalyzerOptions> _options;
        private LinkedListNode<string> _expressionHead;

        public ExpressionAnalyzer(IOptions<ExpressionAnalyzerOptions> options)
        {
            _options = options;
        }

        public double? GetResultFromExpression(string input)
        {
            var regex = new Regex(@"\s");
            var expressionParts = regex.Split(input.ToLower());

            if (expressionParts.Length < 3)
                return default;

            var linkedList = new LinkedList<string>(expressionParts);

            var head = linkedList.First;

            if (!_options.Value.AvailableOperations.Contains(head.Value))
                throw new InvalidOperationException($"Cant find funtion with name '{head}'.");

            _expressionHead = head;

            var expression = ApplyOperationAndGetResult();

            return Expression.Lambda<Func<double>>(expression).Compile()();
        }

        private Expression GetValue()
        {
            _expressionHead = _expressionHead.Next;

            if (_options.Value.AvailableOperations.Contains(_expressionHead.Value))
            {
                return ApplyOperationAndGetResult();
            }
            else if (double.TryParse(_expressionHead.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
            {
                return Expression.Constant(result);
            }
            else throw new InvalidOperationException($"Input word '{_expressionHead.Value}' is not operation or digit.");
        }

        private BinaryExpression ApplyOperationAndGetResult()
        {
            var operation = _expressionHead.Value;

            var left = GetValue();
            var right = GetValue();

            return operation switch
            {
                "add" => Expression.Add(left, right),
                "sub" => Expression.Subtract(left, right),
                "mul" => Expression.Multiply(left, right),
                "div" => Expression.Divide(left, right),
                _ => throw new InvalidOperationException($"Cant find funtion with name '{_expressionHead.Value}'."),
            };
        }
    }
}
