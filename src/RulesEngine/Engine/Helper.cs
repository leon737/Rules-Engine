using System;
using System.Reflection;

namespace RulesEngine.Engine
{
    internal static class Helper
    {
        public static object GetMemberValue<T>(MemberInfo mi, T a)
        {
            object val;
            if (mi is FieldInfo)
            {
                val = ((FieldInfo)mi).GetValue(a);
            }
            else
            {
                val = ((PropertyInfo)mi).GetValue(a);
            }
            return val;
        }

        public static void SetMemberValue<T>(MemberInfo mi, T b, object val)
        {
            if (mi is FieldInfo)
            {
                ((FieldInfo)mi).SetValue(b, val);
            }
            else
            {
                ((PropertyInfo)mi).SetValue(b, val);
            }
        }


        public static object GetDefaultValue(Type t)
        {
            if (t.IsValueType)
                return Activator.CreateInstance(t);

            return null;
        }

        public static bool CheckIsNotNullOrDefault<T>(MemberInfo mi, T a)
        {
            object val;
            Type t;
            if (mi is FieldInfo)
            {
                val = ((FieldInfo)mi).GetValue(a);
                t = ((FieldInfo)mi).FieldType;
            }
            else
            {
                val = ((PropertyInfo)mi).GetValue(a);
                t = ((PropertyInfo)mi).PropertyType;
            }

            if (val == null || (val).Equals(Helper.GetDefaultValue(t)))
            {
                return false;
            }

            if (t == typeof(string) && (string)val == "")
                return false;

            return true;
        }
    }
}
