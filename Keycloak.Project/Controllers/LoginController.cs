using Keycloak.Project.Interfaces;
using Keycloak.Project.Models;
using Microsoft.AspNetCore.Mvc;

namespace Keycloak.Project.Controllers
{
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IKeyCloakClient _dataClient;

        public LoginController(IConfiguration configuration, IKeyCloakClient dataClient)
        {
            _configuration = configuration;
            _dataClient = dataClient;
        }
        [HttpPost("get-token")]
        public async Task<IActionResult> GetToken([FromBody] LoginDto request)
        {
            if (request == null || string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("Username and password are required.");
            }
            var rs = string.Empty;
            try
            {
                rs = await _dataClient.GetTokenKeyCloak(request);
                if (rs == null)
                {
                    return Ok("Cannot find user");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return Ok(rs);
        }
    }
}
