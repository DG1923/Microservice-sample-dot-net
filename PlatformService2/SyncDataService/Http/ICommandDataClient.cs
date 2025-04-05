using PlatformService2.DTOs;

namespace PlatformService2.SyncDataService.Http
{
    public interface ICommandDataClient
    {
        Task SendPlatformToCommand(PlatformReadDTO platform);
    }
}
