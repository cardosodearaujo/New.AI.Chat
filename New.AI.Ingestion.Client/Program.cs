using New.AI.Ingestion.Client.Services;

namespace New.AI.Ingestion.Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var mainService = new MainService();
            await mainService.Process();
        }
    }
}
