namespace FinalProject.ViewModels
{
	public class CommentViewModel
	{
		public int Id { get; set; }
		public string Text { get; set; }
		public int StoryId { get; set; }
		public string UserId { get; set; }
		public AuthorViewModel user { get; set; }
	}
}
