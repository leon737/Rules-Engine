using System.Linq;

namespace RulesEngine.Engine
{
    public interface IDataAdapter<T>
    {
        IQueryable<T> GetData();

        void SaveChanges();
    }
}
