using PlatformService2.DTOs;

namespace PlatformService2.AsynDataService
{
    public interface IMessageBus
    {
        void PublishNewPlatform(PlatformPublishDTO platformPublishDto);
    }
}
