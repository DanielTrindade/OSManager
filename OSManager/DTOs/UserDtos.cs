using System.ComponentModel.DataAnnotations;

namespace OSManager.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class RegisterUserRequest
    {
        [Required(ErrorMessage = "Nome de usuário é obrigatório")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "Nome de usuário deve ter entre 4 e 50 caracteres")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Senha é obrigatória")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Senha deve ter pelo menos 6 caracteres")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "E-mail inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nome completo é obrigatório")]
        public string FullName { get; set; } = string.Empty;

        [RegularExpression("^(Admin|Supervisor|Technician)$", ErrorMessage = "Papel inválido")]
        public string Role { get; set; } = "Technician";
    }

    public class UpdateUserRequest
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "E-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "E-mail inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nome completo é obrigatório")]
        public string FullName { get; set; } = string.Empty;

        [RegularExpression("^(Admin|Supervisor|Technician)$", ErrorMessage = "Papel inválido")]
        public string? Role { get; set; }

        public bool? IsActive { get; set; }
    }

    public class ChangePasswordRequest
    {
        [Required(ErrorMessage = "Senha atual é obrigatória")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nova senha é obrigatória")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Nova senha deve ter pelo menos 6 caracteres")]
        public string NewPassword { get; set; } = string.Empty;
    }

    public class ForgotPasswordRequest
    {
        [Required(ErrorMessage = "E-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "E-mail inválido")]
        public string Email { get; set; } = string.Empty;
    }

    public class ResetPasswordRequest
    {
        [Required(ErrorMessage = "Token é obrigatório")]
        public string Token { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nova senha é obrigatória")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Nova senha deve ter pelo menos 6 caracteres")]
        public string NewPassword { get; set; } = string.Empty;
    }
}