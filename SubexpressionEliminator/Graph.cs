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
