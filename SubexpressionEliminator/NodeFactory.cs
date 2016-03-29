/*	Copyright (C) 2015 Sean Lynch

    This file is part of Math-CSE.

    Foobar is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Foobar is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Math-CSE.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubexpressionEliminator
{
	[Serializable]
	public class ExpressionTraversalException : Exception
	{
		public ExpressionTraversalException() { }
		public ExpressionTraversalException(string message) : base(message) { }
		public ExpressionTraversalException(string message, Exception inner) : base(message, inner) { }
		protected ExpressionTraversalException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context)
		{ }
	}

	class NodeFactory
	{
		/// <summary>
		/// Creates an expression node tree from a string.
		/// </summary>
		/// <param name="text">The string to parse into an AST.</param>
		/// <returns></returns>
		public static IExpressionNode ParseExpressionTree(string text)
		{
			Parser parser = new Parser(text);
			return CreateExpressionTree(parser);
		}
		/// <summary>
		/// Flattens all the nodes in a tree into a collection.
		/// </summary>
		/// <param name="tree"></param>
		/// <returns></returns>
		static public IEnumerable<IExpressionNode> FlattenTree(IExpressionNode tree)
		{
			List<IExpressionNode> nodes = new List<IExpressionNode>();
			nodes.Add(tree);

			foreach (IExpressionNode node in tree.Children)
			{
				nodes.AddRange(FlattenTree(node));
			}

			return nodes;
		}
		/// <summary>
		/// Flattens all the nodes in a list of trees into a collection.
		/// </summary>
		/// <param name="nodes"></param>
		/// <returns></returns>
		static public List<IExpressionNode> FlattenAll(IEnumerable<IExpressionNode> nodes)
		{
			List<IExpressionNode> start = new List<IExpressionNode>(nodes);

			foreach (var node in nodes)
			{
				start.AddRange(FlattenTree(node));
			}

			return start;
		}

		public static IExpressionNode CreateExpressionTree(Parser parser)
		{
			return CreateExpressionTree(parser, true);
		}
		private static IExpressionNode CreateExpressionTree(Parser parser, bool cond)
		{
			IExpressionNode ret = null;

			switch (parser.Current().Value)
			{
				case Parser.EOF:
					return new Nodes.EmptyNode();
				case Parser.Literal:
					{
						Token t = parser.Current();
						parser.Consume();
						ret = new Nodes.LiteralNode(t);
						break;
					}
				case Parser.Identifier:
					if (parser.Next(1).Value == Parser.LParen)
					{
						string name = parser.Current().Text;
						parser.Consume();
						parser.Consume();
						List<IExpressionNode> nodes = new List<IExpressionNode>();
						if (parser.Current().Value != Parser.RParen)
						{
							int value;
							do
							{
								IExpressionNode node = CreateExpressionTree(parser, true);
								value = parser.Current().Value;
								parser.Consume();
								nodes.Add(node);
							} while (value == Parser.Comma);

							if (value != Parser.RParen)
								throw new ExpressionTraversalException("Expected: ')'. Found: " + parser.Next(-1).Text);
						}

						ret = new Nodes.FunctionNode(nodes, name);
					}
					else
					{
						string name = parser.Current().Text;
						parser.Consume();
						if (parser.Current().Value == Parser.Assigment)
						{
							parser.Consume();
							IExpressionNode node = CreateExpressionTree(parser, true);
							ret = new Nodes.AssignmentNode(name, Program.vartype, node);
						}
						else
						{
							ret = new Nodes.VariableNode(name);
						}
					}
					break;
				case Parser.LParen:
					parser.Consume();
					ret = CreateExpressionTree(parser, true);
					if (parser.Current().Value != Parser.RParen)
						throw new ExpressionTraversalException("Expected ')'. Found: " + parser.Current().Text);
					parser.Consume();
					break;
				case Parser.Operator:
					if (parser.Current().Text == "-" || parser.Current().Text == "+")
					{
						string op = parser.Current().Text;
						parser.Consume();
						var node = CreateExpressionTree(parser, false);
						ret = new Nodes.UnaryOperatorNode(op, node);
						break;
					}
					throw new ExpressionTraversalException("Unexpected Token: " + parser.Current().Text);
				default:
					throw new ExpressionTraversalException("Unexpected Token: " + parser.Current().Text);
			}

			if(parser.Current().Value == Parser.Operator)
			{
				IExpressionNode rhs = null;
				string op = parser.Current().Text;
				if (op == "/" || op == "*")
				{
					do
					{
						parser.Consume();
						rhs = CreateExpressionTree(parser, false);
						ret = new Nodes.OperatorNode(ret, rhs, op);
						op = parser.Current().Text;
					} while (parser.Current().Value == Parser.Operator && (op == "*" || op == "/"));
				}
				if (cond)
				{
					if (op == "+" || op == "-")
					{
						do
						{
							parser.Consume();
							rhs = CreateExpressionTree(parser, false);
							ret = new Nodes.OperatorNode(ret, rhs, op);
							op = parser.Current().Text;
						} while (parser.Current().Value == Parser.Operator && (op == "+" || op == "-"));
					}
				}
			}
			
			return ret;
		}
	}
}
