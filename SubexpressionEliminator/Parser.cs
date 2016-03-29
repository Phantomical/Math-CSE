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
using System.Diagnostics;

namespace SubexpressionEliminator
{
	[DebuggerDisplay("Value = {Text}")]
	struct Token
	{
		public int Value { get; private set; }
		public string Text { get; private set; }

		public Token(int val, string txt)
		{
			Value = val;
			Text = txt;
		}
	}

	class Parser
	{
		string Str;
		int Index;

		List<Token> Tokens;

		public Parser(string str)
		{
			Tokens = new List<Token>();
			Str = str;
			Index = 0;

			Parse();
		}

		void Parse()
		{
			string tmp = Str;
			while (tmp.Length != 0)
			{
				char c = tmp[0];

				if (c == '\0')
				{
					Tokens.Add(new Token(EOF, ""));
					break;
				}

				//Ignore whitespace
				if (Char.IsWhiteSpace(c) || c == '\\')
				{
					tmp = tmp.Substring(1);
					continue;
				}

				if (Char.IsDigit(c))
				{
					string total = "";
					int ctr = 0;

					while (Char.IsDigit(tmp[ctr]) || tmp[ctr] == '.')
					{
						total += tmp[ctr];
						++ctr;
					}

					tmp = tmp.Substring(ctr);

					Tokens.Add(new Token(Literal, total));
					continue;
				}

				if (c == ',')
				{
					tmp = tmp.Substring(1);
					Tokens.Add(new Token(Comma, ","));
					continue;
				}

				if (c == '(')
				{
					tmp = tmp.Substring(1);
					Tokens.Add(new Token(LParen, "("));
					continue;
				}

				if (c == ')')
				{
					tmp = tmp.Substring(1);
					Tokens.Add(new Token(RParen, ")"));
					continue;
				}

				if (c == '+' || c == '-' || c == '*' || c == '/')
				{
					Tokens.Add(new Token(Operator, "" + c));
					tmp = tmp.Substring(1);
					continue;
				}

				if (c == '=')
				{
					Tokens.Add(new Token(Assigment, "="));
					tmp = tmp.Substring(1);
					continue;
				}

				{
					string total = "";
					int ctr = 0;

					while (!Char.IsDigit(tmp[ctr])
						&& !Char.IsWhiteSpace(tmp[ctr])
						&& tmp[ctr] != '(' && tmp[ctr] != ')'
						&& tmp[ctr] != '+' && tmp[ctr] != '-'
						&& tmp[ctr] != '/' && tmp[ctr] != '*'
						&& tmp[ctr] != ',' && tmp[ctr] != '=')
					{
						total += tmp[ctr++];
						
					}

					tmp = tmp.Substring(ctr);
					Tokens.Add(new Token(Identifier, total));
					continue;
				}
			}
		}

		public Token CurrentToken
		{
			get
			{
				return Current();
			}
		}

		public Token Current()
		{
			if (Index < Tokens.Count)
				return Tokens[Index];
			return new Token(EOF, "");
		}
		public Token Next(int cnt)
		{
			if (Index + cnt < Tokens.Count)
				return Tokens[Index + cnt];
			return new Token(EOF, "");
		}
		public void Consume()
		{
			Index++;
		}

		public const int EOF = -1;
		public const int Identifier = 1;
		public const int LParen = 2;
		public const int RParen = 3;
		public const int Operator = 4;
		public const int Literal = 5;
		public const int Comma = 6;
		public const int Assigment = 7;
	}
}
