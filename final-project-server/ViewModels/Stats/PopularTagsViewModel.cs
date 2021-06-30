using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.ViewModels.Stats
{
	public class PopularTagsViewModel
	{
		public int TagId { get; set; }
		public string TagName { get; set; }
		public int UsageCount { get; set; }
	}
}
