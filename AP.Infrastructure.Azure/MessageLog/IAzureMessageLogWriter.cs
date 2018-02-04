namespace AP.Infrastructure.Azure.MessageLog
{
    public interface IAzureMessageLogWriter
    {
        void Save(MessageLogEntity entity);
    }
}
