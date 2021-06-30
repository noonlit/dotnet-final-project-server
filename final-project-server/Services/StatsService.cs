using AutoMapper;
using FinalProject.Data;
using FinalProject.Models.Stats;
using FinalProject.ViewModels.Stats;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Services
{
	public class StatsService : IStatsService
	{
		private ApplicationDbContext _context;
		private IMapper _mapper;

		public StatsService(ApplicationDbContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		public async Task<List<PopularTagsViewModel>> GetPopularTagsData()
		{
			var sql = "SELECT TOP 5 " +
				"Tags.Id AS TagId, " +
				"Tags.Name AS TagName, " +
				"ISNULL((SELECT COUNT(TagsId) FROM StoryTag WHERE TagsId = Tags.Id GROUP BY TagsId), 0) AS UsageCount " +
				"FROM Tags ORDER BY UsageCount DESC";

			var result = await _context.PopularTags.FromSqlRaw(sql).ToListAsync();

			return _mapper.Map<List<PopularTags>, List<PopularTagsViewModel>>(result);
		}
	}
}
