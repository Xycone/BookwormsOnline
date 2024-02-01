using BookwormsOnline.CustomValidation;
using System.ComponentModel.DataAnnotations;

namespace BookwormsOnline.ViewModels
{
    public class ForgotPassword
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^\w+[\+\.\w-]*@([\w-]+\.)*\w+[\w-]*\.([a-z]{2,4}|\d+)$", ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
    }
}
