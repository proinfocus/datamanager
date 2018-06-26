using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace DataManager.Core
{
    public class SQLServerDataManager<T> : IDataManager<T> where T : new()
    {
        private readonly string _connectionString;

        public SQLServerDataManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        private string IsPrimaryKey(T table)
        {
            string output = "";

            var properties = typeof(T).GetMembers();
            var o = new T();
            foreach (var item in properties)
            {
                if (item.MemberType == System.Reflection.MemberTypes.Property)
                {
                    var prop = o.GetType().GetProperty(item.Name);
                    var result = Attribute.GetCustomAttributes(prop).FirstOrDefault();
                    if (result is IsPrimaryKey)
                        return item.Name;
                }
            }
            return output;
        }

        public long Insert(T table)
        {
            long output = 0;
            using(var connection = new SqlConnection(_connectionString))
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
                        values += "@" + item.Name + ",";
                    }
                }
                fields = fields.Length > 0 ? fields.Substring(0, fields.Length - 1) : "";
                values = values.Length > 0 ? values.Substring(0, values.Length - 1) : "";
                query = query.Replace("$FIELDS$", fields);
                query = query.Replace("$VALUES$", values);

                using(var command = new SqlCommand(query, connection))
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
                        command.CommandText = "SELECT @@IDENTITY";
                        output = Convert.ToInt64(command.ExecuteScalar());
                    }
                }
            }
            return output;
        }

        public bool Update(T table)
        {
            bool output = false;
            using (var connection = new SqlConnection(_connectionString))
            {
                if (connection.State != System.Data.ConnectionState.Open)
                    connection.Open();

                string query = "UPDATE " + typeof(T).Name + " SET $FIELDS$ WHERE $CONDITION$";

                var properties = typeof(T).GetMembers();
                var o = new T();
                string fields = "";
                string condition = "";
                foreach (var item in properties)
                {
                    if (item.MemberType == System.Reflection.MemberTypes.Property)
                    {
                        var prop = o.GetType().GetProperty(item.Name);
                        var result = Attribute.GetCustomAttributes(prop).FirstOrDefault();
                        if (result is IsPrimaryKey)
                            condition = item.Name + "=@" + item.Name;
                        else
                            fields += item.Name + "=@" + item.Name +",";
                    }
                }
                fields = fields.Length > 0 ? fields.Substring(0, fields.Length - 1) : "";
                query = query.Replace("$FIELDS$", fields);
                query = query.Replace("$CONDITION$", condition);

                using (var command = new SqlCommand(query, connection))
                {
                    foreach (var item in properties)
                    {
                        if (item.MemberType == System.Reflection.MemberTypes.Property)
                        {
                            var prop = o.GetType().GetProperty(item.Name);
                            command.Parameters.AddWithValue(item.Name, prop.GetValue(table, null));
                        }
                    }

                    output = command.ExecuteNonQuery() > 0 ? true : false;
                }
            }
            return output;
        }

        public bool Delete(T table)
        {
            bool output = false;
            using (var connection = new SqlConnection(_connectionString))
            {
                if (connection.State != System.Data.ConnectionState.Open)
                    connection.Open();

                string primaryKeyField = IsPrimaryKey(table);
                using (var command = new SqlCommand("DELETE FROM " + typeof(T).Name + " WHERE " + primaryKeyField + " = @" + primaryKeyField, connection))
                {
                    var o = new T();
                    var thisProp = o.GetType().GetProperty(primaryKeyField);
                    command.Parameters.AddWithValue(primaryKeyField, thisProp.GetValue(table, null));

                    if (command.ExecuteNonQuery() > 0)
                        output = true;                    
                }
            }
            return output;
        }

        public IEnumerable<T> Select<T1>(long topRecords = 0) where T1: new()
        {
            var output = new List<T>();
            using (var connection = new SqlConnection(_connectionString))
            {
                if (connection.State != System.Data.ConnectionState.Open)
                    connection.Open();

                string query = "SELECT " + (topRecords > 0 ? "TOP " + topRecords : "") + " * FROM " + typeof(T1).Name;
                using (var command = new SqlCommand(query, connection))
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
            using (var connection = new SqlConnection(_connectionString))
            {
                if (connection.State != System.Data.ConnectionState.Open)
                    connection.Open();

                using (var command = new SqlCommand(query, connection))
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

        public bool NonQuery(string query, object[] parameters = null)
        {
            bool output = false;
            using (var connection = new SqlConnection(_connectionString))
            {
                if (connection.State != System.Data.ConnectionState.Open)
                    connection.Open();

                using (var command = new SqlCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        int count = 0;
                        foreach (var p in parameters)
                        {
                            command.Parameters.AddWithValue(count.ToString(), p);
                            count++;
                        }
                    }

                    output = command.ExecuteNonQuery() > 0 ? true: false;
                }
            }

            return output;
        }
    }
}
