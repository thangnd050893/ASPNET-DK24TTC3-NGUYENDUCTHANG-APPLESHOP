using System.ComponentModel.DataAnnotations;

namespace AppleShop.Models.ViewModels
{
    public class LoginVM
    {
        [Required] public string UserName { get; set; } = "";
        [Required, DataType(DataType.Password)] public string Password { get; set; } = "";
        public bool Remember { get; set; }
    }

    public class RegisterVM
    {
        [Required, StringLength(100)] public string UserName { get; set; } = "";
        [Required, StringLength(150)] public string FullName { get; set; } = "";
        [Required, DataType(DataType.Password), StringLength(100, MinimumLength = 6)] public string Password { get; set; } = "";
        [Required, DataType(DataType.Password), Compare(nameof(Password))] public string ConfirmPassword { get; set; } = "";
    }
}
