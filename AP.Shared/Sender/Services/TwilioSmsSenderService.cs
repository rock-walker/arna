using AP.Core.Model.Authentication;
using AP.Shared.Sender.Contracts;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace AP.Shared.Sender.Services
{
    public class TwilioSmsSenderService : ISmsSender
    {
        public TwilioSmsSenderService(IOptions<TwilioSmsOptions> optionsAccessor)
        {
            Options = optionsAccessor.Value;
        }

        public TwilioSmsOptions Options { get; set; }

        public Task SendSmsAsync(string number, string message)
        {
            var accountSid = Options.TwilioSmsAccountIdentification;
            var authToken = Options.TwilioSmsAccountPassword;

            TwilioClient.Init(accountSid, authToken);

            var msg = MessageResource.Create(
              to: new PhoneNumber(number),
              from: new PhoneNumber(Options.TwilioSmsAccountFrom),
              body: message);
            return Task.FromResult(0);
        }
    }
}
