using AutoMapper;
using Grpc.Core;
using PlatformService2.Data;
using PlatformService2.Proto;

namespace PlatformService2.SyncDataService.Grpc
{
    public class GrpcPlatformServicecs : GrpcPlatform.GrpcPlatformBase
    {
        private readonly IPlatformRepo _repo;
        private readonly IMapper _mapper;

        public GrpcPlatformServicecs(IPlatformRepo repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;

        }
        //wrong named for GetPlatform , should be GetAllPlatform
        public override Task<PlatformResponse> GetPlatform(GetAllRequest request, ServerCallContext context)
        {
            var response = new PlatformResponse();
            var platforms = _repo.GetAllPlatforms();
            foreach (var platform in platforms)
            {
                response.Platforms.Add(_mapper.Map<GrpcPlatformModel>(platform));
            }
            return Task.FromResult(response);

        }
    }
}
