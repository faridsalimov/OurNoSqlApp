using AzureStorageLibrary;
using AzureStorageLibrary.Services;
using System.Text;

namespace QueueConsoleApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            GetData();   
            Console.ReadLine();
        }

        static async void GetData()
        {
            ConnectionStrings.AzureStorageConnectionString = "";

            AzQueue queue = new AzQueue("testqueue");

            string base64message = Convert.ToBase64String(Encoding.UTF8.GetBytes("Salam Amin"));

            var result = await queue.RetrieveNextMessageAsync();
            var message = Encoding.UTF8.GetString(Convert.FromBase64String(result.MessageText));
            Console.WriteLine(message);

            await queue.DeleteMessage(result.MessageId, result.PopReceipt);
            await Console.Out.WriteLineAsync("Message delete from queue");
        }
    }
}
