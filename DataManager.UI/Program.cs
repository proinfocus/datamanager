using DataManager.Core;
using System;
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
            //string connectionString = ConfigurationManager.ConnectionStrings["SQLServerDataManagerCS"].ToString();
            //var database = new SQLServerDataManager<Dummy>(connectionString);

            string connectionString = ConfigurationManager.ConnectionStrings["SQLiteDataManagerCS"].ToString();
            var database = new SQLiteDataManager<Dummy>(connectionString);

            //var result = database.BulkInsert(new List<Dummy>
            //{
            //    new Dummy { Name = "Rahul Hadgal" },
            //    new Dummy { Name = "Rohan Hadgal" },
            //    new Dummy { Name = "Pranit Hadgal" },
            //    new Dummy { Name = "Pratyusha Hadgal" }
            //});


            //database.NonQuery("DELETE FROM Dummy WHERE Name=@0", new object[] { "Vishant Patil" });
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

            foreach (var item in output)
            {
                Console.WriteLine(item.Id + ", " + item.Name);
            }

            watch.Stop();
            Console.WriteLine("---------------------------------");
            Console.WriteLine("Time taken: " + watch.Elapsed.Seconds + " seconds");
            Console.ReadLine();
        }
    }

    public class Dummy
    {
        [PrimaryKey]
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
