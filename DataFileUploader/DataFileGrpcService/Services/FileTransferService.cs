using DataFileCreator;
using Google.Protobuf;
using Grpc.Core;

namespace DataFileGrpcService.Services
{
    public class FileTransferService : FileTransfer.FileTransferBase
    {
        public override async Task DownloadFile(DownloadFileRequest request, IServerStreamWriter<DownloadFileResponse> responseStream, ServerCallContext context)
        {
            string filePath = DataFileService.FilePath(request.Filename);
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                byte[] buffer = new byte[1024];
                int bytesRead;

                while ((bytesRead = await fs.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await responseStream.WriteAsync(new DownloadFileResponse { Chunk = ByteString.CopyFrom(buffer.Take(bytesRead).ToArray()) });
                }
            }
        }
    }
}
