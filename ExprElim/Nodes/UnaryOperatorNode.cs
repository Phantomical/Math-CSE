using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ExprElim.Nodes
{
	[DebuggerDisplay("{TextValue}")]
	class UnaryOperatorNode : IExpressionNode
	{
		string op;
		RefList operand;
		int hash;

		public RefList Children
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
				return "(" + op + operand[0].Object.TextValue + ")";
			}
		}

		public bool Equals(IExpressionNode Node)
		{
			if (!(Node is UnaryOperatorNode)) return false;

			var n = Node as UnaryOperatorNode;
			return op == n.op && n.operand[0].Equals(operand[0]);
		}

		public UnaryOperatorNode(string op, Ref<IExpressionNode> operand)
		{
			this.op = op;
			this.operand = new RefList();
			this.operand.Add(operand);
		}

		public void CalcHash()
		{
			operand[0].Object.CalcHash();
			hash = -operand[0].Object.GetHashCode() + op.GetHashCode();
		}
		public override int GetHashCode()
		{
			return hash;
		}
	}
}
