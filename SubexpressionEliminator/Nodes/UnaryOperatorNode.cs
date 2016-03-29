using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubexpressionEliminator.Nodes
{
	class UnaryOperatorNode : IExpressionNode
	{
		string op;
		List<IExpressionNode> operand;

		public List<IExpressionNode> Children
		{
			get
			{
				return operand;
			}
		}
		public string TextValue
		{
			get
			{
				return "(" + op + operand[0].TextValue + ")";
			}
		}

		public bool Equals(IExpressionNode Node)
		{
			if (!(Node is UnaryOperatorNode)) return false;

			var n = Node as UnaryOperatorNode;
			return op == n.op && n.operand[0].Equals(operand[0]);
		}

		public UnaryOperatorNode(string op, IExpressionNode operand)
		{
			this.op = op;
			this.operand = new List<IExpressionNode>();
			this.operand.Add(operand);
		}
	}
}
