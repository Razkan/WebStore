using WebStore.Db;
using WebStore.Model.Users;

namespace WebStore.Model.Accounts
{
    public interface IAccount : Identifiable
    {
    }

    [Table]
    //[Indexed("???")]
    public class Account : IAccount
    {
        public Account()
        {
        }

        public static Account Make(string username, string password, User user)
        {
            return new Account
            {
                Username = username,
                Password = password,
                User = user,
                Id = Identification.Generate()
            };
        }

        [Unique]
        public string Username { get; private set; }

        public string Password { get; set; }

        [ForeignKey]
        public User User { get; private set; }

        [PrimaryKey]
        public string Id { get; private set; }
    }
}