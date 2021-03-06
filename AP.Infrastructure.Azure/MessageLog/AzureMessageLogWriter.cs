﻿namespace AP.Infrastructure.Azure.MessageLog
{
    using System;
    using System.Net;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;
    using Polly;

    public class AzureMessageLogWriter : IAzureMessageLogWriter
    {
        private readonly CloudStorageAccount account;
        private readonly string tableName;
        private readonly CloudTableClient tableClient;
        private Polly.Retry.RetryPolicy retryPolicy;

        public AzureMessageLogWriter(CloudStorageAccount account, string tableName)
        {
            if (account == null) throw new ArgumentNullException("account");
            if (tableName == null) throw new ArgumentNullException("tableName");
            if (string.IsNullOrWhiteSpace(tableName)) throw new ArgumentException("tableName");

            this.account = account;
            this.tableName = tableName;
            tableClient = account.CreateCloudTableClient();
            tableClient.DefaultRequestOptions = new TableRequestOptions
            {
                RetryPolicy = new Microsoft.WindowsAzure.Storage.RetryPolicies.NoRetry()
            };

            retryPolicy = Policy.Handle<Exception>().WaitAndRetry(3, retry => TimeSpan.FromSeconds(Math.Pow(2, retry)));
        }

        public void Save(MessageLogEntity entity)
        {
            retryPolicy.Execute(() =>
            {
                var context = tableClient.GetTableReference(tableName);
                var operation = TableOperation.Insert(entity);

                try
                {
                    context.ExecuteAsync(operation).Wait();
                }
                catch (Exception ex)
                {
                    var inner = ex.InnerException as StorageException;
                    if (inner == null)
                    {
                        throw;
                    }
                    var httpCode = inner.RequestInformation.HttpStatusCode;
                    if (httpCode != (int)HttpStatusCode.Conflict)
                    {
                        return;
                    }
                }
            });
        }
    }
}
