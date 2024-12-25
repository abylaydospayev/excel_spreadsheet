// <copyright file="Node.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SpreadsheetEngine
{

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Abstract base class representing a node in an expression tree.
    /// </summary>
    public abstract class Node
    {
        /// <summary>
        /// Evaluates the node using the provided variable values.
        /// </summary>
        /// <param name="variables">A dictionary containing variable names and their corresponding values.</param>
        /// <returns>The result of the evaluation as a double.</returns>
        public abstract double Evaluate(Dictionary<string, double> variables);
    }

    /// <summary>
    /// Represents a constant value in an expression tree.
    /// </summary>
    public class ConstantNode : Node
    {
        private double value;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstantNode"/> class with a specified value.
        /// </summary>
        /// <param name="value">The constant value.</param>
        public ConstantNode(double value)
        {
            this.value = value;
        }

        /// <summary>
        /// Evaluates the constant node, returning its value.
        /// </summary>
        /// <param name="variables">Ignored for constant nodes.</param>
        /// <returns>The constant value.</returns>
        public override double Evaluate(Dictionary<string, double> variables)
        {
            return value;
        }
    }

    /// <summary>
    /// Represents a variable in an expression tree.
    /// </summary>
    public class VariableNode : Node
    {
        private string name;

        // <summary>
        /// Initializes a new instance of the <see cref="VariableNode"/> class with a specified variable name.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        public VariableNode(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// Evaluates the variable node, returning its value from the provided dictionary.
        /// </summary>
        /// <param name="variables">A dictionary containing variable names and their corresponding values.</param>
        /// <returns>The value of the variable, or 0 if not found.</returns>
        public override double Evaluate(Dictionary<string, double> variables)
        {
            return variables.ContainsKey(this.name) ? variables[this.name] : 0;
        }
    }


    /// <summary>
    /// Represents an operator (e.g., +, -, *, /) in an expression tree.
    /// </summary>
    public class OperatorNode : Node
    {
        private Node left;
        private Node right;
        private Func<double, double, double> operation;

        /// <summary>
        /// Initializes a new instance of the <see cref="OperatorNode"/> class with specified operands and operation.
        /// </summary>
        /// <param name="left">The left operand as a Node.</param>
        /// <param name="right">The right operand as a Node.</param>
        /// <param name="operation">The operation to perform on the two operands.</param>
        public OperatorNode(Node left, Node right, Func<double, double, double> operation)
        {
            this.left = left;
            this.right = right;
            this.operation = operation;
        }

        /// <summary>
        /// Evaluates the operator node by evaluating its operands and applying the operation.
        /// </summary>
        /// <param name="variables">A dictionary containing variable names and their corresponding values.</param>
        /// <returns>The result of applying the operation to the evaluated operands.</returns>
        public override double Evaluate(Dictionary<string, double> variables)
        {
            return this.operation(this.left.Evaluate(variables), right.Evaluate(variables));
        }
    }

}
