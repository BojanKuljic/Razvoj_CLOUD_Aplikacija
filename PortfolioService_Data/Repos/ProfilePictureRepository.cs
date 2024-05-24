using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Reflection;
using System.Web;
using System.IO;

namespace PortfolioServiceStorage {
    public class ProfilePictureRepository {
        private CloudStorageAccount storageAccount;
        private CloudBlobContainer container;

        public ProfilePictureRepository() {
            storageAccount = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
            CloudBlobClient blobStorage = storageAccount.CreateCloudBlobClient();
            container = blobStorage.GetContainerReference("profilepictures");
            container.CreateIfNotExists();

            container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
        }

        public string Create(string email, HttpPostedFileBase picture) {
            string uniqueBlobName = string.Format("pfp_{0}", email);
            CloudBlockBlob blob = container.GetBlockBlobReference(uniqueBlobName);

            blob.Properties.ContentType = picture.ContentType;
            blob.UploadFromStream(picture.InputStream);

            return blob.Uri.ToString();
        }

        public string GetUri(string email) {
            CloudBlockBlob blob = container.GetBlockBlobReference(string.Format("pfp_{0}", email));
            return blob.Uri.ToString();
        }

        public void UpdateUri(string oldEmail, string newEmail) {
            CloudBlockBlob oldBlob = container.GetBlockBlobReference(string.Format("pfp_{0}", oldEmail));
            CloudBlockBlob newBlob = container.GetBlockBlobReference(string.Format("pfp_{0}", newEmail));

            newBlob.StartCopyFromBlob(oldBlob);
            oldBlob.Delete();
        }

        public void Delete(string email) {
            CloudBlockBlob blob = container.GetBlockBlobReference(string.Format("pfp_{0}", email));
            blob.Delete();
        }
    }
}
