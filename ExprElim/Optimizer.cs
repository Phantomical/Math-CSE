using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExprElim
{
	class Optimizer
	{
		Stack<Ref<IExpressionNode>> stack;
		Dictionary<int, Ref<IExpressionNode>> hash;
		Dictionary<int, int> countmap;

		void InsertNode(int hval, Ref<IExpressionNode> node)
		{
			while (true)
			{
				if (!hash.ContainsKey(hval))
				{
					hash.Add(hval, node);
					countmap.Add(hval, 0);
					break;
				}
				else if (!node.Object.Equals(hash[hval].Object))
					hval++;
				else
				{
					node.Object = hash[hval].Object;
					countmap[hval]++;
					break;
				}
			}
		}

		public RefList OptimizeTree(RefList nodes)
		{
			foreach (var node in nodes)
			{
				node.CalcHash();
			}

			stack = new Stack<Ref<IExpressionNode>>(nodes);
			hash = new Dictionary<int, Ref<IExpressionNode>>();
			countmap = new Dictionary<int, int>();

			while (stack.Count != 0)
			{
				var node = stack.Pop();
				int hval = node.GetHashCode();

				if (node.Object is Nodes.VariableNode || node.Object is Nodes.LiteralNode)
					continue;

				InsertNode(hval, node);

				var children = node.Object.Children;
				foreach (var child in children)
				{
					stack.Push(child);
				}
			}

			RefList output = new RefList();
			int ctr = 0;

			foreach(var node in hash)
			{
				if (node.Value.Object is Nodes.AssignmentNode)
					output.Add(node.Value);
				if (node.Value.Object is Nodes.RootNode)
					output.Add(node.Value);
				else
				{
					if (countmap[node.Key] != 0)
					{
						string varname = Program.tmpvarstart + (ctr++).ToString();
						Nodes.AssignmentNode nnode = new Nodes.AssignmentNode(
							varname, Program.vartype, new Ref<IExpressionNode>(node.Value.Object));
						node.Value.Object = new Nodes.VariableNode(varname);
						output.Add(new Ref<IExpressionNode>(nnode));
					}
				}
			}

			return output;
		}
	}
}
