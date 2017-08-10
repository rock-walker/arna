using System.Threading.Tasks;

namespace AP.Shared.Sender.Contracts
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
