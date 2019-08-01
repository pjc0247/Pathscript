using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Pathscript
{
    enum LiteralType
    {
        Integer,
        Character,
        String
    }

    public class PScript
    {
        private static Regex regArray = new Regex("\\[([0-9+])\\]");

        public static object Eval(object context, string src)
        {
            if (string.IsNullOrEmpty(src))
                throw new ArgumentException(nameof(src));

            var asm = context.GetType().Assembly;
            var tokens = src.Split('.');
            var _this = context;
            for (int i = 0; i < tokens.Length; i++)
            {
                var token = tokens[i];
                var matches = regArray.Matches(token);
                object result = null;

                if (matches.Count > 0)
                    token = token.Split(new char[] { '[' }, 2)[0];

                var isLastToken = i == tokens.Length - 1;
                if (isLastToken)
                {
                    result = GetPropertyOrField(_this, token);
                }
                else
                {
                    var type = asm.GetTypes()
                        .Where(x => x.Name == token)
                        .FirstOrDefault();
                    if (type != null)
                        result = type;
                    else
                        result = GetPropertyOrField(_this, token);
                }

                foreach (Match match in matches)
                {
                    var tp = ParseLiteral(match.Groups[1].Value);
                    var type = tp.Item1;
                    var idx = tp.Item2;

                    if (result is Array ary)
                        result = ary.GetValue((int)idx);
                    else
                    {
                        foreach (var pi in result.GetType().GetProperties())
                        {
                            if (pi.GetIndexParameters().Length > 0)
                            {
                                result = pi.GetValue(result, new object[] { idx });
                            }
                        }
                    }
                }

                if (isLastToken)
                    return result;
                else
                    _this = result;
            }

            return null;
        }

        private static Tuple<LiteralType, object> ParseLiteral(string str)
        {
            if (str.Length > 2 && str.First() == '"' && str.Last() == '"')
            {
                return new Tuple<LiteralType, object>(
                    LiteralType.String, str.Substring(1, str.Length - 2));
            }
            if (str.Length > 2 && str.First() == '\'' && str.Last() == '\'')
            {
                return new Tuple<LiteralType, object>(
                    LiteralType.Character, str[1]);
            }

            int n;
            if (int.TryParse(str, out n))
                return new Tuple<LiteralType, object>(LiteralType.Integer, n);

            throw new ArgumentException("Unrecognized literal: " + str);
        }

        private static object GetStaticPropertyOrField(Type type, string key)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            var field = type.GetField(key, BindingFlags.Static | BindingFlags.Public);
            if (field != null)
                return field.GetValue(null);

            var property = type.GetProperty(key, BindingFlags.Static | BindingFlags.Public);
            if (property != null)
                return property.GetValue(null);

            throw new InvalidOperationException($"{key} does not exist in {type}.");
        }
        private static object GetInstancePropertyOrField(object obj, string key)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            var type = obj.GetType();
            var field = type.GetField(key, BindingFlags.Instance | BindingFlags.Public);
            if (field != null)
                return field.GetValue(obj);

            var property = type.GetProperty(key, BindingFlags.Instance | BindingFlags.Public);
            if (property != null)
                return property.GetValue(obj);

            throw new InvalidOperationException($"{key} does not exist in {obj}.");
        }
        private static object GetPropertyOrField(object obj, string key)
        {
            if (obj is Type t)
                return GetStaticPropertyOrField(t, key);
            else
                return GetInstancePropertyOrField(obj, key);
        }
    }
}
