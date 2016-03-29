using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubexpressionEliminator.Nodes
{
	class EmptyNode : IExpressionNode
	{
		public List<IExpressionNode> Children { get { return new List<IExpressionNode>(); } }
		public string TextValue { get { return "EOF"; } }

		public bool Equals(IExpressionNode node)
		{
			return false;
		}
	}
}
