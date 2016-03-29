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

namespace SubexpressionEliminator.Nodes
{
	class AssignmentNode : IExpressionNode
	{
		List<IExpressionNode> child;
		public string type;
		public string name;
		public List<IExpressionNode> Children
		{
			get
			{
				return child;
			}
		}
		public string TextValue
		{
			get
			{
				return type + " " + name + "=" + child[0].TextValue + ";";
			}
		}

		public bool Equals(IExpressionNode node)
		{
			if (node is AssignmentNode)
			{
				var n = node as AssignmentNode;
				return name == n.name && type == n.type && n.child[0] == child[0];
			}
			return false;
		}

		public AssignmentNode(string name, string type, IExpressionNode node)
		{
			this.name = name;
			this.type = type;
			child = new List<IExpressionNode>();
			child.Add(node);
		}
	}
}
