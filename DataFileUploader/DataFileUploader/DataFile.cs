namespace DataFileUploader
{
    internal class DataFile
    {
        private readonly static string _startupPath = Directory.GetCurrentDirectory();

        public static bool CreateFile(string tempFileName, int count)
        {
            try
            {
                // Create a file path within the data directory
                string filePath = Path.Combine(_startupPath, "data", tempFileName);

                // Create the file and write some text
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    string jsonFilePath = Path.Combine(_startupPath, "sample.json");
                    // Check if the file exists
                    if (File.Exists(jsonFilePath))
                    {
                        // Read the text from the file
                        string text = File.ReadAllText(filePath);
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
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public static void RenameFile(string tempFileName, string dataFileName)
        {
            string filePath = Path.Combine(_startupPath, "data", tempFileName);
            string dataFilePath = Path.Combine(_startupPath, "data", dataFileName);
            File.Move(filePath, dataFilePath);
        }
    }
}