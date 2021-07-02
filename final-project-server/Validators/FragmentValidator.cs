using FinalProject.Data;
using FinalProject.ViewModels;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Validators
{
	public class FragmentValidator : AbstractValidator<FragmentViewModel>
	{
		private readonly ApplicationDbContext _context;

		public FragmentValidator(ApplicationDbContext context)
		{
			_context = context;
			RuleFor(f => f.Text).NotNull().MinimumLength(10).WithMessage("The fragment must contain some text.");
			RuleFor(f => f.StoryId).NotNull();
		}
	}
}
