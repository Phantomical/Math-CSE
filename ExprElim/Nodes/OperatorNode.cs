using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ExprElim.Nodes
{
	[DebuggerDisplay("{TextValue}")]
	class OperatorNode : IExpressionNode
	{
		string op;
		RefList operands;
		int hash;

		public RefList Children
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
				return "(" + operands[0].Object.TextValue + op + operands[1].Object.TextValue + ")";
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

		public OperatorNode(Ref<IExpressionNode> lhs, Ref<IExpressionNode> rhs, string op)
		{
			this.op = op;
			operands = new RefList();
			operands.Add(lhs);
			operands.Add(rhs);
		}

		public void CalcHash()
		{
			operands[0].CalcHash();
			operands[1].CalcHash();

			hash = (op.GetHashCode() + 1720) 
				^ operands[0].GetHashCode() 
				- operands[1].GetHashCode();
		}
		public override int GetHashCode()
		{
			return hash;
		}
	}
}
