// <copyright file="ExpressionTree.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SpreadsheetEngine
{

    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a mathematical expression as a tree structure.
    /// </summary>
    public class ExpressionTree
    {
        /// <summary>
        /// The root node of the expression tree.
        /// </summary>
        private readonly Node root;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionTree"/> class.
        /// </summary>
        /// <param name="expression">The mathematical expression as a string.</param>
        public ExpressionTree(string expression)
        {
            var factory = new ExpressionFactory();
            this.root = factory.CreateExpressionTree(expression);
        }

        /// <summary>
        /// Evaluates the expression tree with the given variable values.
        /// </summary>
        /// <param name="variables">A dictionary of variable names and their values.</param>
        /// <returns>The result of evaluating the expression.</returns>
        public double Evaluate(Dictionary<string, double> variables)
        {
            return this.root.Evaluate(variables);
        }
    }
}