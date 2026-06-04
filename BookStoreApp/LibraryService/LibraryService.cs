using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Shared.DTOs;
using Shared.Interfaces;
using System.Fabric;

namespace LibraryService
{
    internal sealed class LibraryService : StatefulService, ILibraryService
    {
        public LibraryService(StatefulServiceContext context)
            : base(context) { }

        public async Task<BookDto> GetBookAsync(string bookId)
        {
            var books = await StateManager.GetOrAddAsync<IReliableDictionary<string, BookDto>>("books");

            using var tx = StateManager.CreateTransaction();
            var result = await books.TryGetValueAsync(tx, bookId);
            return result.HasValue ? result.Value : null;
        }

        public async Task<bool> CheckAvailabilityAsync(string bookId, int quantity)
        {
            var book = await GetBookAsync(bookId);
            return book != null && book.Quantity >= quantity;
        }

        public async Task<bool> ReduceStockAsync(string bookId, int quantity)
        {
            var books = await StateManager.GetOrAddAsync<IReliableDictionary<string, BookDto>>("books");

            using var tx = StateManager.CreateTransaction();
            var result = await books.TryGetValueAsync(tx, bookId);

            if (!result.HasValue || result.Value.Quantity < quantity)
                return false;

            var book = result.Value;
            book.Quantity -= quantity;
            await books.SetAsync(tx, bookId, book);
            await tx.CommitAsync();
            return true;
        }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return this.CreateServiceRemotingReplicaListeners();
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            var books = await StateManager.GetOrAddAsync<IReliableDictionary<string, BookDto>>("books");

            using var tx = StateManager.CreateTransaction();
            await books.TryAddAsync(tx, "book1", new BookDto { BookId = "book1", Title = "Clean Code", Quantity = 10, Price = 25.99m });
            await books.TryAddAsync(tx, "book2", new BookDto { BookId = "book2", Title = "Design Patterns", Quantity = 5, Price = 35.99m });
            await tx.CommitAsync();
        }
    }
}