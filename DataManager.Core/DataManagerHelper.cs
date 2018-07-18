using System;
using System.Linq;

namespace DataManager.Core
{
    /// <summary>
    /// Data Manager Helper class housing common methods to perform
    /// CRUD operations by different Database providers
    /// </summary>
    /// <typeparam name="T">Table name on which the methods perform</typeparam>
    public static class DataManagerHelper<T> where T : new()
    {
        /// <summary>
        /// Get the object property which is defined as Primary Key
        /// for using it for Inserts, Updates and Deletes.
        /// </summary>
        /// <param name="table">Tablename to find the Primary Key from</param>
        /// <returns>The name of the property which is marked as Primary Key attribute</returns>
        public static string GetPrimaryKey(T table)
        {
            string output = "";

            var properties = typeof(T).GetMembers();
            var o = new T();
            foreach (var property in properties)
            {
                if (property.MemberType == System.Reflection.MemberTypes.Property)
                {
                    var prop = o.GetType().GetProperty(property.Name);
                    var result = Attribute.GetCustomAttributes(prop).FirstOrDefault();
                    if (result is PrimaryKey)
                        return property.Name;
                }
            }
            return output;
        }
    }
}
