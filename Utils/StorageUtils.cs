using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using CourseWork.Models;
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
        public static bool AddPicture(string name, Stream picture)
        {
            BlobClient blob = container.GetBlobClient(name);
            blob.DeleteIfExists(DeleteSnapshotsOption.IncludeSnapshots);
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
        public static string GetPictureUri(string name)
        {
            if (name == null) return null;
            return container.GetBlobClient(name).Uri.AbsoluteUri;
        }
        private static void DeleteBlob(string name)
        {
            BlobClient blob = container.GetBlobClient(name);
            blob.DeleteIfExists(DeleteSnapshotsOption.IncludeSnapshots);
        }
        public static void DeleteChapterPic(Chapter chapter)
        {
            if (chapter.PicturePath != null)
                DeleteBlob(chapter.PicturePath);
        }
    }
}
