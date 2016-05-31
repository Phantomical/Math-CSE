using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ExprElim.Nodes
{
	[DebuggerDisplay("{TextValue}")]
	class FunctionNode : IExpressionNode
	{
		RefList arguments;
		string name;
		int hash;

		public RefList Children
		{
			get
			{
				return arguments;
			}
		}
		public string TextValue
		{
			get
			{
				string output = name + "(";
				bool cond = true;
				foreach (var arg in arguments)
				{
					if (!cond)
					{
						output += ",";
					}
					else
					{
						cond = false;
					}
					output += arg.Object.TextValue;
				}
				return output + ")";
			}
		}

		public bool Equals(IExpressionNode Node)
		{
			if (!(Node is FunctionNode)) return false;

			var n = Node as FunctionNode;

			if (n.arguments.Count != arguments.Count) return false;

			for(int i = 0; i < arguments.Count; ++i)
			{
				if (!arguments[i].Equals(n.arguments[i]))
					return false;
			}

			return true;
		}

		public FunctionNode(RefList args, string name)
		{
			arguments = args;
			this.name = name;
		}

		public void CalcHash()
		{
			hash = name.GetHashCode() + 0x569;
			foreach(var arg in arguments)
			{
				arg.Object.CalcHash();
				hash ^= arg.GetHashCode() / 5;
			}
		}

		public override int GetHashCode()
		{
			return hash;
		}
	}
}
