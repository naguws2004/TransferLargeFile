using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace DataFileUploader
{
    public class FileTransferService : Greeter.GreeterBase
    {
        private readonly ILogger<FileTransferService> _logger;
        public FileTransferService(ILogger<FileTransferService> logger)
        {
            _logger = logger;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name
            });
        }
    }
}
