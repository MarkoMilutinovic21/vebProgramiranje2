using Microsoft.ServiceFabric.Services.Remoting;
using Shared.DTOs;

namespace Shared.Interfaces
{
    public interface IValidatorService : IService
    {
        Task<ValidationResultDto> ValidatePurchaseAsync(PurchaseRequestDto request);
    }
}