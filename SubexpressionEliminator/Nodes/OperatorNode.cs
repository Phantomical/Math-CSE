using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubexpressionEliminator.Nodes
{
	class OperatorNode : IExpressionNode
	{
		string op;
		List<IExpressionNode> operands;

		public List<IExpressionNode> Children
		{
			get
			{
				return operands;
			}
		}
		public string TextValue
		{
			get
			{
				return "(" + operands[0].TextValue + op + operands[1].TextValue + ")";
			}
		}

		public bool Equals(IExpressionNode Node)
		{
			if(Node is OperatorNode)
			{
				var n = Node as OperatorNode;
				return n.op == op && n.operands[0].Equals(operands[0]) && n.operands[1].Equals(operands[1]);
			}
			return false;
		}

		public OperatorNode(IExpressionNode lhs, IExpressionNode rhs, string op)
		{
			this.op = op;
			operands = new List<IExpressionNode>();
			operands.Add(lhs);
			operands.Add(rhs);
		}
	}
}
