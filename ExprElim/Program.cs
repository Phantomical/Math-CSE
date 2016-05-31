using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace ExprElim
{
	class Program
	{
		public static string tmpvarstart = "tmp";
		public static string vartype = "const auto";

		static void Main(string[] args)
		{
			if (args.Length < 2)
			{
				Console.WriteLine("Usage: " + AppDomain.CurrentDomain.FriendlyName + " <source file> <dest file>");
				return;
			}

			string file = File.ReadAllText(args[0]);

			RefList nodes = NodeFactory.ParseExpressionTreeList(file);

			for (int i = 0; i < nodes.Count; ++i)
			{
				if (!(nodes[i].Object is Nodes.AssignmentNode))
					nodes[i].Object = new Nodes.RootNode(new Ref<IExpressionNode>(nodes[i].Object));
			}

			Optimizer opt = new Optimizer();
			nodes = opt.OptimizeTree(nodes);

			if (!(nodes[0].Object is Nodes.AssignmentNode))
			{
				nodes[0] = new Ref<IExpressionNode>(
					new Nodes.AssignmentNode("result", vartype, nodes.First()));
			}
			nodes.Reverse();

			Graph g = new Graph(nodes);
			g.Sort();
			var nnodes = g.GetList();

			string total = "";

			foreach(var n in nnodes)
			{
				total += n.Object.TextValue + Environment.NewLine;
			}

			File.WriteAllText(args[1], total);
		}
	}

	[DebuggerDisplay("{Object}")]
	class Ref<T> where T : IExpressionNode
	{
		public T Object;

		public Ref(T obj)
		{
			Object = obj;
		}

		public void CalcHash()
		{
			Object.CalcHash();
		}
		public override int GetHashCode()
		{
			return Object.GetHashCode();
		}

		public bool Equals(Ref<T> obj)
		{
			return Object.Equals(obj.Object);
		}
	}

	class RefList : List<Ref<IExpressionNode>>
	{
		public RefList() { }
		public RefList(IEnumerable<Ref<IExpressionNode>> e) :
			base(e)
		{
			
		}
	}

}
