using FinalProject.Data;
using FinalProject.ViewModels;
using FluentValidation;

namespace FinalProject.Validators
{
	public class CommentValidator : AbstractValidator<CommentViewModel>
	{
		private readonly ApplicationDbContext _context;

		public CommentValidator(ApplicationDbContext context)
		{
			_context = context;
			RuleFor(c => c.Text).NotNull().MinimumLength(10).WithMessage("The comment must contain some text");
			RuleFor(c => c.StoryId).NotNull();
		}
	}
}
