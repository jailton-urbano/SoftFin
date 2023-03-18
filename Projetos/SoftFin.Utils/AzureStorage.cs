using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftFin.Utils
{
    public static class AzureStorage
    {
        public static void UploadFile(string filePath, string keyName, string storage)
        {
            CloudBlockBlob blockBlob = ConectaStorage(keyName, storage);
            using (var fileStream = System.IO.File.OpenRead(filePath))
            {
                blockBlob.UploadFromStream(fileStream);
            }
        }

        private static CloudBlockBlob ConectaStorage(string keyName, string storage)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(storage);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(keyName);
            return blockBlob;
        }

        public static void DownloadFile(string filePath, string keyName, string storage)
        {
            CloudBlockBlob blockBlob = ConectaStorage(keyName, storage);
            using (var fileStream = System.IO.File.OpenWrite(filePath))
            {
                blockBlob.DownloadToStream(fileStream);
            }
        }

        public static Stream DownloadFile(string keyName, string storage)
        {
            CloudBlockBlob blockBlob = ConectaStorage(keyName, storage);
            Stream stream = new MemoryStream(); ;
            blockBlob.DownloadToStream(stream);
            stream.Position = 0;
            return stream;
        }

        public static void DeleteFile(string filePath, string keyName, string storage)
        {
            CloudBlockBlob blockBlob = ConectaStorage(keyName, storage);
            blockBlob.Delete();
        }
    }
}
