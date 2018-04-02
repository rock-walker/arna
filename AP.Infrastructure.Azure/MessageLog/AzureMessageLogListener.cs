namespace AP.Infrastructure.Azure.MessageLog
{
    using System;
    using AP.Infrastructure.Azure.Messaging;
    using Microsoft.Azure.ServiceBus;

    public class AzureMessageLogListener : IProcessor, IDisposable
    {
        private IAzureMessageLogWriter eventLog;
        private IMessageReceiver receiver;

        public AzureMessageLogListener(IAzureMessageLogWriter eventLog, IMessageReceiver receiver)
        {
            this.eventLog = eventLog;
            this.receiver = receiver;
        }

        public void SaveMessage(Message msg)
        {
            this.eventLog.Save(msg.ToMessageLogEntity());
        }

        public void Start()
        {
            this.receiver.Start(m => { this.SaveMessage(m); return MessageReleaseAction.CompleteMessage; });
        }

        public void Stop()
        {
            this.receiver.Stop();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                using (this.receiver as IDisposable) { }
            }
        }
    }
}
