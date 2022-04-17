using System;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Http;
using Google.Cloud.Storage.V1;

namespace ConsoleApp12
{
    class Program
    {
        private static readonly string token = "5394308742:AAHPyiyJfj9vHIxmPjOzNydUZm0HD3SHVpE";
        static TelegramBotClient Bot = new TelegramBotClient(token);

        static void Main(string[] args)
        {
            System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "info.json");
            Bot.StartReceiving(updateHandler, errorHandler);
            Console.ReadLine();
        }

        private static async Task updateHandler(ITelegramBotClient bot, Update update, CancellationToken arg3)
        {
            if (update.Type == UpdateType.Message)
            {
                if (update.Message.Type == MessageType.Photo)
                {
                    var download_url = @$"https://api.telegram.org/file/bot5394308742:AAHPyiyJfj9vHIxmPjOzNydUZm0HD3SHVpE/{Bot.GetFileAsync(update.Message.Photo[update.Message.Photo.Count() - 1].FileId).Result.FilePath}";
                    int filesNumber = WorkWithWeb.CountFilesInStorage();

                    WorkWithWeb.DownloadFilesFromPath(download_url, filesNumber);
                    WorkWithWeb.UploadFilesToCLoud($"files\\file-{filesNumber}.jpg", filesNumber);

                    string text = WorkWithWeb.GetTextFromCoreApi($"http://prikhod160422-001-site1.dtempurl.com/Text?url=https://storage.googleapis.com/prikhod228/file-{filesNumber}.jpg");
                    await Bot.SendTextMessageAsync(update.Message.Chat.Id,text );
                }
            }
        }

        private static Task errorHandler(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
        {
            throw new NotImplementedException();
        }
    }
}
