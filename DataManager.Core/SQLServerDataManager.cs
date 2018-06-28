﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace DataManager.Core
{
    /// <summary>
    /// SQL Server Data Manager for performing CRUD operations.
    /// </summary>
    /// <typeparam name="T">Tablename from the Database</typeparam>
    public class SQLServerDataManager<T> : IDataManager<T> where T : new()
    {
        // Connection string
        private readonly string _connectionString;

        /// <summary>
        /// Default constructor for SQL Server Data Manager.
        /// </summary>
        /// <param name="connectionString">Connection string passed by the user</param>
        public SQLServerDataManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Get the object property which is defined as Primary Key
        /// for using it for Inserts, Updates and Deletes.
        /// </summary>
        /// <param name="table">Tablename to find the Primary Key from</param>
        /// <returns>The name of the property which is marked as Primary Key attribute</returns>
        private string GetPrimaryKey(T table)
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
                    if (result is PrimaryKey)
                        return item.Name;
                }
            }
            return output;
        }

        /// <summary>
        /// Insert data passed as the parameter.
        /// </summary>
        /// <param name="table">Instance of object having data with name and structure</param>
        /// <returns>The last row Id for successful inserts</returns>
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
                string primaryKey = GetPrimaryKey(table);

                foreach (var item in properties)
                {
                    if (item.MemberType == System.Reflection.MemberTypes.Property)
                    {
                        var prop = o.GetType().GetProperty(item.Name);
                        if (item.Name == primaryKey)
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
                            if (item.Name == primaryKey)
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

        /// <summary>
        /// Bulk insert data passed as list of objects.
        /// </summary>
        /// <param name="table">Instance of object having data with name and structure</param>
        /// <returns>The last row Id for successful inserts</returns>
        public long BulkInsert(List<T> table)
        {
            long output = 0;
            using (var connection = new SqlConnection(_connectionString))
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
                        if (result is PrimaryKey)
                            continue;

                        fields += item.Name + ",";
                        values += "@" + item.Name + ",";
                    }
                }
                fields = fields.Length > 0 ? fields.Substring(0, fields.Length - 1) : "";
                values = values.Length > 0 ? values.Substring(0, values.Length - 1) : "";
                query = query.Replace("$FIELDS$", fields);
                query = query.Replace("$VALUES$", values);

                using (var command = new SqlCommand(query, connection))
                {
                    foreach (var data in table)
                    {
                        command.Parameters.Clear();
                        foreach (var item in properties)
                        {
                            if (item.MemberType == System.Reflection.MemberTypes.Property)
                            {
                                var prop = o.GetType().GetProperty(item.Name);
                                var result = Attribute.GetCustomAttributes(prop).FirstOrDefault();
                                if (result is PrimaryKey)
                                    continue;

                                command.Parameters.AddWithValue(item.Name, prop.GetValue(data, null));
                            }
                        }
                        output = command.ExecuteNonQuery() > 0 ? 1 : -1;
                    }

                    if (output == 1)
                    {
                        command.CommandText = "SELECT @@IDENTITY";
                        output = Convert.ToInt64(command.ExecuteScalar());
                    }
                }
            }
            return output;
        }

        /// <summary>
        /// Update the data passed as parameter.
        /// </summary>
        /// <param name="table">Instance of object having data with name and structure</param>
        /// <returns>True for successful updation otherwise False</returns>
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
                string primaryKey = GetPrimaryKey(table);

                foreach (var item in properties)
                {
                    if (item.MemberType == System.Reflection.MemberTypes.Property)
                    {
                        var prop = o.GetType().GetProperty(item.Name);
                        if (item.Name == primaryKey)
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

        /// <summary>
        /// Delete the data passed as parameter.
        /// </summary>
        /// <param name="table">Instance of object having data with name and structure</param>
        /// <returns>True for successful deletion otherwise False</returns>
        public bool Delete(T table)
        {
            bool output = false;
            using (var connection = new SqlConnection(_connectionString))
            {
                if (connection.State != System.Data.ConnectionState.Open)
                    connection.Open();

                string primaryKeyField = GetPrimaryKey(table);
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

        /// <summary>
        /// Get records for a given Tablename.
        /// </summary>
        /// <typeparam name="T1">Table to fetch data from</typeparam>
        /// <param name="topRecords">Count of records to return, 0 for all recrods</param>
        /// <returns>List of data for the Tablename T1</returns>
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

        /// <summary>
        /// Get records of the executing query.
        /// </summary>
        /// <param name="query">Query to execute</param>
        /// <param name="parameters">Values to be passed for any parameters used.</param>
        /// <returns>List of data for the Tablename T1</returns>
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
                        var o = new T();
                        while (reader.Read())
                        {
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

        /// <summary>
        /// Execute a query which doesn't returns data.
        /// </summary>
        /// <param name="query">Query to execute</param>
        /// <param name="parameters">Values to be passed for any parameters used.</param>
        /// <returns>True for successful execution otherwise False</returns>
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