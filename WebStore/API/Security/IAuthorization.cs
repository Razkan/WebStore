using System.Net.Http;
using System.Threading.Tasks;

namespace WebStore.API.Security
{
    public interface IAuthorization
    {
        Task Authorize(HttpRequestMessage request);
    }
}
