using System.Text;

namespace DataFileCreator
{
    public class DataFileService
    {
        private readonly static string _startupPath = Directory.GetCurrentDirectory();

        public static bool CreateFile(string tempFileName, int count)
        {
            try
            {
                // Create a file path within the data directory
                string filePath = Path.Combine(_startupPath, "data", tempFileName);

                // Create the file and write some text
                using (StreamWriter writer = new StreamWriter(filePath, true, Encoding.UTF8))
                {
                    string jsonFilePath = Path.Combine(_startupPath, "sample.json");
                    // Check if the file exists
                    if (File.Exists(jsonFilePath))
                    {
                        // Read the text from the file
                        string text = File.ReadAllText(jsonFilePath);
                        for (int i = 0; i < count; i++)
                        {
                            // Print the text to the file
                            writer.WriteLine(text);
                            writer.WriteLine(Environment.NewLine);
                        }
                    }
                }

                Console.WriteLine("File created at: " + filePath);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static bool DeleteFile(string dataFileName)
        {
            try
            {
                // Create a file path within the data directory
                string filePath = Path.Combine(_startupPath, "data", dataFileName);
                File.Delete(filePath);
                foreach (string chunkFile in Directory.EnumerateFiles(Path.Combine(_startupPath, "data"), "chunk_*.part"))
                {
                    File.Delete(chunkFile);
                }
                Console.WriteLine("File deleted at: " + filePath);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static void RenameFile(string tempFileName, string dataFileName)
        {
            string filePath = Path.Combine(_startupPath, "data", tempFileName);
            string dataFilePath = Path.Combine(_startupPath, "data", dataFileName);
            Console.WriteLine("Trying to rename File at: " + filePath);
            File.Move(filePath, dataFilePath);
            Console.WriteLine("File renamed to: " + dataFilePath);
        }

        public static bool FileExists(string fileName)
        {
            string filePath = Path.Combine(_startupPath, "data", fileName);
            if (File.Exists(filePath))
            {
                return true;
            }
            return false;
        }

        public static string FilePath(string fileName)
        {
            return Path.Combine(_startupPath, "data", fileName);
        }

        public static int ChunkFile(string fileName)
        {
            int chunkSize = 1024 * 1024; // 1 MB chunk size

            string filePath = Path.Combine(_startupPath, "data", fileName);
            byte[] buffer = new byte[chunkSize];
            int bytesRead;

            int chunkNumber = 1;
            using (FileStream inputFileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                while ((bytesRead = inputFileStream.Read(buffer, 0, chunkSize)) > 0)
                {
                    string outputFile = Path.Combine(_startupPath, "data", $"chunk_{chunkNumber}.part");
                    using (FileStream outputFileStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                    {
                        outputFileStream.Write(buffer, 0, bytesRead);
                    }
                    chunkNumber++;
                }
            }

            Console.WriteLine("File split successfully!");
            return chunkNumber - 1;
        }
    }
}