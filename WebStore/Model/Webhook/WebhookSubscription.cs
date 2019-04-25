using WebStore.Db.Attribute;
using WebStore.Model.Accounts;
using WebStore.Model.Product;

namespace WebStore.Model.Webhooks
{
    [Table]
    public class WebhookSubscription
    {
        [PrimaryKey]
        public string Id { get; private set; }

        [ForeignKey]
        public Category Category { get; private set; }

        [ForeignKey]
        public Account Account { get; private set; }

        public bool _Insert { get; set; }
        public bool _Update { get; set; }
        public bool _Delete { get; set; }

        public static WebhookSubscription Make(Account account, Category category, bool insert, bool update,
            bool delete)
        {
            return new WebhookSubscription
            {
                Id = Identification.Generate(),
                Account = account,
                Category = category,
                _Insert = insert,
                _Update = update,
                _Delete = delete
            };
        }
    }
}