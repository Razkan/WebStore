using WebStore.Db.Attribute;

namespace WebStore.Model.Users
{
    [Table]
    public class SystemUser : IUser
    {
        [PrimaryKey]
        public string Id { get; private set; }

        public void Commit()
        {
            throw new System.NotImplementedException();
        }
    }
}