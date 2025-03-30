namespace OSManager.Services
{
    public class EmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly IConfiguration _configuration;

        public EmailService(ILogger<EmailService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public Task SendPasswordResetEmailAsync(string email, string token)
        {
            // Em ambiente de produção, configuraria SMTP e enviaria email real
            // Para este projeto, apenas logamos a informação
            var resetUrl = $"{_configuration["AppUrl"]}/reset-password?token={token}";

            _logger.LogInformation($"[MOCK EMAIL] Password reset requested for {email}");
            _logger.LogInformation($"[MOCK EMAIL] Reset URL: {resetUrl}");

            return Task.CompletedTask;
        }

        public Task SendOrderStatusUpdateEmailAsync(string email, string orderDescription, string oldStatus, string newStatus)
        {
            _logger.LogInformation($"[MOCK EMAIL] Order status update for {email}");
            _logger.LogInformation($"[MOCK EMAIL] Order '{orderDescription}' status changed from {oldStatus} to {newStatus}");

            return Task.CompletedTask;
        }
    }
}