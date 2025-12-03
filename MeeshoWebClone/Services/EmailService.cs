using System.Net;
using System.Net.Mail;

namespace MeeshoWebClone.Services
{
    public class EmailSettings
    {
        public string SmtpServer { get; set; } = string.Empty;
        public int SmtpPort { get; set; }
        public string SenderEmail { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool EnableSsl { get; set; } = true;
    }

    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _emailSettings = configuration.GetSection("EmailSettings").Get<EmailSettings>() 
                ?? new EmailSettings();
            _logger = logger;
        }

        public async Task SendPasswordResetEmailAsync(string toEmail, string resetLink)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
            {
                throw new ArgumentException("Email address cannot be null or empty.", nameof(toEmail));
            }

            if (string.IsNullOrWhiteSpace(resetLink))
            {
                throw new ArgumentException("Reset link cannot be null or empty.", nameof(resetLink));
            }

            var subject = "Reset Your Meesho Password";
            var body = GetPasswordResetEmailTemplate(resetLink);

            await SendEmailAsync(toEmail, subject, body);
        }

        private async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            using var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort)
            {
                Credentials = new NetworkCredential(_emailSettings.SenderEmail, _emailSettings.Password),
                EnableSsl = _emailSettings.EnableSsl
            };

            using var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            mailMessage.To.Add(toEmail);

            await client.SendMailAsync(mailMessage);
        }

        private static string GetPasswordResetEmailTemplate(string resetLink)
        {
            return $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Reset Your Password - Meesho</title>
</head>
<body style=""margin: 0; padding: 0; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f5f5f5;"">
    <table role=""presentation"" style=""width: 100%; border-collapse: collapse;"">
        <tr>
            <td style=""padding: 40px 0;"">
                <table role=""presentation"" style=""max-width: 600px; margin: 0 auto; background-color: #ffffff; border-radius: 8px; box-shadow: 0 2px 10px rgba(0,0,0,0.1);"">
                    <!-- Header -->
                    <tr>
                        <td style=""padding: 30px 40px; text-align: center; border-bottom: 1px solid #e9ecef;"">
                            <h1 style=""margin: 0; color: #9f2089; font-size: 32px; font-weight: bold;"">Meesho</h1>
                        </td>
                    </tr>
                    
                    <!-- Main Content -->
                    <tr>
                        <td style=""padding: 40px;"">
                            <h2 style=""margin: 0 0 20px 0; color: #333333; font-size: 24px; font-weight: 600; text-align: center;"">
                                Reset Your Password
                            </h2>
                            
                            <p style=""margin: 0 0 20px 0; color: #666666; font-size: 16px; line-height: 1.6; text-align: center;"">
                                We received a request to reset your password. Click the button below to create a new password.
                            </p>
                            
                            <!-- Reset Button -->
                            <table role=""presentation"" style=""width: 100%; margin: 30px 0;"">
                                <tr>
                                    <td style=""text-align: center;"">
                                        <a href=""{resetLink}"" 
                                           style=""display: inline-block; padding: 14px 40px; background-color: #28a745; color: #ffffff; text-decoration: none; font-size: 16px; font-weight: 600; border-radius: 6px; box-shadow: 0 2px 4px rgba(40,167,69,0.3);"">
                                            Reset Password
                                        </a>
                                    </td>
                                </tr>
                            </table>
                            
                            <p style=""margin: 0 0 15px 0; color: #666666; font-size: 14px; line-height: 1.6; text-align: center;"">
                                If the button doesn't work, copy and paste this link into your browser:
                            </p>
                            
                            <p style=""margin: 0 0 20px 0; color: #9f2089; font-size: 14px; line-height: 1.6; text-align: center; word-break: break-all;"">
                                {resetLink}
                            </p>
                            
                            <hr style=""border: none; border-top: 1px solid #e9ecef; margin: 30px 0;"">
                            
                            <p style=""margin: 0; color: #999999; font-size: 13px; line-height: 1.6; text-align: center;"">
                                <strong>Didn't request this?</strong><br>
                                If you didn't request a password reset, you can safely ignore this email. Your password will remain unchanged.
                            </p>
                        </td>
                    </tr>
                    
                    <!-- Footer -->
                    <tr>
                        <td style=""padding: 20px 40px; background-color: #f8f9fa; border-radius: 0 0 8px 8px; border-top: 1px solid #e9ecef;"">
                            <p style=""margin: 0; color: #999999; font-size: 12px; text-align: center; line-height: 1.6;"">
                                This is an automated message from Meesho. Please do not reply to this email.<br>
                                © 2024 Meesho. All rights reserved.
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
        }
    }
}
