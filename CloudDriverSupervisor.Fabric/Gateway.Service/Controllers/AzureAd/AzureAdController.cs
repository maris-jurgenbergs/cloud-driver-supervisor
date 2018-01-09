namespace Gateway.Service.Controllers.AzureAd
{
    using System.Threading.Tasks;
    using Gateway.Service.AzureAd.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [Authorize]
    public class AzureAdController : Controller
    {
        private readonly IAzureActiveDirectoryService _azureActiveDirectoryService;

        public AzureAdController(IAzureActiveDirectoryService azureActiveDirectoryService)
        {
            _azureActiveDirectoryService = azureActiveDirectoryService;
        }

        [HttpGet("users/{searchValue}")]
        public async Task<ActionResult> Get([FromRoute] string searchValue)
        {
            var users = await _azureActiveDirectoryService.GetUsers();
            return Ok(users);
        }
    }
}