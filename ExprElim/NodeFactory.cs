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

namespace ExprElim
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
		public static RefList ParseExpressionTreeList(string text)
		{
			Parser parser = new Parser(text);
			RefList nodes = new RefList();

			if(parser.Current().Value == Parser.Identifier 
				&& parser.Next(1).Value == Parser.Identifier
				&& parser.Next(2).Value == Parser.Assigment)
			{
				do
				{
					string type = parser.Current().Text;
					string name = parser.Next(1).Text;
					parser.Consume();
					parser.Consume();
					parser.Consume();

					Ref<IExpressionNode> node = new Ref<IExpressionNode>(new Nodes.AssignmentNode(name, type, CreateExpressionTree(parser)));
					nodes.Add(node);
					if (parser.Current().Value == Parser.SemiColon)
						parser.Consume();
				} while (parser.Current().Value == Parser.Identifier
					&& parser.Next(1).Value == Parser.Identifier
					&& parser.Next(2).Value == Parser.Assigment);
			}
			else
			{
				nodes.Add(CreateExpressionTree(parser));
			}

			return nodes;
		}

		/// <summary>
		/// Creates an expression node tree from a string.
		/// </summary>
		/// <param name="text">The string to parse into an AST.</param>
		/// <returns></returns>
		public static Ref<IExpressionNode> ParseExpressionTree(string text)
		{
			Parser parser = new Parser(text);
			return CreateExpressionTree(parser);
		}

		/// <summary>
		/// Flattens all the nodes in a tree into a collection.
		/// </summary>
		/// <param name="tree"></param>
		/// <returns></returns>
		static public RefList FlattenTree(Ref<IExpressionNode> tree)
		{
			RefList nodes = new RefList();
			var stack = new Stack<Ref<IExpressionNode>>();
			stack.Push(tree);

			while(stack.Any())
			{
				var next = stack.Pop();
				nodes.Add(next);
				foreach (var child in next.Object.Children)
					stack.Push(child);
			}

			return nodes;
		}
		/// <summary>
		/// Flattens all the nodes in a list of trees into a collection.
		/// </summary>
		/// <param name="nodes"></param>
		/// <returns></returns>
		static public RefList FlattenAll(RefList nodes)
		{
			RefList start = new RefList(nodes);

			foreach (var node in nodes)
			{
				start.AddRange(FlattenTree(node));
			}

			return start;
		}

		public static Ref<IExpressionNode> CreateExpressionTree(Parser parser)
		{
			return CreateExpressionTree(parser, true);
		}
		private static Ref<IExpressionNode> CreateExpressionTree(Parser parser, bool cond)
		{
			Ref<IExpressionNode> ret = null;

			switch (parser.Current().Value)
			{
				case Parser.EOF:
					return new Ref<IExpressionNode>(new Nodes.EmptyNode());
				case Parser.Literal:
					{
						Token t = parser.Current();
						parser.Consume();
						ret = new Ref<IExpressionNode>(new Nodes.LiteralNode(t));
						break;
					}
				case Parser.Identifier:
					if (parser.Next(1).Value == Parser.LParen)
					{
						string name = parser.Current().Text;
						parser.Consume();
						parser.Consume();
						RefList nodes = new RefList();
						if (parser.Current().Value != Parser.RParen)
						{
							int value;
							do
							{
								Ref<IExpressionNode> node = CreateExpressionTree(parser, true);
								value = parser.Current().Value;
								parser.Consume();
								nodes.Add(node);
							} while (value == Parser.Comma);

							if (value != Parser.RParen)
								throw new ExpressionTraversalException("Expected: ')'. Found: " + parser.Next(-1).Text);
						}

						ret = new Ref<IExpressionNode>(new Nodes.FunctionNode(nodes, name));
					}
					else
					{
						string name = parser.Current().Text;
						parser.Consume();
						if (parser.Current().Value == Parser.Assigment)
						{
							parser.Consume();
							Ref<IExpressionNode> node = CreateExpressionTree(parser, true);
							ret = new Ref<IExpressionNode>(new Nodes.AssignmentNode(name, Program.vartype, node));
						}
						else
						{
							ret = new Ref<IExpressionNode>(new Nodes.VariableNode(name));
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
						ret = new Ref<IExpressionNode>(new Nodes.UnaryOperatorNode(op, node));
						break;
					}
					throw new ExpressionTraversalException("Unexpected Token: " + parser.Current().Text);
				default:
					throw new ExpressionTraversalException("Unexpected Token: " + parser.Current().Text);
			}

			if(parser.Current().Value == Parser.Operator)
			{
				Ref<IExpressionNode> rhs = null;
				string op = parser.Current().Text;
				if (op == "/" || op == "*")
				{
					do
					{
						parser.Consume();
						rhs = CreateExpressionTree(parser, false);
						ret = new Ref<IExpressionNode>(new Nodes.OperatorNode(ret, rhs, op));
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
							ret = new Ref<IExpressionNode>(new Nodes.OperatorNode(ret, rhs, op));
							op = parser.Current().Text;
						} while (parser.Current().Value == Parser.Operator && (op == "+" || op == "-"));
					}
				}
			}
			
			return ret;
		}
	}
}
