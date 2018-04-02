using AP.Infrastructure.BlobStorage;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Polly;
using Polly.Retry;
using System;
using System.Net;
using System.Threading.Tasks;

namespace AP.Infrastructure.Azure.BlobStorage
{
    public class CloudBlobStorage : IBlobStorage
    {
        private readonly CloudStorageAccount account;
        private readonly string rootContainerName;
        private readonly CloudBlobClient blobClient;
        private readonly CloudBlobContainer containerReference;
        private readonly RetryPolicy readRetryPolicy;
        private readonly RetryPolicy writeRetryPolicy;
        private readonly ILogger<CloudBlobStorage> logger;

        public CloudBlobStorage(CloudStorageAccount account, string rootContainerName, ILogger<CloudBlobStorage> logger)
        {
            this.account = account;
            this.rootContainerName = rootContainerName;
            this.logger = logger;

            this.blobClient = account.CreateCloudBlobClient();
            var blobRequestOptions = new BlobRequestOptions
            {
                RetryPolicy = new Microsoft.WindowsAzure.Storage.RetryPolicies.NoRetry()
            };

            blobClient.DefaultRequestOptions = blobRequestOptions;
            
            //previous version
            //blobClient.RetryPolicy = new NoRetry();

            readRetryPolicy = Policy.Handle<Exception>().WaitAndRetry(new[] {TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)},
                (ex, ts, countRetry, context) => logger.LogWarning("An error occurred in attempt number {1} to read from blob storage: {0}", ex.Message, countRetry)); /*Trace*/

            writeRetryPolicy = Policy.Handle<Exception>().WaitAndRetry(new[] { TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(10)},
                (ex, ts, countRetry, context) => logger.LogWarning("An error occurred in attempt number {1} to write to blob storage: {0}", ex.Message, countRetry)); /*Trace*/

            this.containerReference = blobClient.GetContainerReference(this.rootContainerName);
            writeRetryPolicy.Execute(() => containerReference.CreateIfNotExistsAsync());
        }

        public async Task<byte[]> Find(string id)
        {
            var containerReference = blobClient.GetContainerReference(rootContainerName);
            var blobReference = containerReference.GetBlobReference(id);

            return await readRetryPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    var targetLength = blobReference.Properties.Length;
                    byte[] target = new byte[targetLength];
                    await blobReference.DownloadToByteArrayAsync(target, 0);
                    return target;
                }
                catch (StorageException e)
                {
                    var statusCode = e.RequestInformation.HttpStatusCode;
                    if (statusCode == (int)HttpStatusCode.NotFound) /*ResourceNotFound*/ 
                        //||statusCode == StorageErrorCode.BlobNotFound || e.ErrorCode == StorageErrorCode.ContainerNotFound)
                    {
                        return await Task.FromResult<byte[]>(null);
                    }

                    throw;
                }
            });
        }

        public void Save(string id, string contentType, byte[] blob)
        {
            var blobReference = containerReference.GetBlockBlobReference(id);

            this.writeRetryPolicy.Execute(() =>
            {
                //TODO: move to async version
                blobReference.UploadFromByteArrayAsync(blob, 0, blob.Length).Wait();
            });
        }

        public void Delete(string id)
        {
            var client = this.account.CreateCloudBlobClient();
            var containerReference = client.GetContainerReference(rootContainerName);
            var blobReference = containerReference.GetBlobReference(id);

            this.writeRetryPolicy.Execute(() =>
            {
                try
                {
                    blobReference.DeleteIfExistsAsync().Wait();
                }
                catch (StorageException e)
                {
                    var statusCode = e.RequestInformation.HttpStatusCode;
                    if (statusCode != (int)HttpStatusCode.NotFound) /*ResourceNotFound*/
                    {
                        throw;
                    }
                }
            });
        }
    }
}
