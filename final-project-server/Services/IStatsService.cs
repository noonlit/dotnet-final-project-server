using FinalProject.ViewModels.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Services
{
	public interface IStatsService
	{
		public Task<List<PopularTagsViewModel>> GetPopularTagsData();
	}
}
