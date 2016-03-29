using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SubexpressionEliminator
{
	class Program
	{
		static string tmpvarstart = "tmp";
		public static string vartype = "double";

		static IExpressionNode ParseToTree(string str)
		{
			Parser parser = new Parser(str);
			return NodeFactory.CreateExpressionTree(parser);
		}
		static public List<IExpressionNode> FlattenTree(IExpressionNode tree)
		{
			List<IExpressionNode> nodes = new List<IExpressionNode>();
			nodes.Add(tree);

			foreach (IExpressionNode node in tree.Children)
			{
				nodes.AddRange(FlattenTree(node));
			}

			return nodes;
		}
		static public List<IExpressionNode> FlattenAll(List<IExpressionNode> nodes)
		{
			List<IExpressionNode> start = new List<IExpressionNode>(nodes);

			foreach (var node in nodes)
			{
				start.AddRange(FlattenTree(node));
			}

			return start;
		}

		static void RemoveUnused(List<IExpressionNode> nodes)
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
							var flat = FlattenAll(nodes);
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

		static List<IExpressionNode> OptimizeTree(IExpressionNode tree)
		{
			List<IExpressionNode> nodes = new List<IExpressionNode>();
			nodes.Add(tree);

			bool creatednode = false;
			int tmpcounter = 0;
			const int max = 300;

			do
			{
				List<IExpressionNode> flat = FlattenAll(nodes);

				foreach (var node in flat)
				{
					if (node is Nodes.AssignmentNode || node is Nodes.LiteralNode || node is Nodes.VariableNode)
						//There is no point in optimizing these to variables
						continue;

					bool create = false;

					IExpressionNode newnode = new Nodes.VariableNode(tmpvarstart + tmpcounter);

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
						nodes.Add(new Nodes.AssignmentNode(tmpvarstart + tmpcounter, vartype, node));
						tmpcounter++;
					}
				}
			} while (creatednode && tmpcounter < max);

			return nodes;
		}

		static void Main(string[] args)
		{
			if(args.Length < 2)
			{
				Console.WriteLine("Usage: " + AppDomain.CurrentDomain.FriendlyName + " <source file> <dest file>");
				return;
			}

			string file = File.ReadAllText(args[0]);

#if !DEBUG
			try
			{
#endif
				IExpressionNode node = ParseToTree(file);
				List<IExpressionNode> nodes = OptimizeTree(node);
				RemoveUnused(nodes);
				IExpressionNode start = new Nodes.AssignmentNode("result", vartype, nodes.First());
				nodes.RemoveAt(0);

				Graph g = new Graph(nodes);
				g.Sort();
				nodes = g.GetList();
				nodes.Add(start);

				string total = "";

				foreach (var n in nodes)
				{
					total += n.TextValue + Environment.NewLine;
				}

				File.WriteAllText(args[1], total);
#if !DEBUG
			}
			catch (ExpressionTraversalException e)
			{
				Console.WriteLine("Error while parsing input: " + e.Message);
			}
#endif
		}
	}
}
