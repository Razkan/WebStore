using System.Threading.Tasks;

namespace WebStore.API
{
    internal interface IValidatable<in T> where T : IForm
    {
        Task ValidateAsync(T form);

        void Validate(T form);
    }
}
