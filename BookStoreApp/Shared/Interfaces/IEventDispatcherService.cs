using Microsoft.ServiceFabric.Services.Remoting;
using Shared.Events;

namespace Shared.Interfaces
{
    public interface IEventDispatcherService : IService
    {
        Task PublishAsync(PurchaseEvent purchaseEvent);
    }
}