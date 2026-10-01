using AutoMapper;
using CommandService.Models;
using Grpc.Net.Client;

namespace CommandService.SyncDataServices.Grpc
{
    public class PlatformDataClient : IPlatformDataClient
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public PlatformDataClient(IMapper mapper, IConfiguration configuration)
        {
            _mapper = mapper;
            _configuration = configuration;
        }
        public async Task<IEnumerable<Platform>> ReturnAllPlatforms()
        {
            var grpcServiceUrl = _configuration["GrpcPlatform"] ?? throw new ArgumentNullException("GrpcPlatform Url");

            Console.WriteLine($"--> Calling GRPC Service to get all platforms from ${grpcServiceUrl}");
            
            using var channel = GrpcChannel.ForAddress(grpcServiceUrl);
            var client = new GrpcPlatform.GrpcPlatformClient(channel);

            var request = new GetAllRequest();

            try
            {
               var response = await client.GetAllPlatformsAsync(request);

                Console.WriteLine($"--> Received {response.Platforms.Count()} platforms from GRPC Service");

                return response.Platforms.Select(p => _mapper.Map<Platform>(p));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error to call GRPC Service: {ex?.Message}");  
                return Enumerable.Empty<Platform>();
            }
        }
    }

}
