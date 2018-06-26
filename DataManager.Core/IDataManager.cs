using System.Collections.Generic;

namespace DataManager.Core
{
    public interface IDataManager<T> where T : new()
    {
        long Insert(T table);
        bool Update(T table);
        bool Delete(T table);
        IEnumerable<T> Select<T1>(long topRecords = 0) where T1 : new();
        IEnumerable<T> Query(string query, object[] parameters = null);
        bool NonQuery(string query, object[] parameters = null);
    }
}
