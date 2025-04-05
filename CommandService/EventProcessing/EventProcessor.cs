using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using CommandService.Models;
using System.Text.Json;

namespace CommandService.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMapper _mapper;

        public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
        {
            _scopeFactory = scopeFactory;
            _mapper = mapper;
        }
        public void ProcessEvent(string message)
        {
            var eventType = DetermindEvent(message);
            switch (eventType)
            {
                case EventType.PlatformPublished:
                    Console.WriteLine("--> Platform published event detected");
                    addPlatform(message);
                    break;
                default:
                    Console.WriteLine("--> Could not determine event type");
                    break;
            }
        }

        private void addPlatform(string platformPublishedMessage)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();
                var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishDto>(platformPublishedMessage);
                try
                {
                    var platform = _mapper.Map<Platform>(platformPublishedDto);
                    if (!repo.ExternalPlatformExists(platform.ExternalID))
                    {
                        repo.CreatePlatform(platform);
                        repo.SaveChanges();
                        Console.WriteLine("--> Platform added to DB");
                    }
                    else
                    {
                        Console.WriteLine("--> Platform already exists in DB");
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"--> Could not add platform to DB {ex.Message}");
                }
            }
        }

        private EventType DetermindEvent(string notificationMessage)
        {
            Console.WriteLine("--> Determining event type...");
            var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);
            switch(eventType.Event)
            {
                case "Platform_Published":
                    Console.WriteLine("--> Platform published event detected");
                    return EventType.PlatformPublished;
                default:
                    Console.WriteLine("--> Could not determine event type");
                    return EventType.Udetermined;
            }
        }
        enum EventType
        {
            PlatformPublished,
            Udetermined

        }

      
    }
}
