using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.Options;

namespace Calculator.Analyzer
{
    public class ExpressionAnalyzer
    {
        private readonly IOptions<ExpressionAnalyzerOptions> _options;

        public ExpressionAnalyzer(IOptions<ExpressionAnalyzerOptions> options)
        {
            _options = options;
        }

        public double? GetResultFromExpression(string input)
        {
            var expressionParts = input.Split(' ');

            if (expressionParts.Length < 3)
                return default;

            var linkedList = new LinkedList<string>(expressionParts);

            return ApplyOperationAndGetResult(linkedList.First);
        }

        private double? GetValue(LinkedListNode<string> head)
        {
            if (_options.Value.AvailableOperations.ContainsKey(head.Value))
            {
                return ApplyOperationAndGetResult(head);
            }
            else if (double.TryParse(head.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
            {
                return result;
            }
            else throw new InvalidOperationException($"Input word '{head.Value}' is not operation or digit.");
        }

        private double? ApplyOperationAndGetResult(LinkedListNode<string> head)
        {
            var left = GetValue(head.Next);
            var right = GetValue(head.Next.Next);

            return head.Value switch
            {
                "add" => left + right,
                "sub" => left - right,
                "mul" => left * right,
                "div" => left / right,
                _ => throw new InvalidOperationException($"Cant find funtion with name '{head.Value}'."),
            };
        }
    }
}
