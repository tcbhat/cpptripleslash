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

        public static string ReplaceWord(this string str, string pattern, string replacement)
        {
            return Regex.Replace(str, "\\b" + pattern + "\\b", replacement);
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
            if (arg.Contains("=")) //default value => "int n = 42"
            {
                int eqIndex = arg.IndexOf('=');
                return GetArgumentName(arg.Substring(0, eqIndex));
            }
            if (arg.Contains("(")) //function parameter => "int (*name)(...)"
            {
                Match match = Regex.Match(arg, "\\( *\\* *([a-zA-Z0-9_]+) *\\)");
                if (!match.Success)
                {
                    throw new ParseException("FunctionParameterRegexFail");
                }
                return match.Groups[1].Value;
            }
            //char name[size]
            var blockOpen = arg.IndexOf('[');
            if (blockOpen != -1)
                arg = arg.Substring(0, blockOpen);
            //normal argument
            arg = arg
                .Replace("&", "")
                .Replace("*", "")
                .ReplaceWord("const", "")
                .ReplaceWord("volatile", "")
                .SuperTrim()
                //http://en.cppreference.com/w/cpp/language/types
                .ReplaceWord("unsigned long long int", "int")
                .ReplaceWord("signed long long int", "int")
                .ReplaceWord("unsigned long long", "int")
                .ReplaceWord("unsigned short int", "int")
                .ReplaceWord("unsigned long int", "int")
                .ReplaceWord("signed long long", "int")
                .ReplaceWord("signed short int", "int")
                .ReplaceWord("signed long int", "int")
                .ReplaceWord("unsigned short", "int")
                .ReplaceWord("unsigned long", "int")
                .ReplaceWord("long long int", "int")
                .ReplaceWord("unsigned int", "int")
                .ReplaceWord("signed short", "int")
                .ReplaceWord("signed long", "int")
                .ReplaceWord("long double", "int")
                .ReplaceWord("signed int", "int")
                .ReplaceWord("short int", "int")
                .ReplaceWord("long long", "int")
                .ReplaceWord("long int", "int")
                .ReplaceWord("unsigned", "int")
                .ReplaceWord("signed", "int")
                .ReplaceWord("short", "int")
                .ReplaceWord("long", "int");
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
