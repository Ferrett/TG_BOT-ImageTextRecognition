using System;

using System.Collections.Generic;
using System.IO;
using Google.Apis;
using Google.Apis.Analytics.v3;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Vision.V1;
using Newtonsoft.Json;
using Google.Apis.Services;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using System.Threading.Tasks;
using System.Threading;
using System.Net;


using System.Net.Http;
using Google.Cloud.Storage.V1;
using System.Text;

namespace ConsoleApp12
{

    class Program
    {
        private static readonly string token = "5394308742:AAHPyiyJfj9vHIxmPjOzNydUZm0HD3SHVpE";
        static TelegramBotClient Bot = new TelegramBotClient(token);

        private static readonly HttpClient client = new HttpClient();
        static void Main(string[] args)
        {
           

            Bot.StartReceiving(updateHandler, errorHandler);
            Console.ReadLine();
        }

        private static int filesNumber = 0;
        private static string bucketName = "prikhod228";

        private static async Task updateHandler(ITelegramBotClient bot, Update update, CancellationToken arg3)
        {
            if (update.Type == UpdateType.Message)
            {
                if (update.Message.Type == MessageType.Photo)
                {
                    System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", $@"C:\Users\User\source\repos\ConsoleApp12\bin\Debug\netcoreapp3.1" + @"\info.json");
                    var client = StorageClient.Create();
                    foreach (var obj in client.ListObjects(bucketName, ""))
                    {
                        filesNumber++;
                    }

                    var file = Bot.GetFileAsync(update.Message.Photo[update.Message.Photo.Count() - 1].FileId);
                    var download_url = @"https://api.telegram.org/file/bot5394308742:AAHPyiyJfj9vHIxmPjOzNydUZm0HD3SHVpE/" + file.Result.FilePath;

                    using (var download = new WebClient())
                    {
                        download.DownloadFile(download_url, $"files\\file-{filesNumber}.jpg");
                    }

                    DownloadFilesToCLoud($"files\\file-{filesNumber}.jpg", client);

                    string text = GetTextFromApi($"http://prikhod160422-001-site1.dtempurl.com/Text?url=https://storage.googleapis.com/prikhod228/file-{filesNumber}.jpg");
                    await Bot.SendTextMessageAsync(update.Message.Chat.Id,text );
                }
            }
        }

        public static void DownloadFilesToCLoud(string url, StorageClient client)
        {
            string objectName = $"file-{filesNumber}.jpg";

            using var fileStream = System.IO.File.OpenRead(url);
            client.UploadObject(bucketName, objectName, $"image/jpg", fileStream);

            MakePublic.MakePublicFile(bucketName, objectName);
        }

        public static string GetTextFromApi(string uri)
        {
           // uri = "http://prikhod160422-001-site1.dtempurl.com/Text?url=https://storage.googleapis.com/prikhod228/file-0.jpg";
             HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        private static Task errorHandler(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
        {
            throw new NotImplementedException();
        }
    }
}
