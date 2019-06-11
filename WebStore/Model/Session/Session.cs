using System;
using WebStore.Db.Attribute;
using WebStore.Helpers;
using WebStore.Model.Accounts;

namespace WebStore.Model.Session
{
    public interface ISession : IDatabaseEntity
    {
        Account Account { get; }
        TimeSpan TTL { get; }
        DateTime LastAction { get; }
        string Token { get; }
        DateTime Expires();
        bool Expired();
        void Refresh();
        void Update();
    }

    [Table]
    public class Session : ISession
    {
        [PrimaryKey]
        public string Id { get; private set; }

        [ForeignKey]
        public Account Account { get; private set; }

        public TimeSpan TTL { get; private set; }
        public DateTime LastAction { get; private set; }

        public string Token { get; private set; }

        public DateTime Expires() => LastAction.Add(TTL);

        public bool Expired() => Expires() > DateTime.UtcNow;

        public void Refresh()
        {
            LastAction = DateTime.UtcNow;
            Token = SessionToken.Generate();
        }

        public void Update() => LastAction = DateTime.UtcNow;

        public static Session Make(Account account, TimeSpan ttl)
        {
            return new Session
            {
                Id = Identification.Generate(),
                Account = account,
                TTL = ttl,
                LastAction = DateTime.UtcNow,
                Token = SessionToken.Generate()
            };
        }

        public void Commit()
        {
            throw new NotImplementedException();
        }
    }

    //class LongSession : Session
    //{
    //    public LongSession(User.IUser user)
    //    {
    //        User = user;
    //        TimeToLive = TimeSpan.FromHours(6);
    //        LastAction = DateTime.UtcNow;
    //    }


    //    public User.IUser User { get; }

    //    public TimeSpan TimeToLive { get; }
    //    public DateTime LastAction { get; private set; }

    //    public bool Expired => LastAction.Add(TimeToLive) > DateTime.UtcNow;

    //    public void Refresh() => LastAction = DateTime.UtcNow;
    //}
}