using New.AI.Ingestion.Client.DTOs;
using System.Net.Http.Json;

namespace New.AI.Ingestion.Client.Services
{
    public class ReadFilesAndSendToIngestionService
    {
        private const int PAGE_LENGTH = 100;

        private const string URL_BASE = "https://localhost:7269";
        private string DateNow { get => DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"); }

        private readonly HttpClient _httpClient = new HttpClient
        {
            BaseAddress = new Uri(URL_BASE),
            Timeout = Timeout.InfiniteTimeSpan
        };

        public async Task Process(string folderPath)
        {
            var loteCount = 0;

            var listOfFiles = await new ReadFilesService().Process(folderPath, true);

            if (listOfFiles != null && listOfFiles.Any())
            {  
                var lotes = listOfFiles.Chunk(PAGE_LENGTH).ToList();
                var totalLotes = lotes.Count();

                foreach (var lote in lotes)
                {
                    var ingestion = new IngestionDTO();

                    foreach (var fileInfo in lote)
                    {
                        var fileBytes = await File.ReadAllBytesAsync(fileInfo.FullName);

                        ingestion.IngestionFiles.Add(
                            new IngestionFileDTO
                            {
                                FileName = fileInfo.Name,
                                Format = fileInfo.Extension,
                                Size = fileInfo.Length,                                
                                ContentText = Convert.ToBase64String(fileBytes)
                            });
                    }

                    loteCount++;

                    var percentual = (loteCount * 100) / totalLotes;

                    Console.WriteLine($"{DateNow} - Enviando lote {loteCount} de {totalLotes} - {percentual}%.");

                    var response = await _httpClient.PostAsJsonAsync("/api/Ingestion", ingestion);

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"{DateNow} - Sucesso! {loteCount} lotes processados.");
                    }
                    else
                    {
                        string error = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"{DateNow} - Falha ao enviar ({response.StatusCode}): {error}");
                    }
                }
            }
        }
    }
}
