using System;

namespace FinalProject.ViewModels.Authentication
{
	public class LoginResponse
	{
		public string Token { get; set; }
		public DateTime Expiration { get; set; }
	}
}
