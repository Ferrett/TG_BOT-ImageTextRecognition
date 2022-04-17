using System.IO;
using System.Net;
using Google.Cloud.Storage.V1;

namespace ConsoleApp12
{
    public static class WorkWithWeb
    {
        private static readonly string bucketName = "prikhod228";
        private static StorageClient client = StorageClient.Create();

        public static int CountFilesInStorage()
        {
            int filesNumber = 1;
            foreach (var obj in client.ListObjects(bucketName, ""))
            {
                filesNumber++;
            }
            return filesNumber;
        }
        public static void DownloadFilesFromPath(string download_url,int filesNumber)
        {
            using (var download = new WebClient())
            {
                if (!Directory.Exists("files"))
                    Directory.CreateDirectory("files");

                download.DownloadFile(download_url, $"files\\file-{filesNumber}.jpg");
            }
        }
        public static void UploadFilesToCLoud(string url, int filesNumber)
        {
            string objectName = $"file-{filesNumber}.jpg";

            using var fileStream = System.IO.File.OpenRead(url);
            client.UploadObject(bucketName, objectName, $"image/jpg", fileStream);

            MakeFilePublic.MakePublic(bucketName, objectName);
        }

        public static string GetTextFromCoreApi(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

    }
}
