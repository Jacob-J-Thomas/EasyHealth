using AIPoweredSQLConverter.API;
using AIPoweredSQLConverter.Business;
using AIPoweredSQLConverter.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIPoweredSQLConverter.Controllers
{
    [ApiController]
    [Authorize]
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

        [HttpGet("get/newAPIKey/{username}")]
        public async Task<IActionResult> GetNewAPIKey([FromRoute] string username)
        {
            try
            {
                if (string.IsNullOrEmpty(username)) return BadRequest("The username is required.");

                //var result = await _authLogic.RetrieveAuthToken();
                //if (result?.AccessToken == null) return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong when authenticating");

                //// scrub unnescessary data
                //var clientSafeResult = new AuthTokenResponse()
                //{
                //    AccessToken = result.AccessToken,
                //    ExpiresIn = result.ExpiresIn,
                //};

                //return Ok(clientSafeResult);
                return Ok("Pretend API Key");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong when authenticating");
            }
        }

        [HttpGet("get/sqlData/{username}")]
        public IActionResult GetSQLData([FromRoute] string username)
        {
            try
            {
                if (string.IsNullOrEmpty(username)) return BadRequest("The username is required.");
                var response = _promptFlowLogic.GetSQLData(username);
                if (!string.IsNullOrEmpty(response)) return Ok(response);
                return NotFound("No data found.");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong");
            }
        }


        [HttpPost("post/sqlData")]
        public async Task<IActionResult> SaveSQLData([FromBody] FrontEndRequest requestBody)
        {
            try
            {
                if (requestBody == null) return BadRequest("The request body is malformed.");
                if (string.IsNullOrEmpty(requestBody.Username)) return BadRequest("The username is required.");
                if (string.IsNullOrEmpty(requestBody.SqlData)) return BadRequest("The SQL data is required.");

                var success = await _promptFlowLogic.UpsertUserData(requestBody);
                if (success) return NoContent();
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong when saving the SQL data");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong");
            }
        }

        [HttpPost("post/sqlHelp")]
        public async Task<IActionResult> RequestSQLDataHelp([FromBody] FrontEndRequest requestBody)
        {
            try
            {
                if (requestBody == null) return BadRequest("The request body is malformed.");
                if (string.IsNullOrEmpty(requestBody.Username)) return BadRequest("The username is required.");
                if (string.IsNullOrEmpty(requestBody.SqlData)) return BadRequest("The SQL data is required.");
                if (string.IsNullOrEmpty(requestBody.Query)) return BadRequest("The query is required.");

                var response = await _promptFlowLogic.GetSQLDataHelp(requestBody);
                if (!string.IsNullOrEmpty(response)) return Ok(response);
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong when constructing the SQL definition.");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong");
            }
        }

        [HttpPost("post/convertQuery")]
        public async Task<IActionResult> ConvertQueryToSQL([FromBody] FrontEndRequest requestBody)
        {
            try
            {
                if (requestBody == null) return BadRequest("The request body is malformed.");
                if (string.IsNullOrEmpty(requestBody.Username)) return BadRequest("The username is required.");
                if (!requestBody.Messages.Any()) return BadRequest("A message is required.");

                var response = await _promptFlowLogic.ConvertQueryToSQL(requestBody);
                if (!string.IsNullOrEmpty(response)) return Ok(response);
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong when converting the string to SQL.");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong");
            }
        }
    }
}