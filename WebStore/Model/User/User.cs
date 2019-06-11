using WebStore.Db.Attribute;

namespace WebStore.Model.Users
{
    [Table]
    public class User : IUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [PrimaryKey]
        public string Id { get; private set; }

        public static User Make() => new User {Id = Identification.Generate()};

        public void Commit()
        {
            throw new System.NotImplementedException();
        }
    }
}