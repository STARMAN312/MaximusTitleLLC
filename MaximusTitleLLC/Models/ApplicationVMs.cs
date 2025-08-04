using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.ComponentModel.DataAnnotations;

namespace MaximusTitleLLC.Models
{
    public class ContactFormVM
    {
        [Required]
        public required string Name { get; set; }

        [Required, EmailAddress]
        public required string Email { get; set; }

        public required string Phone { get; set; }

        [Required, MaxLength(250)]
        public required string Message { get; set; }

        public string? HCaptchaToken { get; set; }
    }

    public class HCaptchaVerifyResponse
    {
        public bool success { get; set; }
        public DateTime? challenge_ts { get; set; }
        public required string hostname { get; set; }
        public string[]? error_codes { get; set; }
    }

    public class LogInVM
    {
        [Required(ErrorMessage = "Username is required.")]
        public required string Username { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        [DataType(DataType.Password)]
        public required string Password { get; set; }
    }

    public class RegisterVM
    {
        [Required(ErrorMessage = "Username is required.")]
        public required string Username { get; set; }
        [Required(ErrorMessage = "Full Name is required.")]
        public required string FullName { get; set; }

        [Required(ErrorMessage = "Full Name is required.")]
        public required string UserName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        [DataType(DataType.Password)]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
            ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character."
        )]
        public required string Password { get; set; }
    }
}
