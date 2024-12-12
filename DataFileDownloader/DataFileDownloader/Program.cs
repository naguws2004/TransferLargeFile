// The port number must match the port of the gRPC server.
using DataFileGrpcService;
using Grpc.Core;
using Grpc.Net.Client;

string _startupPath = Directory.GetCurrentDirectory();

using var channel = GrpcChannel.ForAddress("https://localhost:7134");
var client = new FileTransferNotification.FileTransferNotificationClient(channel);

var fileStatus = await client.IsReadyForDownloadAsync(
                  new ClientMessage { Message = "Greet Client" });
Console.WriteLine("File: " + fileStatus.Found);

if (fileStatus.Found)
{
    var fileDetails = await client.StartDownloadAsync(
                      new ClientMessage { Message = "Greet Client" });
    Console.WriteLine("FileName: " + fileDetails.FileName);
    Console.WriteLine("FileSize: " + fileDetails.FileSizeBytes);
    Console.WriteLine("FileCount: " + fileDetails.NumParts);

    int chunkCount = fileDetails.NumParts;

    var transferClient = new FileTransfer.FileTransferClient(channel);
    for (int i = 1; i <= chunkCount; i++)
    {
        string filePath = Path.Combine(_startupPath, "data", $"chunk_{i}.part");
        // Process the received chunk
        using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
            using (var call = transferClient.DownloadFile(new DownloadFileRequest { Filename = $"chunk_{i}.part" }))
            {
                await foreach (var response in call.ResponseStream.ReadAllAsync())
                {
                    await fileStream.WriteAsync(response.Chunk.ToByteArray());
                }
            }
        }
    }
    Console.WriteLine("File chunks downloaded successfully!");

    int chunkSize = 1024 * 1024; // Assuming the same chunk size used for splitting
    string baseDirectory = Path.Combine(_startupPath, "data"); // Assuming the same directory for chunks
    string outputFilePath = Path.Combine(baseDirectory, "dataFile.dat"); // Output file path

    using (FileStream outputFileStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
    {
        for (int i = 1; i <= chunkCount; i++)
        {
            string chunkPath = Path.Combine(baseDirectory, $"chunk_{i}.part");

            using (FileStream chunkStream = new FileStream(chunkPath, FileMode.Open, FileAccess.Read))
            {
                byte[] buffer = new byte[chunkSize];
                int bytesRead;

                while ((bytesRead = chunkStream.Read(buffer, 0, chunkSize)) > 0)
                {
                    outputFileStream.Write(buffer, 0, bytesRead);
                }
            }
        }
    }

    Console.WriteLine("File combined successfully!");

    fileStatus = await client.EndDownloadAsync(
                      new ClientMessage { Message = "Greet Client" });
    if (!fileStatus.Found)
    {
        Console.WriteLine("File Deleted");
    }

    foreach (string chunkFile in Directory.EnumerateFiles(Path.Combine(_startupPath, "data"), "chunk_*.part"))
    {
        File.Delete(chunkFile);
    }
    Console.WriteLine("Chunk Files deleted");
}

Console.WriteLine("Press any key to exit...");
Console.ReadKey();

