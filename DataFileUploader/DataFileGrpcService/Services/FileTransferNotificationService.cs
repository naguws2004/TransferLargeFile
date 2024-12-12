using DataFileCreator;
using DataFileGrpcService.Common;
using Grpc.Core;

namespace DataFileGrpcService.Services
{
    public class FileTransferNotificationService : FileTransferNotification.FileTransferNotificationBase
    {
        private readonly ILogger<FileTransferNotificationService> _logger;

        public FileTransferNotificationService(ILogger<FileTransferNotificationService> logger)
        {
            _logger = logger;
        }

        public override Task<FileFound> IsReadyForDownload(ClientMessage request, ServerCallContext context)
        {
            string dataFilePath = DataFileService.FilePath(Constants.DataFileName);
            var found = new FileFound
            {
                Found = true
            };
            var notfound = new FileFound
            {
                Found = false
            };
            if (DataFileService.FileExists(dataFilePath))
            {
                return Task.FromResult(found);
            }
            return Task.FromResult(notfound);
        }

        public override Task<FileDetails> StartDownload(ClientMessage request, ServerCallContext context)
        {
            string dataFilePath = DataFileService.FilePath(Constants.DataFileName);
            FileInfo fileInfo = new FileInfo(dataFilePath);
            long byteSize = fileInfo.Length;
            int parts = DataFileService.ChunkFile(Constants.DataFileName);
            return Task.FromResult(new FileDetails
            {
                FileName = "chunk_{chunkNumber}.part",
                FileSizeBytes = byteSize,
                NumParts = parts
            });
        }

        public override Task<FileFound> EndDownload(ClientMessage request, ServerCallContext context)
        {
            string dataFilePath = DataFileService.FilePath(Constants.DataFileName);
            var deleted = DataFileService.DeleteFile(dataFilePath);
            var found = new FileFound
            {
                Found = true
            };
            var notfound = new FileFound
            {
                Found = false
            };
            if (!DataFileService.FileExists(dataFilePath))
            {
                return Task.FromResult(notfound);
            }
            return Task.FromResult(found);
        }
    }
}
