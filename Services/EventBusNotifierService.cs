
namespace CategoryApi.Services
{
    public class EventBusNotifierService : INotifyService
    {
        private readonly ILogger<EventBusNotifierService> logger;
        private readonly IEventBus bus;

        public EventBusNotifierService(ILogger<EventBusNotifierService> logger, IEventBus bus)
        {
            this.logger = logger;
            this.bus = bus;
        }
        public void Send<T>(T @event) where T : IntegrationEvent
        {
            var eventName = typeof(T).Name;
            if (@event is null)
            {
                logger.LogError("App ({AppName}) did not received ({EventName}) , this evetn has not content",
                Program.Name,
                eventName);
                return;
            }

            try
            {
                logger.LogInformation("App ({AppName}) try to send event ({EventName}) , with id ({EventId}) , CreatedAt ({When})",
                Program.Name,
                eventName,
                @event.Id,
                @event.CreationDate);
                bus.Publish(@event);
                logger.LogInformation("App ({AppName}) has sent event ({EventName}) , with id ({EventId}) , CreatedAt ({When})",
                Program.Name,
                eventName,
                @event.Id,
                @event.CreationDate);
            }
            catch (System.Exception exp)
            {
                logger.LogError(exp, "App ({AppName}) could not publish event ({EventName}) , with id ({EventId}) , CreatedAt ({When})",
                Program.Name,
                @event.CreationDate,
                @event.Id);
            }
        }

        public async Task SendAsync<T>(T @event) where T : IntegrationEvent
        {
            Send(@event);
            await Task.CompletedTask;
        }
    }
}