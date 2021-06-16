namespace FinalProject.ViewModels.Authentication
{
	public class ConfirmUserRequest
	{
		public string Email { get; set; }
		public string ConfirmationToken { get; set; }
	}
}
