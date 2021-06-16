using FinalProject.Data;
using FinalProject.ViewModels;
using FluentValidation;

namespace FinalProject.Validators
{
	public class StoryValidator : AbstractValidator<StoryViewModel>
	{
		private readonly ApplicationDbContext _context;

		public StoryValidator(ApplicationDbContext context)
		{
			_context = context;
			RuleFor(s => s.Title).MinimumLength(1);
			RuleFor(s => s.Description).MinimumLength(10);
			RuleFor(s => s.Genre).NotNull();
		}
	}
}
