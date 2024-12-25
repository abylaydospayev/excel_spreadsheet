/// <summary>
/// Factory class responsible for creating an expression tree from a string representation.
/// </summary>
namespace SpreadsheetEngine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ExpressionFactory
    {
        /// <summary>
        /// Creates an expression tree from a given mathematical expression string.
        /// </summary>
        /// <param name="expression">A string representing a mathematical expression to be parsed.</param>
        /// <returns>The root node of the constructed expression tree.</returns>
        public Node CreateExpressionTree(string expression)
        {
            return ParseExpression(ref expression);
        }

        /// <summary>
        /// Parses the given expression and constructs the expression tree.
        /// </summary>
        /// <param name="expression">A reference to the string representing a mathematical expression.</param>
        /// <returns>The root node of the constructed expression tree.</returns>
        private Node ParseExpression(ref string expression)
        {
            // Remove whitespace
            expression = expression.Replace(" ", "");

            // Handle parentheses and operator precedence
            return ParseTerm(ref expression);
        }

        /// <summary>
        /// Parses terms in the expression, handling addition and subtraction.
        /// </summary>
        /// <param name="expression">A reference to the string representing a mathematical expression.</param>
        /// <returns>The leftmost node of the parsed terms.</returns>
        private Node ParseTerm(ref string expression)
        {
            Node left = this.ParseFactor(ref expression);

            while (expression.Length > 0 && (expression[0] == '+' || expression[0] == '-'))
            {
                char op = expression[0];
                expression = expression.Substring(1); // Remove operator
                Node right = this.ParseFactor(ref expression);
                left = new OperatorNode(left, right, this.GetOperation(op));
            }

            return left;
        }

        /// <summary>
        /// Parses factors in the expression, handling multiplication and division.
        /// </summary>
        /// <param name="expression">A reference to the string representing a mathematical expression.</param>
        /// <returns>The leftmost node of the parsed factors.</returns>
        private Node ParseFactor(ref string expression)
        {
            Node left = this.ParsePrimary(ref expression);

            while (expression.Length > 0 && (expression[0] == '*' || expression[0] == '/'))
            {
                char op = expression[0];
                expression = expression.Substring(1); // Remove operator
                Node right = this.ParsePrimary(ref expression);
                left = new OperatorNode(left, right, this.GetOperation(op));
            }

            return left;
        }

        /// <summary>
        /// Parses primary elements of the expression, including numbers, variables, and parentheses.
        /// </summary>
        /// <param name="expression">A reference to the string representing a mathematical expression.</param>
        /// <returns>A node representing a number, variable, or sub-expression.</returns>
        private Node ParsePrimary(ref string expression)
        {
            if (expression.Length == 0)
            {
                throw new ArgumentException("Unexpected end of expression.");
            }

            if (expression[0] == '(')
            {
                // Handle parentheses
                expression = expression.Substring(1); // Remove '('
                Node node = this.ParseTerm(ref expression);

                if (expression.Length == 0 || expression[0] != ')')
                {
                    throw new ArgumentException("Missing closing parenthesis.");
                }

                expression = expression.Substring(1); // Remove ')'
                return node;
            }

            // Handle numbers and variables
            int index = 0;
            while (index < expression.Length &&
                   (char.IsDigit(expression[index]) ||
                    char.IsLetter(expression[index]) ||
                    expression[index] == '.'))
            {
                index++;
            }

            string part = expression.Substring(0, index);
            expression = expression.Substring(index); // Remove parsed part

            return this.CreateNode(part);
        }

        /// <summary>
        /// Creates a node from a given part of an expression, which can be a constant or a variable.
        /// </summary>
        /// <param name="part">A string representing a part of an expression (constant or variable).</param>
        /// <returns>A Node representing either a ConstantNode or VariableNode.</returns>
        private Node CreateNode(string part)
        {
            if (double.TryParse(part.Trim(), out double value))
            {
                return new ConstantNode(value);
            }

            var trimmedPart = part.Trim();
            if (this.IsVariable(trimmedPart))
            {
                return new VariableNode(trimmedPart);
            }

            throw new ArgumentException("Invalid expression part: " + part);
        }

        /// <summary>
        /// Checks if a given part is a valid variable name according to defined rules.
        /// </summary>
        /// <param name="part">A string representing a potential variable name.</param>
        /// <returns>True if it is a valid variable name; otherwise, false.</returns>
        private bool IsVariable(string part)
        {
            return char.IsLetter(part[0]) && part.All(c => char.IsLetterOrDigit(c));
        }

        /// <summary>
        /// Maps an operator character to its corresponding operation function.
        /// </summary>
        /// <param name="operatorChar">A character representing an arithmetic operator (+, -, *, /).</param>
        /// <returns>A function that performs the corresponding arithmetic operation on two double values.</returns>
        private Func<double, double, double> GetOperation(char operatorChar)
        {
            switch (operatorChar)
            {
                case '+': return (a, b) => a + b;
                case '-': return (a, b) => a - b;
                case '*': return (a, b) => a * b;
                case '/': return (a, b) => a / b;
                default: throw new ArgumentException("Invalid operator: " + operatorChar);
            }
        }
    }
}
