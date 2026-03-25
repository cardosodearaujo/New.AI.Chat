namespace New.AI.Ingestion.Client.Services
{
    public class ReadFilesService
    {
        private List<FileInfo> ListOfFiles = new List<FileInfo>();

        private readonly string[] _extensions = { ".cs", ".pas" };
            /*{
            ".cs",  ".aspx", ".xml", ".xsd", ".csproj", ".json", ".ascx", ".resx", ".mxm", 
            ".sln", ".ts", ".html", ".scss" , ".js", ".pas", ".dof", ".master", ".css", ".sql"};*/

        private int _interator = 0;

        public async Task<List<FileInfo>> Process(string folderPath, bool print = false)
        {
            Read(folderPath, print);

            ListOfFiles = ListOfFiles.OrderByDescending(I => I.Length).ToList();

            return ListOfFiles;
        }

        private void Read(string folderPath, bool print = false)
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
                        if (print) Console.WriteLine($"{_interator} - {file}");
                        ListOfFiles.Add(fileInfo);
                    }
                }

                foreach (var directory in Directory.GetDirectories(folderPath))
                {
                    Read(directory, print);
                }
            }
        }
    }
}
