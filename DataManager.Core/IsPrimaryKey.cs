using System;

namespace DataManager.Core
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IsPrimaryKey : Attribute
    {
        public bool Value { get; set; }
    }
}
