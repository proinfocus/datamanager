using System;
using DataManager.Core;

namespace DataManager.UI
{
    class Program
    {
        static void Main(string[] args)
        {
            IDataManager<Dummy> database = new DataManager<Dummy>();
            database.Query("some sql query");

            Console.ReadLine();
        }
    }

    public class Dummy
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
