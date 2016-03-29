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
	/// <summary>
	/// This class builds a Directed Acyclic Graph from a list of ExpressionNodes.
	/// </summary>
	class Graph
	{
		List<Node> Nodes;
		class Node
		{
			public List<Node> Edges;
			public IExpressionNode ExprNode;
			public bool marked;

			public void FindEdges(List<Node> Nodes)
			{
				IEnumerable<IExpressionNode> flat = NodeFactory.FlattenTree(ExprNode);
				IEnumerable<IExpressionNode> lst = flat.Where(x => (x is Nodes.VariableNode));
				Edges = Nodes.Where(x =>
				{
					bool val = false;
					foreach(var n in lst)
					{
						val = val || (!Object.ReferenceEquals(x, this) && x.ShouldEdge(n));
					}
					return val;
				}).ToList();
			}

			bool ShouldEdge(IExpressionNode node)
			{
				if (ExprNode is Nodes.AssignmentNode && node is Nodes.VariableNode)
				{
					return (ExprNode as Nodes.AssignmentNode).name == (node as Nodes.VariableNode).name;
				}
				return false;
			}

			public Node(IExpressionNode node)
			{
				ExprNode = node;
				marked = false;
			}
		}

		public Graph(List<IExpressionNode> nodes)
		{
			Nodes = nodes.Select(x => new Node(x)).ToList();

			Nodes.ForEach(x => x.FindEdges(Nodes));
		}

		static void Visit(Node n, List<Node> sorted)
		{
			if (!n.marked)
			{
				n.marked = true;
				foreach (var node in n.Edges)
					Visit(node, sorted);
				sorted.Add(n);
			}
		}
		public void Sort()
		{
			List<Node> sorted = new List<Node>();

			foreach (var n in Nodes)
			{
				Visit(n, sorted);
			}

			Nodes = sorted;
		}
		public List<IExpressionNode> GetList()
		{
			return Nodes.Select(x => x.ExprNode).ToList();
		}
	}
}
