using System;
using System.Linq;
using DataManager.Core;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Diagnostics;

namespace DataManager.UI
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            //string connectionString = ConfigurationManager.ConnectionStrings["SQLiteDataManagerCS"].ToString();
            string connectionString = ConfigurationManager.ConnectionStrings["SQLServerDataManagerCS"].ToString();
            IDataManager<Dummy> database = new SQLServerDataManager<Dummy>(connectionString);
            var result = database.Insert(new Dummy { Name = "Vishant Patil" });

            //database.Delete(lastItem);
            //var output = database.Select<Dummy>();
            //var output = database.Query("select * from dummy where name like $0", new object[] { "r%" });

            var output = database.Select<Dummy>();

            //var lastItem = output.LastOrDefault();
            //if (lastItem != null)
            //{
            //    lastItem.Name += " updated.";
            //}
            //database.Update(lastItem);

            //output = database.Select<Dummy>(2);
            foreach (var item in output)
            {
                Console.WriteLine(item.Id + ", " + item.Name);
            }

            watch.Stop();
            Console.WriteLine(watch.Elapsed.Seconds + " second(s).");
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
