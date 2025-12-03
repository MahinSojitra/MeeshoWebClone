namespace MeeshoWebClone.Services
{
    public interface IEmailService
    {
        Task SendPasswordResetEmailAsync(string toEmail, string resetLink);
        Task SendEmailConfirmationAsync(string toEmail, string confirmationLink);
        Task SendLoginOtpEmailAsync(string toEmail, string otp);
    }
}
