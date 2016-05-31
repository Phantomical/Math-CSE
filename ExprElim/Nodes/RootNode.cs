using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ExprElim.Nodes
{
	[DebuggerDisplay("{TextValue}")]
	class RootNode : IExpressionNode
	{
		RefList child;
		public RefList Children
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
				return child[0].Object.TextValue;
			}
		}

		public void CalcHash()
		{
			
		}

		public bool Equals(IExpressionNode other)
		{
			return other.Equals(child[0].Object);
		}

		public RootNode(Ref<IExpressionNode> node)
		{
			child = new RefList();
			child.Add(node);
		}
	}
}
