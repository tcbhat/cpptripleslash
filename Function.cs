using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CppTripleSlash
{
    public static class StringExtensions
    {
        public static string SuperTrim(this string str)
        {
            var sb = new StringBuilder(str.Length);
            var space = false;
            for (var i = 0; i < str.Length; i++)
            {
                if (!space && str[i] == ' ')
                {
                    space = true;
                    sb.Append(' ');
                }
                else
                {
                    space = false;
                    sb.Append(str[i]);
                }
            }
            return sb.ToString().Trim();
        }
    }

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
            arg = arg
                .Replace("const", "")
                .Replace("volatile", "")
                .Replace("&", "")
                .Replace("*", "")
                .SuperTrim()
                //http://en.cppreference.com/w/cpp/language/types
                .Replace("unsigned long long int", "int")
                .Replace("signed long long int", "int")
                .Replace("unsigned long long", "int")
                .Replace("unsigned short int", "int")
                .Replace("unsigned long int", "int")
                .Replace("signed long long", "int")
                .Replace("signed short int", "int")
                .Replace("signed long int", "int")
                .Replace("unsigned short", "int")
                .Replace("unsigned long", "int")
                .Replace("long long int", "int")
                .Replace("unsigned int", "int")
                .Replace("signed short", "int")
                .Replace("signed long", "int")
                .Replace("long double", "int")
                .Replace("signed int", "int")
                .Replace("short int", "int")
                .Replace("long long", "int")
                .Replace("long int", "int")
                .Replace("unsigned", "int")
                .Replace("signed", "int")
                .Replace("short", "int")
                .Replace("long", "int");
            int lastSpace = arg.LastIndexOf(' ');
            if (lastSpace == -1)
            {
                return "";
            }
            return arg.Substring(lastSpace + 1).SuperTrim();
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
            return result.Where(s => !string.IsNullOrEmpty(s)).Select(s => s.SuperTrim());
        }

        public void Parse(string decl)
        {
            ReturnType = null;
            Arguments.Clear();
            if (string.IsNullOrEmpty(decl))
            {
                throw new ParseException("NullOrEmpty");
            }
            decl = decl.SuperTrim();
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
            string nameReturnType = decl.Substring(0, firstParen).SuperTrim();
            int lastSpace = nameReturnType.LastIndexOf(' ');
            string args = decl.Substring(firstParen + 1, lastParen - firstParen - 1).SuperTrim();
            foreach (string arg in SplitArgs(args))
            {
                Arguments.Add(GetArgumentName(arg).SuperTrim());
            }
            ReturnType = nameReturnType.Substring(0, lastSpace).Replace("static", "").SuperTrim();
        }

        public override string ToString()
        {
            var sb = new StringBuilder(ReturnType + "(");
            for (var i = 0; i < Arguments.Count; i++)
            {
                if (i > 0)
                    sb.Append(',');
                sb.Append($"\"{Arguments[i]}\"");
            }
            return sb.ToString() + ")";
        }
    }
}
