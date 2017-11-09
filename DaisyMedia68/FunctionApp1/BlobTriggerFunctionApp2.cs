using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System.Collections.Generic;
using System.Linq;

namespace FunctionApp1
{
    public static class BlobTriggerFunctionApp2
    {
        //TRIGGERED BY DM682* BLOBS ARRIVING IN cehcontainer1

        // ensure the variables introduced in the attributes , i.e. in {..} , eg triggerFileName , have parameters eg string triggerFileName for binding

        [FunctionName("BlobTriggerFunctionApp2")]
        public static void Run(
            [BlobTrigger("cehcontainer1/DM682{triggerFileName}")]      Stream triggerFileBlob, string triggerFileName,
            [Blob("cehcontainer1/MappingFile.txt", FileAccess.Read)]     Stream mappingFileBlob,
            [Blob("cehcontainer2/DM682OUT{triggerFileName}", FileAccess.Write)]      Stream OutFileBlob,
        TraceWriter log)
        {
            log.Info($"C# Blob trigger function Processed blob\n Name:{triggerFileName} \n Size: {triggerFileBlob.Length} Bytes");


            (new funcApp()).run(triggerFileBlob, mappingFileBlob, OutFileBlob, triggerFileName);

        }

        private class funcApp
        {
            internal void run(Stream triggerFile, Stream mappingFile, Stream outFile, string filename)
            {

                //get Coda Codes 
                Dictionary<string, string> CodaCodes = LoadGlCodes(mappingFile);



                string codacode;


                using (StreamReader sr = new StreamReader(triggerFile))
                {
                    using (StreamWriter sw = new StreamWriter(outFile))
                    {
                        string paymentLine;
                        while ((paymentLine = sr.ReadLine()) != null)
                        {

                            //split the line
                            IList<String> fields = paymentLine.Split(',');

                            //valid line?
                            if (fields[0] == "2")
                            {
                                // bolt on the Coda Code
                                CodaCodes.TryGetValue(fields[1], out codacode);
                                if (codacode != null)
                                {
                                    paymentLine = paymentLine + "," + codacode;
                                }

                            }


                            Console.WriteLine(paymentLine);

                            sw.WriteLine(paymentLine);
                        }
                        outFile.Flush();
                    }

                }
            }

            private Dictionary<string, string> LoadGlCodes(Stream CmInFile)
            {

                Dictionary<string, string> cmd = new Dictionary<string, string>();
                IList<string> fields;


                using (StreamReader glr = new StreamReader(CmInFile))
                {
                    string glLine;
                    while ((glLine = glr.ReadLine()) != null)
                    {
                        fields = glLine.Split(',').ToList<string>();
                        cmd.Add(fields[0], fields[1]);
                    }

                }
                return cmd;
            }
        }

    }

}
