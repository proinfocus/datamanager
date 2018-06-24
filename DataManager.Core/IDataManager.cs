using System.Collections.Generic;

namespace DataManager.Core
{
    public interface IDataManager<T> where T : new()
    {
        long Insert(T table);
        bool Update(T table, object[] parameters = null);
        bool Delete(T table, object[] parameters = null);
        IEnumerable<T> Select<T1>() where T1 : new();
        IEnumerable<T> Query(string query, object[] parameters = null);
    }
}
