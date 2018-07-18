using DataManager.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace DataManager.UI
{
    class Program
    {
        private static IDataManager<Dummy> dataManager;

        static void Main(string[] args)
        {
            Setup();


            //InsertQuery();

            //BulkInsertQuery();

            //UpdateQuery();

            //DeleteQuery();        

            NonQuery();

            SelectQuery();

            Console.ReadLine();
        }

        private static void NonQuery()
        {
            bool output = dataManager.NonQuery("DELETE FROM Dummy");
            if (output)
            {
                Console.WriteLine("Non query execution was successful.");
            }
            else
            {
                Console.WriteLine("Non query execution failed.");
            }
            Console.WriteLine();
        }

        private static void Setup()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SQLiteDataManagerCS"].ToString();
            dataManager = new SQLiteDataManager<Dummy>(connectionString);
        }

        private static void SelectQuery()
        {
            var output = dataManager.Select<Dummy>();

            foreach (var item in output)
            {
                Console.WriteLine(item.Id + ", " + item.Name);
            }
            Console.WriteLine();
        }

        private static void InsertQuery()
        {
            var output = dataManager.Select<Dummy>();

            var item = new Dummy
            {
                Name = Guid.NewGuid().ToString("D")
            };

            long result = dataManager.Insert(item);
            Console.WriteLine("New record was inserted having name: " + item.Name);
            Console.WriteLine();
        }

        private static void BulkInsertQuery()
        {
            var output = dataManager.Select<Dummy>();
            var list = new List<Dummy>();

            for(int i = 0; i<10; i++)
            {
                list.Add(new Dummy
                {
                    Name = Guid.NewGuid().ToString("D")
                });
            }
            long result = dataManager.BulkInsert(list);
            Console.WriteLine("10 Records were inserted.");
            Console.WriteLine();
        }

        private static void UpdateQuery()
        {
            var output = dataManager.Select<Dummy>();

            foreach (var item in output)
            {
                item.Name += " Updated.";
                dataManager.Update(item);
                Console.WriteLine(item.Id + ", " + item.Name);
            }
            Console.WriteLine();
        }

        private static void DeleteQuery()
        {
            var output = dataManager.Select<Dummy>();
            var lastItem = output.LastOrDefault();
            if (lastItem != null)
            {
                dataManager.Delete(lastItem);
                Console.WriteLine("Last item deleted having name: " + lastItem.Name);
            }
            else
            {
                Console.WriteLine("No record to delete.");
            }
            Console.WriteLine();
        }
    }
}
