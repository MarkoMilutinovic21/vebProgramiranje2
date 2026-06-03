using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Shared.DTOs;
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

            return new ValidationResultDto { IsValid = true, Message = "Valid." };
        }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return this.CreateServiceRemotingInstanceListeners();
        }
    }
}