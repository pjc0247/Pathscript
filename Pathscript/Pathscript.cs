using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;

namespace Pathscript
{
    public class PScript
    {
        public static object Eval(object context, string src)
        {
            if (string.IsNullOrEmpty(src))
                throw new ArgumentException(nameof(src));

            var asm = context.GetType().Assembly;
            var tokens = src.Split('.');
            var _this = context;
            for (int i = 0; i < tokens.Length; i++)
            {
                var isLastToken = i == tokens.Length - 1;
                if (isLastToken)
                {
                    return GetPropertyOrField(_this, tokens[i]);
                }

                var type = asm.GetTypes()
                    .Where(x => x.Name == tokens[i])
                    .FirstOrDefault();
                if (type != null)
                    _this = type;
                else
                    _this = GetPropertyOrField(_this, tokens[i]);
            }

            return null;
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
