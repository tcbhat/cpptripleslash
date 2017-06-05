using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CppTripleSlash
{
    class Function
    {
        class ParseException : Exception
        {
            public ParseException(string message) : base(message) { }
        }

        public string ReturnType;
        public List<string> Arguments = new List<string>();

        private static string GetArgumentName(string arg)
        {
            if (arg == "...") //variadic
            {
                return arg;
            }
            if (arg.Contains("=")) //default value
            {
                int eqIndex = arg.IndexOf('=');
                return GetArgumentName(arg.Substring(0, eqIndex - 1));
            }
            if (arg.Contains("(")) //function parameter
            {
                Match match = Regex.Match(arg, "\\( *\\* *([a-zA-Z0-9_]+) *\\)");
                if (!match.Success)
                {
                    throw new ParseException("FunctionParameterRegexFail");
                }
                return match.Groups[1].Value;
            }
            //normal argument
            int lastSpace = arg.LastIndexOf(' ');
            if (lastSpace == -1)
            {
                throw new ParseException("NoSpaceInNormalArgument");
            }
            return arg.Substring(lastSpace + 1).Trim();
        }

        private static IEnumerable<string> SplitArgs(string args)
        {
            List<string> result = new List<string>();
            int depth = 0;
            StringBuilder sb = new StringBuilder();
            foreach (char ch in args)
            {
                switch (ch)
                {
                    case '(':
                        depth++;
                        sb.Append(ch);
                        break;
                    case ')':
                        depth--;
                        sb.Append(ch);
                        break;
                    default:
                        if (depth == 0 && ch == ',')
                        {
                            result.Add(sb.ToString());
                            sb.Clear();
                        }
                        else
                        {
                            sb.Append(ch);
                        }
                        break;
                }
            }
            result.Add(sb.ToString());
            return result.Where(s => !string.IsNullOrEmpty(s)).Select(s => s.Trim());
        }

        public void Parse(string decl)
        {
            if (string.IsNullOrEmpty(decl))
            {
                throw new ParseException("NullOrEmpty");
            }
            decl = decl.Trim();
            if (!decl.EndsWith(";"))
            {
                throw new ParseException("NotEndsWithSemicolon");
            }
            int firstParen = decl.IndexOf('(');
            if (firstParen == -1)
            {
                throw new ParseException("NoFirstParen");
            }
            int lastParen = decl.LastIndexOf(')');
            if (lastParen == -1)
            {
                throw new ParseException("NoLastParen");
            }
            string nameReturnType = decl.Substring(0, firstParen).Trim();
            int lastSpace = nameReturnType.LastIndexOf(' ');
            string args = decl.Substring(firstParen + 1, lastParen - firstParen - 1).Trim();
            foreach (string arg in SplitArgs(args))
            {
                Arguments.Add(GetArgumentName(arg).Trim('*'));
            }
            ReturnType = nameReturnType.Substring(0, lastSpace).Replace("static", "").Trim();
        }
    }
}
