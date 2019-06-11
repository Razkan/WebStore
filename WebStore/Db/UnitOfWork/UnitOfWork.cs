using WebStore.Db.Repository;

namespace WebStore.Db.UnitOfWorks
{
    public class UnitOfWork
    {
        public UnitOfWork()
        {
            AccountRepository = new AccountRepository(Database.Instance);
            CategoryRepository = new CategoryRepository(Database.Instance);
            SessionRepository = new SessionRepository(Database.Instance);
            UserRepository = new UserRepository(Database.Instance);
            WebhookRepository = new WebhookRepository(Database.Instance);
            WebhookEndpointRepository = new WebhookEndpointRepository(Database.Instance);
            WebhookSubscriptionRepository = new WebhookSubscriptionRepository(Database.Instance);
        }

        public IAccountRepository AccountRepository { get; }
        public ICategoryRepository CategoryRepository { get; }
        public ISessionRepository SessionRepository { get; }
        public IUserRepository UserRepository { get; }
        public IWebhookRepository WebhookRepository { get; }
        public IWebhookEndpointRepository WebhookEndpointRepository { get; }
        public IWebhookSubscriptionRepository WebhookSubscriptionRepository { get; }
    }
}