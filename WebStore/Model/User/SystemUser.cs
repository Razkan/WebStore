using WebStore.Db;
using WebStore.Db.Attribute;

namespace WebStore.Model.Users
{
    [Table]
    public class SystemUser : IUser
    {
        public string Id { get; }
    }
}