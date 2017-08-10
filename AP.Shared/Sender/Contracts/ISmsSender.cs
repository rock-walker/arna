using System.Threading.Tasks;

namespace AP.Shared.Sender.Contracts
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}
