using System.Linq;
using RulesEngine.Engine;

namespace RulesEngineTests
{
    public class SampleDataAdapter<T> : IDataAdapter<T>
    {
        public IQueryable<T> GetData() => new[] {default(T)}.AsQueryable();

        public void SaveChanges()
        {
            
        }
    }
}
