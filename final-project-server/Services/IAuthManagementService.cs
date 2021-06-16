using FinalProject.ViewModels.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinalProject.Services
{
	public interface IAuthManagementService
	{
		Task<ServiceResponse<RegisterResponse, IEnumerable<IdentityError>>> RegisterUser(RegisterRequest registerRequest);
		Task<bool> ConfirmUserRequest(ConfirmUserRequest confirmUserRequest);
		Task<ServiceResponse<LoginResponse, string>> LoginUser(LoginRequest loginRequest);
	}
}
