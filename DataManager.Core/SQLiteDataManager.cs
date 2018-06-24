using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.Linq;

namespace DataManager.Core
{
    public class SQLiteDataManager<T> : IDataManager<T> where T : new()
    {
        private readonly string connectionString;

        public SQLiteDataManager()
        {
            connectionString = ConfigurationManager.ConnectionStrings["SQLiteDataManagerCS"].ToString();
        }

        public long Insert(T table)
        {
            long output = 0;
            using(var connection = new SQLiteConnection(connectionString))
            {
                if (connection.State != System.Data.ConnectionState.Open)
                    connection.Open();

                string query = "INSERT INTO " + typeof(T).Name + " ($FIELDS$) VALUES ($VALUES$)";

                var properties = typeof(T).GetMembers();
                var o = new T();
                string fields = "";
                string values = "";
                foreach (var item in properties)
                {
                    if (item.MemberType == System.Reflection.MemberTypes.Property)
                    {
                        var prop = o.GetType().GetProperty(item.Name);
                        var result = Attribute.GetCustomAttributes(prop).FirstOrDefault();
                        if (result is IsPrimaryKey)
                            continue;

                        fields += item.Name + ",";
                        values += "$" + item.Name + ",";
                    }
                }
                fields = fields.Length > 0 ? fields.Substring(0, fields.Length - 1) : "";
                values = values.Length > 0 ? values.Substring(0, values.Length - 1) : "";
                query = query.Replace("$FIELDS$", fields);
                query = query.Replace("$VALUES$", values);

                using(var command = new SQLiteCommand(query, connection))
                {
                    foreach(var item in properties)
                    {
                        if (item.MemberType == System.Reflection.MemberTypes.Property)
                        {
                            var prop = o.GetType().GetProperty(item.Name);
                            var result = Attribute.GetCustomAttributes(prop).FirstOrDefault();
                            if (result is IsPrimaryKey)
                                continue;

                            command.Parameters.AddWithValue(item.Name, prop.GetValue(table, null));
                        }
                    }

                    output = command.ExecuteNonQuery() > 0 ? 1 : -1;
                    if (output == 1)
                    {
                        command.CommandText = "SELECT last_insert_rowid()";
                        output = (long)command.ExecuteScalar();
                    }
                }
            }
            return output;
        }

        public bool Update(T table, object[] parameters = null)
        {
            throw new NotImplementedException();
        }

        public bool Delete(T table, object[] parameters = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> Select<T1>() where T1: new()
        {
            var output = new List<T>();
            using (var connection = new SQLiteConnection(connectionString))
            {
                if (connection.State != System.Data.ConnectionState.Open)
                    connection.Open();

                using (var command = new SQLiteCommand("SELECT * FROM " + typeof(T1).Name, connection))
                {                    
                    var properties = typeof(T1).GetMembers();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var o = new T();
                            foreach (var item in properties)
                            {
                                if (item.MemberType == System.Reflection.MemberTypes.Property)
                                {
                                    var prop = o.GetType().GetProperty(item.Name);
                                    prop.SetValue(o, Convert.ChangeType(reader[item.Name], prop.PropertyType), null);
                                }
                            }
                            output.Add(o);
                        }
                    }
                }
            }
            return output;
        }

        public IEnumerable<T> Query(string query, object[] parameters = null)
        {
            var output = new List<T>();
            using (var connection = new SQLiteConnection(connectionString))
            {
                if (connection.State != System.Data.ConnectionState.Open)
                    connection.Open();

                using (var command = new SQLiteCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        int count = 0;
                        foreach(var p in parameters)
                        {
                            command.Parameters.AddWithValue(count.ToString(), p);
                            count++;
                        }
                    }

                    var properties = typeof(T).GetMembers();
                    
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var o = new T();
                            foreach (var item in properties)
                            {
                                if (item.MemberType == System.Reflection.MemberTypes.Property)
                                {
                                    var prop = o.GetType().GetProperty(item.Name);
                                    prop.SetValue(o, Convert.ChangeType(reader[item.Name], prop.PropertyType), null);
                                }
                            }
                            output.Add(o);
                        }
                    }
                }
            }
            return output;
        }
    }

    #region Attributes

    [AttributeUsage(AttributeTargets.Property)]
    public class IsPrimaryKey : Attribute
    {
        public bool Value { get; set; }
    }

    #endregion
}
