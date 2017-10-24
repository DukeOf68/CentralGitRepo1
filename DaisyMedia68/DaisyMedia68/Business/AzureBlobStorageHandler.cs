using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Schema;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.RetryPolicies;

namespace DaisyMedia68.Business
{
    public class AzureBlobStorageHandler
    {
        public const string ROOT_CONTAINER_NAME = "daisycontainer1";   //Account : 'daisy01'

        private CloudBlobClient _blobClient;
        //private string _containerName;
        private string _blobName;
        private string _directoryName;

        //To initialize the default storage credentials if none are provided. 
        //For now we're going to assume everything is going to this blob storage.
        //private StorageCredentials _storageCredentials = new StorageCredentials("your_login", "the_password_from_azure");

        private CloudStorageAccount _storageAccount;
        private CloudBlobContainer _container;
        private CloudBlockBlob _blockBlob; // = _container.GetBlockBlobReference("myfirstupload.txt");


        private readonly TimeSpan _backOffPeriod = TimeSpan.FromSeconds(2);
        private const int RetryCount = 1;

        public TempDataDictionary DebugInfo { get; } = new TempDataDictionary();

        public AzureBlobStorageHandler()
        {
            string blobConnection = ConfigurationManager.AppSettings["StorageDefaultConnectionString"];

            DebugInfo.Add("1", "<script>alert(' blobConnection : "+ blobConnection + "');</script>");

            //_storageAccount = new CloudStorageAccount(_storageCredentials, false);
            _storageAccount = CloudStorageAccount.Parse(blobConnection);

            _blobClient = _storageAccount.CreateCloudBlobClient();

            BlobRequestOptions bro = new BlobRequestOptions()
            {
                SingleBlobUploadThresholdInBytes = 1024 * 1024, //1MB, the minimum
                ParallelOperationThreadCount = 1,
                RetryPolicy = new ExponentialRetry(_backOffPeriod, RetryCount),
            };
            _blobClient.DefaultRequestOptions = bro;

            _container = _blobClient.GetContainerReference(ROOT_CONTAINER_NAME);
            DebugInfo.Add("2", "<script>alert(' containerName : " + _container.Name + "');</script>");
        }

        //public byte[] GetBlob(string containerName, string blobName)
        //{
        //    CloudBlobContainer container = _blobClient.GetContainerReference(containerName);
        //    _blockBlob = container.GetBlockBlobReference(blobName);
        //    _blockBlob.FetchAttributes();

        //    long fileByteLength = _blockBlob.Properties.Length;
        //    byte[] fileContents = new byte[fileByteLength];
        //    _blockBlob.DownloadToByteArray(fileContents, 0);

        //    return fileContents;
        //}

        /// <summary>
        /// Downloads the contents of a blob into a byte[]
        /// </summary>
        /// <param name="blobName">The name of the blob to download</param>
        /// <returns>byte[] with the blob's contents</returns>

        public byte[] GetBlob(string blobName)
        {
            _blockBlob = _container.GetBlockBlobReference(blobName);
            _blockBlob.FetchAttributes();

            long fileByteLength = _blockBlob.Properties.Length;
            byte[] fileContents = new byte[fileByteLength];
            _blockBlob.DownloadToByteArray(fileContents, 0);

            return fileContents;
        }

        /// <summary>
        /// Updates or created a blob in Azure blob storage
        /// </summary>
        /// <param name="containerName">Name of the container to upload into.</param>
        /// <param name="blobName">Name of the blob.</param>
        /// <param name="content">The content of the blob.</param>
        /// <returns></returns>
        //public bool PutBlob(string containerName, string blobName, byte[] content)
        //{
        //    return ExecuteWithExceptionHandlingAndReturnValue(
        //        () =>
        //        {
        //            CloudBlobContainer container = _blobClient.GetContainerReference(containerName);
        //            _blockBlob = container.GetBlockBlobReference(blobName);
        //            //CloudBlob blob = container.GetBlobReference(blobName);
        //            using (var stream = new MemoryStream(content, writable: false))
        //            {
        //                _blockBlob.UploadFromStream(stream);
        //            }
        //            //blob.UploadByteArray(content);
        //        });
        //}


        public bool UploadToBlobFromFile(string filename)
        {
            return ExecuteWithExceptionHandlingAndReturnValue(
                () =>
                {
                    _blockBlob = _container.GetBlockBlobReference(filename);

                    _blockBlob.StreamWriteSizeInBytes = 256 * 1024; //256 k
                    _blockBlob.UploadFromFile(filename);
                });
        }

        /// <summary>
        /// Updates or created a blob in Azure blob storage
        /// </summary>
        /// <param name="blobName">Name of the blob.</param>
        /// <param name="content">The content of the blob.</param>
        /// <returns></returns>
        public bool PutBlob(string blobName, byte[] content)
        {
            return ExecuteWithExceptionHandlingAndReturnValue(
                () =>
                {
                    _blockBlob = _container.GetBlockBlobReference(blobName);
                    //CloudBlob blob = container.GetBlobReference(blobName);
                    using (var stream = new MemoryStream(content, writable: false))
                    {
                        _blockBlob.UploadFromStream(stream);
                    }
                    //blob.UploadByteArray(content);
                });
        }


        #region "privates"

        private bool ExecuteWithExceptionHandlingAndReturnValue(Action action)
        {
            try
            {
                action();
                return true;
            }
            catch (StorageException ex)
            {
                //Blob service error codes: https://msdn.microsoft.com/en-au/library/azure/dd179439.aspx
                //Ignore lease already present error
                if (ex.RequestInformation.ExtendedErrorInformation.ErrorCode != "409")
                {
                    return false;
                }
                throw;
            }
        }
        #endregion



    }
}

//string containerName = "daisy01";  //ConfigurationManager.AppSettings["StorageDefaultConnectionString"];

//CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
//cloudBlobClient.DefaultRequestOptions = bro;

//var cloudBlobContainer = cloudBlobClient.GetContainerReference(containerName);

////the blob will be named the same as the filename being uploaded
//CloudBlockBlob blob = cloudBlobContainer.GetBlockBlobReference(Path.GetFileName(fileName));
//blob.StreamWriteSizeInBytes = 256 * 1024; //256 k
//blob.UploadFromFile(fileName);