using FinalProject.Services;
using FinalProject.ViewModels.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FinalProject.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthenticationController : ControllerBase
	{
		private IAuthManagementService _authenticationService;

		public AuthenticationController(IAuthManagementService authService)
		{
			_authenticationService = authService;
		}

		[HttpPost]
		[Route("register")] // /api/authentication/register
		public async Task<ActionResult> RegisterUser(RegisterRequest registerRequest)
		{
			var registerServiceResult = await _authenticationService.RegisterUser(registerRequest);
			if (registerServiceResult.ResponseError != null)
			{
				return BadRequest(registerServiceResult.ResponseError);
			}

			return Ok(registerServiceResult.ResponseOk);
		}

		[HttpPost]
		[Route("confirm")]
		public async Task<ActionResult> ConfirmUser(ConfirmUserRequest confirmUserRequest)
		{
			var serviceResult = await _authenticationService.ConfirmUserRequest(confirmUserRequest);
			if (serviceResult)
			{
				return Ok();
			}

			return BadRequest();
		}

		[HttpPost]
		[Route("login")]
		public async Task<ActionResult> Login(LoginRequest loginRequest)
		{
			var serviceResult = await _authenticationService.LoginUser(loginRequest);
			if (serviceResult.ResponseOk != null)
			{
				return Ok(serviceResult.ResponseOk);
			}

			return Unauthorized();
		}

		[HttpGet("current")]
		[Authorize(AuthenticationSchemes = "Identity.Application,Bearer")]
		public async Task<ActionResult<AuthUserResponse>> GetCurrentUser()
		{
			var serviceResult = await _authenticationService.GetCurrentUser(User.FindFirst(ClaimTypes.NameIdentifier).Value);

			if (serviceResult.ResponseOk != null)
			{
				return serviceResult.ResponseOk;
			}

			return NotFound();
		}
	}
}