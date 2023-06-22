
namespace CategoryApi.Services
{

    public interface INotifyService
    {
        void Send<T>(T @event) where T : IntegrationEvent;
        Task SendAsync<T>(T @event) where T : IntegrationEvent;
    }
    
    
}