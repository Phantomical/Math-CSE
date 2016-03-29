using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubexpressionEliminator.Nodes
{
	class FunctionNode : IExpressionNode
	{
		List<IExpressionNode> arguments;
		string name;

		public List<IExpressionNode> Children
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
					output += arg.TextValue;
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

		public FunctionNode(List<IExpressionNode> args, string name)
		{
			arguments = args;
			this.name = name;
		}
	}
}
