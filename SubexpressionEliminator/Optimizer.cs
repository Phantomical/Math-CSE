/*	Copyright (C) 2015 Sean Lynch

    This file is part of Math-CSE.

    Foobar is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Foobar is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Math-CSE.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubexpressionEliminator
{
	class Optimizer
	{
		class Ref
		{
			private IExpressionNode Node;
			private int Index;

			public IExpressionNode Value
			{
				get
				{
					return Node.Children[Index];
				}
				set
				{
					Node.Children[Index] = value;
				}
			}

			public Ref(IExpressionNode parent, int index)
			{
				Node = parent;
				Index = index;
			}

			bool Equals(Ref o)
			{
				return Index == o.Index && Node.Equals(o.Node);
			}
		}

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

			int tmpcounter = 0;

			/*
				This part could be put in a loop but it doesn't seem to help
				much. If this is run many times it will only create variable
				assignments without actually optimizing the algorithm at all.
				Since all those extra nodes are pruned anyway by the next
				stages. Also, until the algorithm stops producing extraneous
				nodes, it won't know when to stop. So one iteration is 
				usually good.
			*/
			List<IExpressionNode> flat = NodeFactory.FlattenAll(nodes);

			foreach (var node in flat)
			{
				if (node is Nodes.AssignmentNode || node is Nodes.LiteralNode || node is Nodes.VariableNode)
					//There is no point in optimizing these to variables
					continue;
				if (node is Nodes.UnaryOperatorNode && node.Children[0] is Nodes.LiteralNode)
					//There is no point in optimizing -1 or -2 to a separate variable
					continue;

				IExpressionNode newnode = new Nodes.VariableNode(Program.tmpvarstart + tmpcounter);

				var results = new List<Ref>();

				foreach (var n in flat)
				{
					for (int i = 0; i < n.Children.Count; ++i)
					{
						if (n.Children[i].Equals(node))
						{
							var rf = new Ref(n, i);
							if (!results.Contains(rf))
							{
								results.Add(rf);
							}
						}
					}
				}

				if (results.Count < 2)
					continue;

				foreach (var result in results)
					result.Value = newnode;

				nodes.Add(new Nodes.AssignmentNode(Program.tmpvarstart + tmpcounter, Program.vartype, node));
				tmpcounter++;
			}

			return nodes;
		}
	}
}
