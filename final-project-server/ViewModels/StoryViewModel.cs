using System;
using System.Collections.Generic;

namespace FinalProject.ViewModels
{
	public class StoryViewModel
	{
		public enum GenreType
		{
			Fantasy, SciFi, Horror, Humour
		}

		public int Id { get; set; }

		public string Title { get; set; }

		public string Description { get; set; }

		public GenreType Genre { get; set; }

		public AuthorViewModel Owner { get; set; }

		public DateTime CreatedAt { get; set; }

		public bool IsComplete { get; set; }

		public List<TagViewModel> Tags { get; set; }
	}
}
