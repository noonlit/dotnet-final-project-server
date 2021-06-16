namespace FinalProject.Models
{
	public class Fragment
	{
		public int Id { get; set; }
		public ApplicationUser User { get; set; }
		public string UserId { get; set; }
		public Story Story { get; set; }
		public int StoryId { get; set; }
		public int Position { get; set; }
		public string Text { get; set; }
	}
}
