using System;
using System.Linq;
using DataManager.Core;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataManager.UI
{
    class Program
    {
        static void Main(string[] args)
        {
            IDataManager<Dummy> database = new SQLiteDataManager<Dummy>();
            //var result = database.Insert(new Dummy { Name = "Vishant Patil" });
            //var output = database.Query("select * from dummy where name like $0", new object[] { "r%" });

            var output = database.Select<Dummy>();
            foreach (var item in output)
            {
                Console.WriteLine(item.Id + ", " + item.Name);
            }
            Console.ReadLine();
        }
    }

    public class Dummy
    {
        [IsPrimaryKey(Value = true)]
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
