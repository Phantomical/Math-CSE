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
using System.Threading;
using System.IO;

namespace SubexpressionEliminator
{
	class Program
	{
		public static string tmpvarstart = "tmp";
		public static string vartype = "double";

		static void Main(string[] args)
		{
			if (args.Length < 2)
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
				List<IExpressionNode> node = NodeFactory.ParseExpressionTreeList(file);
				List<IExpressionNode> nodes = Optimizer.OptimizeTree(node);
				bool cond = false;
				if (!(nodes[0] is Nodes.AssignmentNode))
				{
					nodes[0] = new Nodes.AssignmentNode("result", vartype, nodes.First());
					cond = true;
				}
				nodes.Reverse();

				if(cond)
					Optimizer.RemoveUnused(nodes);
				
				Graph g = new Graph(nodes);
				g.Sort();
				nodes = g.GetList();

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
