using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace FunctionApp1
{
    public static class BlobTriggerFunctionApp1
    {

        //TRIGGERED BY DM681* BLOBS ARRIVING IN cehcontainer1


        // local.settings.json file has the connection for the storage account, lifted from Cloud Explorer properties

        // BlobTriggerFunctionApp1 - triggered by a blob names DM68* appearing in cehcontainer1 - simple., 
        [FunctionName("BlobTriggerFunctionApp1")]
        public static void Run([BlobTrigger("cehcontainer1/DM681{name}")]Stream myBlob, string name, TraceWriter log)
        {
            log.Info($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
        }
               
    }
}
