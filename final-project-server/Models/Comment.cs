namespace FinalProject.Models
{
	public class Comment
	{
		public int Id { get; set; }

		public string Text { get; set; }

		public Story Story { get; set; }

		public int StoryId { get; set; }
	}
}
