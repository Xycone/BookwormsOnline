using BookwormsOnline.CustomValidation;
using System.ComponentModel.DataAnnotations;

namespace BookwormsOnline.ViewModels
{
    public class ChangePassword
    {
        [Required]
        [DataType(DataType.Password)]
        [ValidateCurrentPassword]
        public string CurrentPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[$@$!%*?&])[A-Za-z\d$@$!%*?&]{8,}$", ErrorMessage = "Passwords must be at least 8 characters long and contain at least an uppercase letter, lowercase letter, digit and a symbol")]
        [PreventOldPassword]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = "password and confirmation password does not match")]
        public string ConfirmNewPassword { get; set; }
    }
}
