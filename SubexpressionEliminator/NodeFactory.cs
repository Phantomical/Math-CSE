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
