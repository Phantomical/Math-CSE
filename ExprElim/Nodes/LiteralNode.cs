using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ExprElim.Nodes
{
	[DebuggerDisplay("{TextValue}")]
	class LiteralNode : IExpressionNode
	{
		public RefList Children { get { return new RefList(); } }
		public string TextValue { get; private set; }
		int hash;
		
		public bool Equals(IExpressionNode Node)
		{
			if (Node is LiteralNode)
			{
				return Node.TextValue == TextValue;
			}
			return false;
		}

		public LiteralNode(Token tok)
		{
			TextValue = tok.Text;
		}

		public void CalcHash()
		{
			hash = TextValue.GetHashCode() ^ 0x4196263;
		}
	}
}
