using System;
using System.Collections.Generic;

namespace FinalProject.ViewModels
{
	public class PaginatedStoryViewModel
	{
		public StoryViewModel Story { get; set; }
		public PaginatedResultSet<FragmentViewModel> Fragments { get; set; }
		public PaginatedResultSet<CommentViewModel> Comments { get; set; }
	}
}
