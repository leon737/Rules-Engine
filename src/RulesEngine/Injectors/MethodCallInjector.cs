using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace RulesEngine.Injectors
{
    public class MethodCallInjector<TA, TB> : InjectorBase<TA, TB, MethodCallExpression>
    {
        protected override Expression InjectCore(TA a, MethodCallExpression expr)
        {
            var result = expr.Arguments.Select(argument => InjectorFactory.GetInstance().GetInjector<TA, TB>(argument, ParameterExpression).Inject(a, argument)).ToList();

            if (expr.Object is MemberExpression && ((expr.Object) as MemberExpression).Member.DeclaringType == typeof(TA))
            {
                if (expr.Method.Name == "GetValueOrDefault")
                {
                    var f = Expression.Lambda(expr.Arguments[0]).Compile();
                    var defaultValue = f.DynamicInvoke();
                    var m = (expr.Object) as MemberExpression;
                    object val;
                    Type t;
                    if (m.Member is FieldInfo)
                    {
                        val = ((FieldInfo)m.Member).GetValue(a);
                        t = ((FieldInfo)m.Member).FieldType;
                    }
                    else
                    {
                        val = ((PropertyInfo)m.Member).GetValue(a);
                        t = ((PropertyInfo)m.Member).PropertyType;
                    }
                    if (Nullable.GetUnderlyingType(t) != null && val == null)
                    {
                        return Expression.Constant(defaultValue);
                    }
                }
                var constant = MakeConstant(expr.Object, a);
                return expr.Update(constant, result);
            }
            if (ParameterExpression != null && expr.Object is MemberExpression && ((expr.Object) as MemberExpression).Member.DeclaringType == typeof(TB))
            {
                var parameter = MakeParameter(expr.Object);
                return expr.Update(parameter, result);
            }
            return expr.Update(expr.Object, result);
        }
    }
}
