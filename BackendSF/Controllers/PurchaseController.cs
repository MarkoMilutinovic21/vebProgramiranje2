using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Shared.DTOs;
using Shared.Interfaces;

namespace BackendSF.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchaseController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Purchase([FromBody] PurchaseRequestDto request)
        {
            var proxy = ServiceProxy.Create<IValidatorService>(
                new Uri("fabric:/BookStoreApp/Validator")
            );

            var result = await proxy.ValidatePurchaseAsync(request);

            if (!result.IsValid)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }
    }
}