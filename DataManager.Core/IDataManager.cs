using System.Collections.Generic;

namespace DataManager.Core
{
    public interface IDataManager<T>
    {
        long Insert();
        bool Update();
        bool Delete();
        List<T> Select<T1>(object[] parameters = null);
        List<T> Query(string query, object[] parameters = null);
    }
}
