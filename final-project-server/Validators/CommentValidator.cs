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
			RuleFor(c => c.Text).MinimumLength(10);
			RuleFor(c => c.StoryId).NotNull();
		}
	}
}
