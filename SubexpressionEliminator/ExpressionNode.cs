using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubexpressionEliminator
{
	interface IExpressionNode : IEquatable<IExpressionNode>
	{
		List<IExpressionNode> Children { get; }
		string TextValue { get; }
	}
}
