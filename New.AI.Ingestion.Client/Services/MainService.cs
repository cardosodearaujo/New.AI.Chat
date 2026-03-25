namespace New.AI.Ingestion.Client.Services
{
    public class MainService
    {
        public async Task Process()
        {
            Console.WriteLine("Informe a pasta onde consta os arquivos que devem ser ingeridos:");
            var folderPath = Console.ReadLine();
            await new ReadFilesAndSendToIngestionService().Process(folderPath);
            Console.WriteLine("Programa encerrado.");
            Console.ReadKey();
        }
    }
}
