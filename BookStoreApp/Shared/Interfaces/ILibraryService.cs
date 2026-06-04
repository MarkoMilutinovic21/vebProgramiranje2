using Microsoft.ServiceFabric.Services.Remoting;
using Shared.DTOs;

namespace Shared.Interfaces
{
    public interface ILibraryService : IService
    {
        Task<BookDto> GetBookAsync(string bookId);
        Task<bool> CheckAvailabilityAsync(string bookId, int quantity);
        Task<bool> ReduceStockAsync(string bookId, int quantity);
    }
}