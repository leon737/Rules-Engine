using Functional.Fluent;
using System.Linq.Expressions;

namespace RulesEngine.Injectors
{
    public sealed class InjectorFactory
    {
        public static InjectorFactory GetInstance() => new InjectorFactory();        

        public IInjector<TA, TB> GetInjector<TA, TB>(Expression expr, ParameterExpression pe) =>  expr.ToMaybe().TypeMatch()
                .With(Case.Is<MemberExpression>(), _ => (IInjector<TA,TB>)new MemberInjector<TA, TB>())
                .With(Case.Is<MethodCallExpression>(), _ => new MethodCallInjector<TA, TB>())
                .With(Case.Is<LambdaExpression>(), _ => new LambdaInjector<TA, TB>())
                .With(Case.Is<BinaryExpression>(), _ => new BinaryInjector<TA, TB>())
                .With(Case.Is<UnaryExpression>(), _ => new UnaryInjector<TA, TB>())
                .Else(_=> new DefaultInjector<TA, TB>())
                .Do().Init(pe);
    }
}
