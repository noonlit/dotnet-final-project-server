using FinalProject.Services;
using FinalProject.ViewModels.Stats;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class StatsController : ControllerBase
	{
		private readonly IStatsService _queryService;

		public StatsController(IStatsService queryService)
		{
			_queryService = queryService;
		}

		[HttpGet("PopularTags")]
        public async Task<ActionResult<IEnumerable<PopularTagsViewModel>>> GetPopularTags()
		{
			return await _queryService.GetPopularTagsData();
		}

	}
}
