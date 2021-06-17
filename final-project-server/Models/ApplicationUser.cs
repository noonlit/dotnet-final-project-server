using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace FinalProject.Models
{
	public class ApplicationUser : IdentityUser
	{
		public List<Fragment> Fragments { get; set; }

		public List<Comment> Comments { get; set; }
	}
}
