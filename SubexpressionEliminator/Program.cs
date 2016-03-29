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
		public static string tmpvarstart = "tmp";
		public static string vartype = "double";
		
		static void Main(string[] args)
		{
			if(args.Length < 2)
			{
				Console.WriteLine("Usage: " + AppDomain.CurrentDomain.FriendlyName + " <source file> <dest file>");
				return;
			}

			string file = File.ReadAllText(args[0]);

			//This makes the debugger break when the exception is thrown
			//It serves no use in release builds so we enable it.
#if !DEBUG
			try
			{
#endif
				IExpressionNode node = NodeFactory.ParseExpressionTree(file);
				List<IExpressionNode> nodes = Optimizer.OptimizeTree(node);
				Optimizer.RemoveUnused(nodes);
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
				//Error reporting needs to be improved
				//TODO: Make error reporting report the line where it errored
				Console.WriteLine("Error while parsing input: " + e.Message);
			}
#endif
		}
	}
}
