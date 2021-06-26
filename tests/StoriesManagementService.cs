using AutoMapper;
using FinalProject.Data;
using FinalProject.Mapping;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Threading.Tasks;
using static tests.OperationalStoreForTests;

namespace tests
{
	class StoriesManagementService
	{
		private static IMapper _mapper;
		private ApplicationDbContext _context;
		[SetUp]
		public void Setup()
		{

				var mappingConfig = new MapperConfiguration(mc =>
				{
					mc.AddProfile(new MappingProfile());
				});
				IMapper mapper = mappingConfig.CreateMapper();
				_mapper = mapper;
			

			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDB")
				.Options;

			_context = new ApplicationDbContext(options, new OperationalStoreOptionsForTests());

			_context.Stories.Add(new FinalProject.Models.Story { });
			_context.Stories.Add(new FinalProject.Models.Story { });
			_context.SaveChanges();
		}

		[TearDown]
		public void Teardown()
		{
			foreach (var story in _context.Stories)
			{
				_context.Remove(story);
			}
			_context.SaveChanges();
		}

		[Test]
		public async Task TestGetMovies()
		{
			var service = new FinalProject.Services.StoryManagementService(_context, _mapper);
			var moviesResponse = await service.GetStories();
			var moviesCount = moviesResponse.ResponseOk.Entities.Count;
			Assert.AreEqual(2, moviesCount);
		}
	}
}
