using Azure;
using Azure.Communication.Email;
using JobNexus.Helpers.Utils;
using JobNexus.Interfaces;
using Microsoft.Extensions.Options;
using RazorLight;
using RazorLight.Compilation;

using static JobNexus.Helpers.Utils.HelperFunctions;

namespace JobNexus.Services
{
    public class EmailService : IEmailService
    {
        private readonly ACSSettings _settings;

        private readonly ILogger<EmailService> _logger;

        private readonly RazorLightEngine _razorEngine;

        private readonly EmailClient _emailClient;

        public EmailService(IOptions<ACSSettings> options, EmailClient emailClient,
                            ILogger<EmailService> logger, RazorLightEngine razorEngine)
        {
            _settings = options.Value;
            _emailClient = emailClient;
            _logger = logger;
            _razorEngine = razorEngine;
        }

        public async Task SendEmailAsync<T>(string toEmail, string subject, string template, T model)
        {
            var templateContent = await ReadTemplateAsync(template);

            // Use a unique key per template content to allow caching
            var templateKey = $"tpl-{template.GetHashCode()}";

            string htmlBody;
            try
            {
                htmlBody = await _razorEngine.CompileRenderStringAsync(templateKey, templateContent, model);
            }
            catch (TemplateCompilationException ex)
            {
                _logger.LogError(ex, "Razor compilation failed for template {Template}", template);
                throw;
            }

            var senderAddress = _settings.FromEmail;

            if (string.IsNullOrWhiteSpace(senderAddress))
            {
                throw new InvalidOperationException("Sender email is not configured.");
            }

            // Create EmailContent
            var content = new EmailContent(subject)
            {
                Html = htmlBody
            };

            var recipients = new EmailRecipients([new EmailAddress(toEmail)]);
            var emailMessage = new EmailMessage(senderAddress, recipients, content);

            try
            {
                await _emailClient.SendAsync(WaitUntil.Completed, emailMessage);

                // Optionally inspect sendResponse for messageId:
                _logger.LogInformation("Email sent via ACS to {To} with subject {Subject}",
                    toEmail, subject);
            }
            catch (RequestFailedException ex)
            {
                // Azure SDK specific failure
                _logger.LogError(ex, "ACS Email send failed to {To} with subject {Subject}. ErrorCode: {Code}", toEmail, subject, ex.ErrorCode);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {To}", toEmail);
                throw;
            }
        }
    }
}
