using api.Domain.Entities;
using api.Infrastructure.Persistence.TwitchRepository;
using Microsoft.AspNetCore.Mvc;

namespace api.Presentation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TwitchController : ControllerBase
    {
        private readonly TwitchRepository _twitchRepository;
        public TwitchController(TwitchRepository twitchRepository)
        {
            _twitchRepository = twitchRepository;
        }

        [HttpGet("token")]
        public async Task<IActionResult> GetToken(string code)
        {
            try
            {
                TwitchToken tokenResponse = await _twitchRepository.GetToken(code);
                return Ok(tokenResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [HttpGet("refresh")]
        public async Task<IActionResult> RefreshToken(string refreshToken)
        {
            try
            {
                TwitchToken tokenResponse = await _twitchRepository.RefreshToken(refreshToken);
                return Ok(tokenResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [HttpGet("auth")]
        public async Task<IActionResult> GetInformation(string accessToken)
        {
            try
            {
                TwitchToken response = await _twitchRepository.GetAuth(accessToken);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}