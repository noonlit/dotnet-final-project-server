namespace FinalProject.Models
{
	public class Comment
	{
		public int Id { get; set; }

		public string Text { get; set; }

		public string UserId { get; set; }

		public ApplicationUser User { get; set; }

		public Story Story { get; set; }

		public int StoryId { get; set; }
	}
}
