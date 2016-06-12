using System.Linq.Expressions;

namespace RulesEngine.Injectors
{
    public sealed class InjectorFactory
    {
        public static InjectorFactory GetInstance() => new InjectorFactory();        

        public IInjector<TA, TB> GetInjector<TA, TB>(Expression expr, ParameterExpression pe)
        {
            //TODO: replace with type pattern matching (Function.Fluent)
            if (expr is MemberExpression)
                return new MemberInjector<TA, TB>().Init(pe);
            if (expr is MethodCallExpression)
                return new MethodCallInjector<TA, TB>().Init(pe);
            if (expr is LambdaExpression)
                return new LambdaInjector<TA, TB>().Init(pe);
            if (expr is BinaryExpression)
                return new BinaryInjector<TA, TB>().Init(pe);
            if (expr is UnaryExpression)
                return new UnaryInjector<TA, TB>().Init(pe);

            return new DefaultInjector<TA, TB>().Init(pe);
        }
    }
}
