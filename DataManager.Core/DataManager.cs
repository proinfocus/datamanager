using System;
using System.Collections.Generic;

namespace DataManager.Core
{
    public class DataManager<T> : IDataManager<T> where T : new()
    {
        public long Insert(T table)
        {
            Console.WriteLine("Insert function performed.");
            return 1;
        }

        public bool Update(T table, object[] parameters = null)
        {
            Console.WriteLine("Update function performed.");
            return true;
        }

        public bool Delete(T table, object[] parameters = null)
        {
            Console.WriteLine("Delete function performed.");
            return true;
        }

        public IEnumerable<T> Select<T1>() where T1 : new()
        {
            Console.WriteLine("Select function performed.");
            return new List<T>();
        }

        public IEnumerable<T> Query(string query, object[] parameters = null)
        {
            Console.WriteLine("Query function performed.");
            return new List<T>();
        }
    }
}
