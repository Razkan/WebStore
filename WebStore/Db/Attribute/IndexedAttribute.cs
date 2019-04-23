using System;

namespace WebStore.Db.Attribute
{
    [AttributeUsage(AttributeTargets.Class)]
    public class IndexedAttribute : System.Attribute
    {
        public IndexedAttribute(params object[] args)
        {
        }
    }
}