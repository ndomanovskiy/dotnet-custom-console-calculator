using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace Calculator.Analyzer.Tests
{
    public class BasicTests
    {
        [TestCase("add 3")]
        public void CalculateResultFromIncorrectInputString(string input)
        {
            var options = Options.Create(new ExpressionAnalyzerOptions());

            var analyzer = new ExpressionAnalyzer(options);

            var result = analyzer.GetResultFromExpression(input);

            Assert.AreEqual(result, default);
        }

        [TestCase("add 3 4", 7)]
        [TestCase("add 3.234 4.8872", 8.1212)]
        [TestCase("mul 2 add 3 4", 14)]
        [TestCase("mul 2 add 3.5 4", 15)]
        [TestCase("mul add 3 4 2", 14)]
        [TestCase("mul div 14 2 7", 49)]
        [TestCase("add 4 mul div 14 2 7", 53)]
        public void CalculateResultFromCorrectInputString(string input, double expectedResult)
        {
            var options = Options.Create(new ExpressionAnalyzerOptions()
            {
                AvailableOperations = new string[] { "add", "sub", "mul", "div" }
            });

            var analyzer = new ExpressionAnalyzer(options);

            var result = analyzer.GetResultFromExpression(input);

            Assert.AreEqual(result, expectedResult);
        }
    }
}
