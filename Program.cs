using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using System;
using System.Threading.Tasks;    
public class Program
{
    private const string BLOB_SERVICE_ENDPOINT = "BLOB_SERVICE_ENDPOINT";
    private const string STORAGE_ACCOUNT_NAME = "STORAGE_ACCOUNT_NAME";
    private const string STORAGE_ACCOUNT_KEY = "STORAGE_ACCOUNT_KEY";    
    private const string CONTAINER_NAME = "CONTAINER_NAME";

    public static async Task Main(string[] args)
    {
        StorageSharedKeyCredential accountCredentials = new StorageSharedKeyCredential(STORAGE_ACCOUNT_NAME, STORAGE_ACCOUNT_KEY);
        BlobServiceClient client = new BlobServiceClient(new Uri(BLOB_SERVICE_ENDPOINT), accountCredentials);
        BlobContainerClient container = client.GetBlobContainerClient(CONTAINER_NAME);

        await foreach (BlobItem blob in container.GetBlobsAsync())
        {    
            BlobClient blobClient = container.GetBlobClient(blob.Name);

            BlobSasBuilder sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = CONTAINER_NAME,
                BlobName = blobClient.Name,
                Resource = "b"
            };

            sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddHours(1);
            sasBuilder.SetPermissions(BlobSasPermissions.Read |
                BlobSasPermissions.Write);

            Uri sasUri = blobClient.GenerateSasUri(sasBuilder);
            Console.WriteLine("SAS URI for blob is: {0}", sasUri);
            await Console.Out.WriteLineAsync($"Existing Blob:\t{blob.Name}");
        }
    }


}