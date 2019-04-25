using WebStore.Model.Accounts;

namespace WebStore.Model.Webhooks
{
    public interface IWebhook : Identifiable
    {
        //string IPEndpoint { get; set; }
        //ushort PortEndpoint { get; set; }
        //string PathEndpoint { get; set; }
        Account Account { get; }
        WebhookEndpoint Endpoint { get; }
    }
}