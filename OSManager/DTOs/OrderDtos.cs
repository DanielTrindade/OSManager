using System.ComponentModel.DataAnnotations;

namespace OSManager.DTOs
{
    public class CreateOrderRequest
    {
        [Required(ErrorMessage = "Descrição é obrigatória")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Descrição deve ter entre 10 e 500 caracteres")]
        public string Description { get; set; } = string.Empty;
    }

    public class OrderResponse
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? RejectionReason { get; set; }
        public UserDto User { get; set; } = new();
        public UserDto? Approver { get; set; }
        public List<ChecklistItemDto> ChecklistItems { get; set; } = new();
        public List<ImageDto> Images { get; set; } = new();
        public bool AllChecklistItemsCompleted { get; set; }
    }

    public class UpdateOrderStatusRequest
    {
        [Required(ErrorMessage = "ID é obrigatório")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Status é obrigatório")]
        [RegularExpression("^(Created|InProgress|Completed|Approved|Rejected)$", ErrorMessage = "Status inválido")]
        public string Status { get; set; } = string.Empty;

        [RequiredIf("Status", "Rejected", ErrorMessage = "Motivo da rejeição é obrigatório quando o status é 'Rejected'")]
        public string? RejectionReason { get; set; }
    }

    // Atributo personalizado para validação condicional
    [AttributeUsage(AttributeTargets.Property)]
    public class RequiredIfAttribute : ValidationAttribute
    {
        private readonly string _propertyName;
        private readonly object _propertyValue;

        public RequiredIfAttribute(string propertyName, object propertyValue)
        {
            _propertyName = propertyName;
            _propertyValue = propertyValue;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var instance = validationContext.ObjectInstance;
            var type = instance.GetType();
            var propertyValue = type.GetProperty(_propertyName)?.GetValue(instance, null);

            if (propertyValue?.ToString() == _propertyValue.ToString() && (value == null || (value is string str && string.IsNullOrWhiteSpace(str))))
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}