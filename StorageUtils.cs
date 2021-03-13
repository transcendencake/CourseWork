using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CourseWork
{
    public static class StorageUtils
    {
        private static BlobContainerClient container;
        public static void SetDefaults(string connectionString, string containerName)
        {
            container = new BlobContainerClient(connectionString, containerName);
        }
        public static bool AddPicture(string name, FileStream picture)
        {
            BlobClient blob = container.GetBlobClient(name);
            try
            {
                blob.Upload(picture);
            }
            catch (Exception)
            {

                return false;
            }
            return true;
        }
    }
}
