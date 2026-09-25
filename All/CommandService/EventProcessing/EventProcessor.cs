using AutoMapper;
using CommandService.Dtos;
using CommandService.Models;
using CommandService.Repository;
using System.Text.Json;

namespace CommandService.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMapper _mapper;
        private const string eventTypePlatformPublished = "Platform_Published";
        public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
        {
            _scopeFactory = scopeFactory;
            _mapper = mapper;
        }
        
        public void ProcessEvent(string message)
        {
            // Deserialize the message to determine the event type
            var eventType = DetermineEventType(message);

            switch (eventType)
            {
                case EventType.PlatformPublished:
                    AddPlatform(message);
                    break;
                default:
                    Console.WriteLine("--> Could not determine the event type");
                    break;
            }
        }
        private EventType DetermineEventType(string message)
        {
            Console.WriteLine("--> Determining Event Type");

            var eventType = JsonSerializer.Deserialize<GenericEventDto>(message);

            if (eventType == null) { throw new ArgumentNullException(nameof(eventType)); }

            switch (eventType.Event)
            {
                case eventTypePlatformPublished:
                    Console.WriteLine("--> Platform Published Event Detected");
                    return EventType.PlatformPublished;
                default:
                    Console.WriteLine("--> Could not determine the event type");
                    return EventType.Undetermined;
            }
        }
        private void AddPlatform(string platformPublishedMessage)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<ICommandRepository>();
                var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);
                try
                {
                    var plat = _mapper.Map<Platform>(platformPublishedDto);
                    Console.WriteLine("--> Start to add Platform to DB...");

                    if (!repo.ExternalPlatformExists(plat.ExternalID))
                    {
                        repo.CreatePlatform(plat);
                        repo.SaveChanges();
                        Console.WriteLine("--> Platform added!");
                    }
                    else
                    {
                        Console.WriteLine("--> Platform already exists...");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"--> Could not add Platform to DB: {ex.Message}");
                }
            }
        }
    }
}
