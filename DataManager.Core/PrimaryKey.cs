using System;

namespace DataManager.Core
{
    /// <summary>
    /// Use this attribute to define Primary Key for an object.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PrimaryKey : Attribute
    {
    }
}
