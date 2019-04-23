using System;

namespace WebStore.Db.Attribute
{
    public class PrimaryKeyAttribute : System.Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ForeignKeyAttribute : System.Attribute
    {
    }
}