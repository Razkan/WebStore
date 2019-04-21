using WebStore.Db;

namespace WebStore.Model.Users
{
    [Table]
    public class SystemUser : IUser
    {
        public string Id { get; }
    }
}