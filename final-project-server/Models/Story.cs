using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FinalProject.Models
{
	public class Story
	{
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public enum GenreType
		{
			Fantasy, SciFi, Horror, Humour
		}

		public int Id { get; set; }

		public string Title { get; set; }

		public string Description { get; set; }

		public GenreType Genre { get; set; }

		public ApplicationUser Owner { get; set; }

		public List<Fragment> Fragments { get; set; }

		public DateTime CreatedAt { get; set; }

		public bool IsComplete { get; set; }

		public List<Comment> Comments { get; set; }
	}
}
