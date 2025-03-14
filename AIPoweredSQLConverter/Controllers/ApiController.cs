using AIPoweredSQLConverter.API;
using AIPoweredSQLConverter.Business;
using Microsoft.AspNetCore.Mvc;

namespace AIPoweredSQLConverter.Controllers
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
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong when authenticating");
            }
        }

        [HttpPost("/post/sqlData")]
        public async Task<IActionResult> SaveSQLData([FromBody] FrontEndRequest requestBody)
        {
            try
            {
                if (requestBody == null) return BadRequest("The request body is malformed.");
                if (string.IsNullOrEmpty(requestBody.Username)) return BadRequest("The username is required.");
                if (string.IsNullOrEmpty(requestBody.SqlData)) return BadRequest("The SQL data is required.");

                var success = await _promptFlowLogic.UpsertUserSQLProfile(requestBody);
                if (success) return NoContent();
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong when saving the SQL data");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong");
            }
        }

        [HttpPost("/post/sqlHelp")]
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

        [HttpPost("/post/convertQuery")]
        public async Task<IActionResult> ConvertQueryToSQL([FromBody] FrontEndRequest requestBody)
        {
            try
            {
                if (requestBody == null) return BadRequest("The request body is malformed.");
                if (string.IsNullOrEmpty(requestBody.Username)) return BadRequest("The username is required.");
                if (string.IsNullOrEmpty(requestBody.Query)) return BadRequest("The query is required.");
                if (requestBody.ConversationId == null) return BadRequest("The conversationId is required.");

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