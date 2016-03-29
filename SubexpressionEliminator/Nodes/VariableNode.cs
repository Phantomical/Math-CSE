using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubexpressionEliminator.Nodes
{
	class VariableNode : IExpressionNode
	{
		public string name;

		public List<IExpressionNode> Children
		{
			get
			{
				return new List<IExpressionNode>();
			}
		}
		public string TextValue
		{
			get
			{
				return name;
			}
		}

		public bool Equals(IExpressionNode Node)
		{
			if (!(Node is VariableNode)) return false;
			return Node.TextValue == name;
		}

		public VariableNode(string name)
		{
			this.name = name;
		}
	}
}
