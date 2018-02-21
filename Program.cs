using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;

namespace Azurite157Repro
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Run(async () =>
            {
                var account = CloudStorageAccount.Parse("BlobEndpoint=http://localhost:10000/devstoreaccount1;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==");
                var client = account.CreateCloudBlobClient();

                var sourceContainer = client.GetContainerReference("source-container");
                await sourceContainer.CreateIfNotExistsAsync();
                var sourceBlob = sourceContainer.GetBlockBlobReference("the-blob");
                await sourceBlob.UploadTextAsync("test");

                var targetContainer = client.GetContainerReference("target-container");
                await targetContainer.CreateIfNotExistsAsync();
                var targetBlob = targetContainer.GetBlockBlobReference(sourceBlob.Name);
                await targetBlob.StartCopyAsync(sourceBlob);

                // This call blocks for azurite, the blob storage seems to get entirely unusable after using StartCopyAsync.
                var str = await targetBlob.DownloadTextAsync();
                Console.WriteLine("Copied: " + str);
            }).Wait();
        }
    }
}
