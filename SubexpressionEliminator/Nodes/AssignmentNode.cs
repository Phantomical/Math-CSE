using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubexpressionEliminator.Nodes
{
	class AssignmentNode : IExpressionNode
	{
		List<IExpressionNode> child;
		public string type;
		public string name;
		public List<IExpressionNode> Children
		{
			get
			{
				return child;
			}
		}
		public string TextValue
		{
			get
			{
				return type + " " + name + "=" + child[0].TextValue + ";";
			}
		}

		public bool Equals(IExpressionNode node)
		{
			if (node is AssignmentNode)
			{
				var n = node as AssignmentNode;
				return name == n.name && type == n.type && n.child[0] == child[0];
			}
			return false;
		}

		public AssignmentNode(string name, string type, IExpressionNode node)
		{
			this.name = name;
			this.type = type;
			child = new List<IExpressionNode>();
			child.Add(node);
		}
	}
}
