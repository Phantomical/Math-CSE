using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExprElim.Nodes
{
	class EmptyNode : IExpressionNode
	{
		public RefList Children { get { return new RefList(); } }
		public string TextValue { get { return "EOF"; } }

		public bool Equals(IExpressionNode node)
		{
			return false;
		}

		public void CalcHash()
		{

		}
	}
}
