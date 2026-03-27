using System.Net.Http.Json;

namespace New.AI.Ingestion.Client.Services
{
    public class ReadFilesService
    {
        private List<FileInfo> ListOfFiles = new List<FileInfo>();

        private readonly string[] _extensions = { ".cs", ".pas" };

        private int _interator = 0;

        private const string URL_BASE = "https://localhost:7269";

        private readonly HttpClient _httpClient = new HttpClient
        {
            BaseAddress = new Uri(URL_BASE),
            Timeout = Timeout.InfiniteTimeSpan
        };

        public async Task<List<FileInfo>> Process(string folderPath, bool print = false)
        {
            await Read(folderPath, print);

            ListOfFiles = ListOfFiles.OrderByDescending(I => I.Length).ToList();

            return ListOfFiles;
        }

        private async Task Read(string folderPath, bool print = false)
        {
            if (Directory.Exists(folderPath))
            {
                var files = Directory.GetFiles(folderPath);
                foreach (var file in files)
                {
                    var fileInfo = new FileInfo(file);
                    var content = File.ReadAllText(file);

                    if (fileInfo.Length > 0
                        && _extensions.Contains(fileInfo.Extension.ToLowerInvariant())
                        && !string.IsNullOrEmpty(content))
                    {
                        _interator++;

                        var exists = await CheckExistsAsync(fileInfo.Name);

                        if (exists)
                        {
                            if (print) Console.WriteLine($"{_interator} - IGNORADO (já existe) - {file}");
                            continue;
                        }
                        
                        if (print) Console.WriteLine($"{_interator} - {file}");
                        ListOfFiles.Add(fileInfo);
                    }
                }

                foreach (var directory in Directory.GetDirectories(folderPath))
                {
                    await Read(directory, print);
                }
            }
        }

        private async Task<bool> CheckExistsAsync(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return false;

            try
            {
                var response = await _httpClient.GetAsync($"/api/File/exists?FileName={Uri.EscapeDataString(fileName)}");

                if (!response.IsSuccessStatusCode) return false;

                var exists = await response.Content.ReadFromJsonAsync<bool>();

                return exists;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
