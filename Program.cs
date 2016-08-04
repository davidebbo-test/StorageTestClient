using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;

namespace StorageTestClient
{
    class Program
    {
        static CloudStorageAccount _storageAccount;
        static CloudBlobClient _blobClient;
        static CloudQueueClient _queueClient;

        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Syntax:");
                Console.WriteLine("  StorageTestClient ClearQueue myqueue --> delete all items in the queue");
                Console.WriteLine("  StorageTestClient PopulateQueue myqueue 10 --> Add 10 items to the queue");
                
                return;
            }

            string command = args[0];
            string arg = args[1];

            string connectionString = System.Configuration.ConfigurationManager.AppSettings["StorageConnectionString"];

            _storageAccount = CloudStorageAccount.Parse(connectionString);
            _blobClient = _storageAccount.CreateCloudBlobClient();
            _queueClient = _storageAccount.CreateCloudQueueClient();

            switch (command.ToLowerInvariant())
            {
                case "clearqueue":
                    ClearQueue(arg);
                    break;
                case "populatequeue":
                    CreateTestQueueMessages(arg, Int32.Parse(args[2]));
                    break;
                default:
                    Console.WriteLine($"Unknown command '{command}'");
                    return;
            }
        }

        private static void CreateTestBlobs(string containerName, int count)
        {
            var container = _blobClient.GetContainerReference(containerName);

            for (int i = 0; i < count; i++)
            {
                var blob = container.GetBlockBlobReference(Guid.NewGuid().ToString());
                blob.UploadText("Test Data");
            }
        }

        static void ClearQueue(string queueName)
        {
            CloudQueue queue = _queueClient.GetQueueReference(queueName);
            queue.CreateIfNotExists();
            queue.Clear();
        }

        private static void CreateTestQueueMessages(string queueName, int num)
        {
            CloudQueue queue = _queueClient.GetQueueReference(queueName);
            queue.CreateIfNotExists();

            for (int i = 0; i < num; i++)
            {
                var message = new CloudQueueMessage("Test message " + i);
                queue.AddMessage(message);
            }
        }
    }
}
