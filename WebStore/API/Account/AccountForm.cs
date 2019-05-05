using System.ComponentModel.DataAnnotations;

namespace WebStore.API
{
    public class AccountForm : IForm
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}