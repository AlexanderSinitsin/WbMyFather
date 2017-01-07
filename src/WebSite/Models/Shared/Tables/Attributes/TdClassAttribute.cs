using System;

namespace WebSite.Models.Shared.Tables.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TdClassAttribute : Attribute
    {
        public string ClassName { get; }

        public TdClassAttribute(string className)
        {
            ClassName = className;
        }
    }
}
