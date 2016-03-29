using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubexpressionEliminator.Nodes
{
	class LiteralNode : IExpressionNode
	{
		public List<IExpressionNode> Children { get { return new List<IExpressionNode>(); } }
		public string TextValue { get; private set; }
		
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
	}
}
