using System.ComponentModel.DataAnnotations;

namespace Talabat.APIs.Dtos
{
    public class RegisterDto
    {
        [Required]
        public string DisplayName { get; set; } = null!;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;

        [Required]
        [RegularExpression("^(?=.*[A-Z])(?=.*[a-z])(?=.*\\d)(?=.*[@#$%^&+=])(?!.*\\s).{8,16}$"
            , ErrorMessage ="Password Must Have 1 Uppercase, 1 Lowercase, 1 number ") ]
        public string Password {  get; set; } = null!;


    }
}
