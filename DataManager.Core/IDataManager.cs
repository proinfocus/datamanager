using System.Collections.Generic;

namespace DataManager.Core
{
    /// <summary>
    /// Interface for Data Manager performing CRUD operations.
    /// </summary>
    /// <typeparam name="T">Tablename from the Database</typeparam>
    public interface IDataManager<T> where T : new()
    {
        /// <summary>
        /// Insert data passed as the parameter.
        /// </summary>
        /// <param name="table">Instance of object having data with name and structure</param>
        /// <returns>The last row Id for successful inserts</returns>
        long Insert(T table);

        /// <summary>
        /// Bulk insert data passed as list of objects.
        /// </summary>
        /// <param name="table">Instance of object having data with name and structure</param>
        /// <returns>The last row Id for successful inserts</returns>
        long BulkInsert(List<T> table);

        /// <summary>
        /// Update the data passed as parameter.
        /// </summary>
        /// <param name="table">Instance of object having data with name and structure</param>
        /// <returns>True for successful updation otherwise False</returns>
        bool Update(T table);

        /// <summary>
        /// Delete the data passed as parameter.
        /// </summary>
        /// <param name="table">Instance of object having data with name and structure</param>
        /// <returns>True for successful deletion otherwise False</returns>
        bool Delete(T table);

        /// <summary>
        /// Get records for a given Tablename.
        /// </summary>
        /// <typeparam name="T1">Table to fetch data from</typeparam>
        /// <param name="topRecords">Count of records to return, 0 for all recrods</param>
        /// <returns>List of data for the Tablename T1</returns>
        IEnumerable<T> Select<T1>(long topRecords = 0) where T1 : new();

        /// <summary>
        /// Get records of the executing query.
        /// </summary>
        /// <param name="query">Query to execute</param>
        /// <param name="parameters">Values to be passed for any parameters used.</param>
        /// <returns>List of data for the Tablename T1</returns>
        IEnumerable<T> Query(string query, object[] parameters = null);

        /// <summary>
        /// Execute a query which doesn't returns data.
        /// </summary>
        /// <param name="query">Query to execute</param>
        /// <param name="parameters">Values to be passed for any parameters used.</param>
        /// <returns>True for successful execution otherwise False</returns>
        bool NonQuery(string query, object[] parameters = null);
    }
}
