using System;
using System.Collections.Generic;
using System.Linq;

namespace ExprElim
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
			public Ref<IExpressionNode> ExprNode;
			public bool marked;

			public void FindEdges(List<Node> Nodes)
			{
				RefList lst = new RefList();
				//foreach(var node in Nodes)
				lst.AddRange(NodeFactory.FlattenTree(ExprNode)
					.Where(x => (x.Object is Nodes.VariableNode))
					.Where(x => (x.Object as Nodes.VariableNode).name.StartsWith(Program.tmpvarstart))
					);
				Edges = Nodes.Where(x =>
				{
					bool val = false;
					foreach (var n in lst)
					{
						val = val || (!Object.ReferenceEquals(x, this) && x.ShouldEdge(n));
					}
					return val;
				}).ToList();
			}

			bool ShouldEdge(Ref<IExpressionNode> node)
			{
				if (ExprNode.Object is Nodes.AssignmentNode && node.Object is Nodes.VariableNode)
				{
					return (ExprNode.Object as Nodes.AssignmentNode).name == (node.Object as Nodes.VariableNode).name;
				}
				return false;
			}

			public Node(Ref<IExpressionNode> node)
			{
				ExprNode = node;
				marked = false;
			}
		}

		public Graph(RefList nodes)
		{
			Nodes = nodes.Select(x => new Node(x)).ToList();

			foreach (var node in Nodes)
				node.FindEdges(Nodes);
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
		public List<Ref<IExpressionNode>> GetList()
		{
			return Nodes.Select(x => x.ExprNode).ToList();
		}
	}
}