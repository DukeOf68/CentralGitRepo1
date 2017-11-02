using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Windows.Forms;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace DaisyMedia68.Business
{
    public class AzureBlobHandler2
    {

        public void UploadFileToBlobStorage(string filename)
        {


            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container.
            CloudBlobContainer container = blobClient.GetContainerReference("daisycontainer1");

            // Create the container if it doesn't already exist.
            container.CreateIfNotExists();


            container.SetPermissions(
                new BlobContainerPermissions {PublicAccess = BlobContainerPublicAccessType.Blob});


            // Retrieve reference to a blob named "myblob".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference("blobby1");

            // Create or overwrite the "myblob" blob with contents from a local file.
            using (var fileStream = System.IO.File.OpenRead(filename))
            {
                blockBlob.UploadFromStream(fileStream);
            }

        }
    }
}
