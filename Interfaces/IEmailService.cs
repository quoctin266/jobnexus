namespace JobNexus.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync<T>(string toEmail, string subject, 
                                    string template, T model);
    }
}
