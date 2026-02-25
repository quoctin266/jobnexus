using JobNexus.Helpers.Utils;
using JobNexus.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using RazorLight;
using RazorLight.Compilation;

namespace JobNexus.Services
{
    public class EmailService : IEmailService
    {
        private readonly IWebHostEnvironment _env;

        private readonly SmtpSettings _settings;

        private readonly ILogger<EmailService> _logger;

        private readonly RazorLightEngine _razorEngine;

        public EmailService(IWebHostEnvironment env, IOptions<SmtpSettings> options, 
                            ILogger<EmailService> logger, RazorLightEngine razorEngine)
        {
            _env = env;
            _settings = options.Value;
            _logger = logger;
            _razorEngine = razorEngine;
        }

        public async Task SendEmailAsync<T>(string toEmail, string subject, string template, T model)
        {
            var templatePath = Path.Combine(_env.ContentRootPath, "Templates", template);

            if (!File.Exists(templatePath)) 
                throw new FileNotFoundException("Email template not found.", template);

            var templateContent = await File.ReadAllTextAsync(templatePath).ConfigureAwait(false);

            // Use a unique key per template content to allow caching
            var templateKey = $"tpl-{templatePath.GetHashCode()}";

            string htmlBody;
            try
            {
                htmlBody = await _razorEngine.CompileRenderStringAsync(templateKey, templateContent, model).ConfigureAwait(false);
            }
            catch (TemplateCompilationException ex)
            {
                _logger.LogError(ex, "Razor compilation failed for template {Template}", template);
                throw;
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;

            var builder = new BodyBuilder { HtmlBody = htmlBody };
            message.Body = builder.ToMessageBody();

            try
            {
                using var client = new SmtpClient();

                var secureSocket = _settings.UseSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto;
                await client.ConnectAsync(_settings.Host, _settings.Port, secureSocket).ConfigureAwait(false);

                if (!string.IsNullOrEmpty(_settings.User))
                {
                    await client.AuthenticateAsync(_settings.User, _settings.Password).ConfigureAwait(false);
                }

                await client.SendAsync(message).ConfigureAwait(false);
                await client.DisconnectAsync(true).ConfigureAwait(false);

                _logger.LogInformation("Email sent to {To} with subject {Subject}", toEmail, subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {To}", toEmail);
                throw;
            }
        }
    }
}
