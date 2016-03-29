using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubexpressionEliminator
{
	class Optimizer
	{
		public static void RemoveUnused(ICollection<IExpressionNode> nodes)
		{
			bool cond;
			do
			{
				cond = false;

				List<IExpressionNode> ns = nodes.ToList();

				for (int j = 0; j < ns.Count; ++j)
				{
					var n = ns[j];
					if (n is Nodes.AssignmentNode)
					{
						if (n.Children.Count == 1 && n.Children[0] is Nodes.VariableNode)
						{
							IExpressionNode node = n.Children[0];
							string name = node.TextValue;
							string replace = (n as Nodes.AssignmentNode).name;
							nodes.Remove(n);
							var flat = NodeFactory.FlattenAll(nodes);
							foreach (var f in flat)
							{
								for (int i = 0; i < f.Children.Count; ++i)
								{
									if (f.Children[i] is Nodes.VariableNode && f.Children[i].TextValue == replace)
									{
										f.Children[i] = node;
										cond = true;
									}
								}
							}
						}
					}
				}
			} while (cond);
		}

		public static List<IExpressionNode> OptimizeTree(IExpressionNode tree)
		{
			List<IExpressionNode> nodes = new List<IExpressionNode>();
			nodes.Add(tree);

			bool creatednode = false;
			int tmpcounter = 0;
			const int max = 300;

			do
			{
				List<IExpressionNode> flat = NodeFactory.FlattenAll(nodes);

				foreach (var node in flat)
				{
					if (node is Nodes.AssignmentNode || node is Nodes.LiteralNode || node is Nodes.VariableNode)
						//There is no point in optimizing these to variables
						continue;

					bool create = false;

					IExpressionNode newnode = new Nodes.VariableNode(Program.tmpvarstart + tmpcounter);

					for (int k = 0; k < flat.Count; k++)
					{
						var lst = flat[k].Children;
						for (int j = 0; j < lst.Count; ++j)
						{
							if (lst[j].Equals(node))
							{
								create = true;
								lst[j] = newnode;
							}
						}
					}

					if (create)
					{

						creatednode = true;
						nodes.Add(new Nodes.AssignmentNode(Program.tmpvarstart + tmpcounter, Program.vartype, node));
						tmpcounter++;
					}
				}
			} while (creatednode && tmpcounter < max);

			return nodes;
		}
	}
}
