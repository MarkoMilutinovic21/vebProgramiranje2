using Microsoft.ServiceFabric.Services.Remoting;
using Shared.DTOs;

namespace Shared.Interfaces
{
    public interface IBankService : IService
    {
        Task<bool> CheckBalanceAsync(string userId, decimal amount);
        Task<bool> DeductBalanceAsync(string userId, decimal amount);
        Task<UserAccountDto> GetUserAccountAsync(string userId);
    }
}