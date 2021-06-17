namespace FinalProject.ViewModels
{
	public class FragmentViewModel
	{
		public int Id { get; set; }
		public string UserId { get; set; }
		public AuthorViewModel user { get; set; }
		public int StoryId { get; set; }
		public int Position { get; set; }
		public string Text { get; set; }
	}
}
