using AutoMapper;
using CommandService.Models;
using CommandService.Proto;
using Grpc.Net.Client;

namespace CommandService.SyncDataService.Grpc
{
    public class PlatformDataClient : IPlatformDataClient
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public PlatformDataClient(IConfiguration configuration,IMapper mapper)
        {
            _configuration = configuration;
            _mapper = mapper;
        }
        public IEnumerable<Platform> ReturnAllPlatform()
        {
            var grpcPlatformAddress = _configuration["GrpcPlatform"];
            if (string.IsNullOrEmpty(grpcPlatformAddress))
            {
                throw new ArgumentNullException(nameof(grpcPlatformAddress), "GRPC Platform address is not configured.");
            }

            Console.WriteLine($"--> Calling GRPC Service {grpcPlatformAddress}");
            var channel = GrpcChannel.ForAddress(grpcPlatformAddress);
            var client = new GrpcPlatform.GrpcPlatformClient(channel);
            var request = new GetAllRequest();
            try
            {
                var reply = client.GetPlatform(request);
                return _mapper.Map<IEnumerable<Platform>>(reply.Platforms);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not call GRPC Server kk {ex.Message}");
                return null;
            }
        }
    }
}
