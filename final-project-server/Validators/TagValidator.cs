using FinalProject.Data;
using FinalProject.ViewModels;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Validators
{
	public class TagValidator : AbstractValidator<TagViewModel>
	{
		private readonly ApplicationDbContext _context;

		public TagValidator(ApplicationDbContext context)
		{
			_context = context;
			RuleFor(s => s.Name).MinimumLength(1);
			RuleFor(s => s.Name).Must(BeUnique).WithMessage("Tags names should not be duplicated.");
		}

		private bool BeUnique(string name)
		{
			return !_context.Tags.Any(e => e.Name == name);
		}
	}
}
