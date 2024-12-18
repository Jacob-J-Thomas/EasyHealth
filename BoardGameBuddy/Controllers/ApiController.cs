using BoardGameBuddy.API;
using BoardGameBuddy.Business;
using Microsoft.AspNetCore.Mvc;

namespace BoardGameBuddy.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApiController : ControllerBase
    {
        private readonly IAuthLogic _authLogic;
        private readonly IPromptFlowLogic _promptFlowLogic;

        public ApiController(IAuthLogic authLogic, IPromptFlowLogic promptLogic)
        {
            _authLogic = authLogic;
            _promptFlowLogic = promptLogic;
        }

        [HttpGet("Auth")]
        public async Task<IActionResult> Authorize()
        {
            try
            {
                var result = await _authLogic.RetrieveAuthToken();
                if (result?.AccessToken == null) return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong when authenticating");

                // scrub unnescessary data
                var clientSafeResult = new AuthTokenResponse()
                {
                    AccessToken = result.AccessToken,
                    ExpiresIn = result.ExpiresIn,
                };

                return Ok(clientSafeResult);
                
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong when authenticating");
            }
        }

        [HttpGet("GenerateWord")]
        public async Task<IActionResult> GetHangmanWord()
        {
            try
            {
                var word = await _promptFlowLogic.GenerateHangmanWord();
                if (string.IsNullOrEmpty(word) || word.Contains(' ')) return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong when generating a word");
                return Ok(word);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong");
            }
        }

        [HttpGet("Start/{word}")]
        public async Task<IActionResult> StartHangman([FromRoute] string word)
        {
            try
            {
                var gameStartResponse = await _promptFlowLogic.StartHangmanGame(word);
                if (string.IsNullOrEmpty(gameStartResponse.Message)) return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong when starting the game");
                return Ok(gameStartResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong");
            }
        }
    }
}