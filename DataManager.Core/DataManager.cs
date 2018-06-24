using System;
using System.Collections.Generic;

namespace DataManager.Core
{
    public class DataManager<T> : IDataManager<T>
    {
        public long Insert()
        {
            Console.WriteLine("Insert function performed.");
            return 1;
        }

        public bool Update()
        {
            Console.WriteLine("Update function performed.");
            return true;
        }

        public bool Delete()
        {
            Console.WriteLine("Delete function performed.");
            return true;
        }

        public List<T> Select<T1>(object[] parameters = null)
        {
            Console.WriteLine("Select function performed.");
            return new List<T>();
        }

        public List<T> Query(string query, object[] parameters = null)
        {
            Console.WriteLine("Query function performed.");
            return new List<T>();
        }
    }
}
