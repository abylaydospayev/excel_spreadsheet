using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework; // Make sure to include NUnit for the test attributes
using SpreadsheetEngine;

namespace Test_unit
{
    /// <summary>
    /// Unit tests for the <see cref="ExpressionTree"/> class.
    /// </summary>
    public class ExpressionTree_test
    {
        private ExpressionTree expressionTree;

        /// <summary>
        /// Generates an array of variable names based on the specified count.
        /// </summary>
        /// <param name="count">The number of variable names to generate.</param>
        /// <returns>An array of variable names.</returns>
        private static string[] GenerateVariableNames(int count)
        {
            return Enumerable.Range(1, count).Select(i => "V" + i).ToArray();
        }

        /// <summary>
        /// Generates a dictionary of variables with their corresponding default values.
        /// </summary>
        /// <param name="count">The number of variables to generate.</param>
        /// <returns>A dictionary mapping variable names to their default values.</returns>
        private static Dictionary<string, double> GenerateVariables(int count)
        {
            return GenerateVariableNames(count).ToDictionary(name => name, name => (double)name.Length);
        }

        /// <summary>
        /// Tests addition expressions with variables and constants.
        /// </summary>
        [Test]
        public void TestAdditionExpression()
        {
            expressionTree = new ExpressionTree("A+B+C1+6");
            var variables = new Dictionary<string, double>
            {
                { "A", 1 },
                { "B", 2 },
                { "C1", 3 }
            };

            double result = expressionTree.Evaluate(variables);
            Assert.AreEqual(12, result);
        }

        /// <summary>
        /// Tests subtraction expressions with variables and constants.
        /// </summary>
        [Test]
        public void TestSubtractionExpression()
        {
            expressionTree = new ExpressionTree("A-B-C1-6");
            var variables = new Dictionary<string, double>
            {
                { "A", 10 },
                { "B", 5 },
                { "C1", 2 }
            };

            double result = expressionTree.Evaluate(variables);
            Assert.AreEqual(-3, result);
        }

        /// <summary>
        /// Tests multiplication expressions with variables and constants.
        /// </summary>
        [Test]
        public void TestMultiplicationExpression()
        {
            expressionTree = new ExpressionTree("A*B*C1*2");
            var variables = new Dictionary<string, double>
            {
                { "A", 2 },
                { "B", 3 },
                { "C1", 4 }
            };

            double result = expressionTree.Evaluate(variables);
            Assert.AreEqual(48, result);
        }

        /// <summary>
        /// Tests division expressions with variables and constants.
        /// </summary>
        [Test]
        public void TestDivisionExpression()
        {
            expressionTree = new ExpressionTree("A/B/C1/2");
            var variables = new Dictionary<string, double>
            {
                { "A", 16 },
                { "B", 4 },
                { "C1", 2 }
            };

            double result = expressionTree.Evaluate(variables);
            Assert.AreEqual(1, result);
        }

        /// <summary>
        /// Tests that undefined variables default to zero when evaluating an expression.
        /// </summary>
        [Test]
        public void TestUndefinedVariableDefaultsToZero()
        {
            expressionTree = new ExpressionTree("A+B+C");
            var variables = new Dictionary<string, double>
            {
                { "A", 5 },
                { "B", 10 }
                // C is not defined
            };

            double result = expressionTree.Evaluate(variables);
            Assert.AreEqual(15, result); // C defaults to 0
        }

        /// <summary>
        /// Tests a complex expression involving parentheses and multiple operations.
        /// </summary>
        [Test]
        public void TestComplexExpression()
        {
            expressionTree = new ExpressionTree("(a + b) / c");
            var variables = new Dictionary<string, double>
            {
                { "a", 3 },
                { "b", 3 },
                { "c", 2 }
            };

            double result = expressionTree.Evaluate(variables);
            Assert.AreEqual(3, result); // Expecting (3 + 3) / 2 = 3
        }

        /// <summary>
        /// Tests operator precedence with parentheses in an expression.
        /// </summary>
        [Test]
        public void TestParenthesesPrecedence()
        {
            expressionTree = new ExpressionTree("A + B * C");
            var variables = new Dictionary<string, double>
           {
               { "A", 2 },
               { "B", 3 },
               { "C", 4 }
           };

            double result = expressionTree.Evaluate(variables);
            Assert.AreEqual(14, result); // Expecting 2 + (3 * 4) = 14
        }

        /// <summary>
        /// Evaluates a simple expression with multiple variables and checks the result.
        /// </summary>
        [Test]
        public void Evaluate_Variables_ReturnsCorrectResult()
        {
            var tree = new ExpressionTree("A + B * C");
            var variables = new Dictionary<string, double>
           {
               { "A", 1 },
               { "B", 2 },
               { "C", 3 }
           };

            var result = tree.Evaluate(variables);
            Assert.AreEqual(1 + (2 * 3), result); // Expecting 7
        }

        /// <summary>
        /// Evaluates an expression with undefined variables and checks that they default to zero.
        /// </summary>
        [Test]
        public void Evaluate_VariableWithDefaultValue()
        {
            var tree = new ExpressionTree("X + Y");
            var variables = new Dictionary<string, double>(); // X and Y not defined

            var result = tree.Evaluate(variables);
            Assert.AreEqual(0, result); // Both X and Y default to 0
        }

        /// <summary>
        /// Evaluates a complex expression with multiple variables and checks the result.
        /// </summary>
        [Test]
        public void Evaluate_ComplexExpressionWithVariables_ReturnsCorrectResult()
        {
            var tree = new ExpressionTree("A1 + B2 * (C3 - D4)");
            var variables = new Dictionary<string, double>
           {
               { "A1", 5 },
               { "B2", 2 },
               { "C3", 10 },
               { "D4", 3 }
           };

            var result = tree.Evaluate(variables);
            Assert.AreEqual(5 + (2 * (10 - 3)), result); // Expecting 19
        }


        /// <summary>
        /// Evaluates an invalid mathematical expression and expects an exception to be thrown.
        /// </summary>
        [Test]
        public void Evaluate_InvalidExpression_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() =>
                new ExpressionTree("3 +")); // Incomplete expression
        }

        /// <summary>
        /// Evaluates an expression missing a closing parenthesis and expects an exception to be thrown.
        /// </summary>
        [Test]
        public void Evaluate_MissingClosingParenthesis_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() =>
                new ExpressionTree("(3 + 5")); // Missing closing parenthesis
        }
    }
}