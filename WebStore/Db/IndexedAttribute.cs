using System;

namespace WebStore.Db
{
    [AttributeUsage(AttributeTargets.Class)]
    public class IndexedAttribute : Attribute
    {
        public IndexedAttribute(params object[] args)
        {
        }
    }
}