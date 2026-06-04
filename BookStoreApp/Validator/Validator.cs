using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Shared.DTOs;
using Shared.Events;
using Shared.Interfaces;
using System.Fabric;

namespace Validator
{
    internal sealed class Validator : StatelessService, IValidatorService
    {
        public Validator(StatelessServiceContext context)
            : base(context) { }

        public async Task<ValidationResultDto> ValidatePurchaseAsync(PurchaseRequestDto request)
        {
            if (request.Quantity <= 0)
                return new ValidationResultDto { IsValid = false, Message = "Quantity must be greater than 0." };

            var bankService = ServiceProxy.Create<IBankService>(
                new Uri("fabric:/BookStoreApp/BankService")
            );

            var libraryService = ServiceProxy.Create<ILibraryService>(
                new Uri("fabric:/BookStoreApp/LibraryService")
            );

            var book = await libraryService.GetBookAsync(request.BookId);
            if (book == null)
                return new ValidationResultDto { IsValid = false, Message = "Book not found." };

            var totalPrice = book.Price * request.Quantity;

            var hasBalance = await bankService.CheckBalanceAsync(request.UserId, totalPrice);
            if (!hasBalance)
                return new ValidationResultDto { IsValid = false, Message = "Insufficient funds." };

            var isAvailable = await libraryService.CheckAvailabilityAsync(request.BookId, request.Quantity);
            if (!isAvailable)
                return new ValidationResultDto { IsValid = false, Message = "Not enough books in stock." };

            await bankService.DeductBalanceAsync(request.UserId, totalPrice);
            await libraryService.ReduceStockAsync(request.BookId, request.Quantity);

            var eventDispatcher = ServiceProxy.Create<IEventDispatcherService>(
                new Uri("fabric:/BookStoreApp/EventDispatcherService")
            );

            var purchaseEvent = new PurchaseEvent
            {
                UserId = request.UserId,
                BookId = request.BookId,
                BookTitle = book.Title,
                Quantity = request.Quantity,
                TotalPrice = totalPrice,
                Email = request.Email,
                Timestamp = DateTime.UtcNow
            };

            _ = eventDispatcher.PublishAsync(purchaseEvent);

            return new ValidationResultDto { IsValid = true, Message = "Purchase successful." };
        }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return this.CreateServiceRemotingInstanceListeners();
        }
    }
}