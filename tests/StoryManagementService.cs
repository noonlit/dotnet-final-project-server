using AutoMapper;
using FinalProject.Data;
using FinalProject.Mapping;
using FinalProject.Models;
using FinalProject.Services;
using FinalProject.ViewModels;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Threading.Tasks;
using static tests.OperationalStoreForTests;

namespace tests
{
	class StoryManagementService
	{
		private static IMapper _mapper;
		private ApplicationDbContext _context;
		private FinalProject.Services.StoryManagementService _service;


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

			_context.Stories.Add(new Story { });
			_context.Stories.Add(new Story { });
			_context.SaveChanges();

			_service = new FinalProject.Services.StoryManagementService(_context, _mapper);
		}

		[TearDown]
		public void Teardown()
		{
			foreach (var story in _context.Stories)
			{
				_context.Remove(story);
			}

			foreach (var tag in _context.Tags)
			{
				_context.Remove(tag);
			}

			_context.SaveChanges();
		}

		[Test]
		public async Task TestGetStories()
		{
			var response = await _service.GetStories();
			var count = response.ResponseOk.Entities.Count;
			Assert.AreEqual(2, count);
		}

		[Test]
		public async Task TestGetFilteredStories()
		{
			var story = await _context.Stories
				.FirstOrDefaultAsync();

			var tag = new Tag { Name = "Tag" };
			var result = await _service.AddTagToStory(story.Id, tag);

			Assert.True(result.ResponseError == null);

			var stories = await _service.GetFilteredStories(tag.Id);
			Assert.AreEqual(1, stories.ResponseOk.Entities.Count);
		}

		[Test]
		public async Task TestGetStory()
		{
			var story = await _context.Stories.FirstOrDefaultAsync();
			var storyId = story.Id;

			var response = await _service.GetStory(storyId);
			Assert.AreEqual(storyId, response.ResponseOk.Story.Id);
		}

		[Test]
		public async Task TestCreateStory()
		{
			var currentStories = await _service.GetStories();
			var count = currentStories.ResponseOk.Entities.Count;
			Assert.AreEqual(2, count);

			var author = await _context.ApplicationUsers.FirstOrDefaultAsync();
			var story = new Story
			{
				Description = "A new story",
				Genre = Story.GenreType.Fantasy,
				Owner = author
			};

			var result = await _service.CreateStory(story);
			Assert.True(result.ResponseError == null);
			currentStories = await _service.GetStories();
			count = currentStories.ResponseOk.Entities.Count;
			Assert.AreEqual(3, count);
		}

		[Test]
		public async Task TestUpdateStory()
		{
			var story = await _context.Stories.FirstOrDefaultAsync();

			story.Description = "New description";

			var result = await _service.UpdateStory(story);
			Assert.True(result.ResponseError == null);

			var loadedStory = await _context.Stories.FindAsync(story.Id);
			Assert.AreEqual("New description", loadedStory.Description);
		}

		[Test]
		public async Task TestDeleteStory()
		{
			var story = await _context.Stories.FirstOrDefaultAsync();

			var storyId = story.Id;

			await _service.DeleteStory(story.Id);

			Assert.False(await _context.Stories.AnyAsync(s => s.Id == storyId));
		}


		[Test]
		public async Task TestGetCommentsForStory()
		{
			var story = await _context.Stories
				.Include(s => s.Comments)
				.FirstOrDefaultAsync();

			var comment = new Comment { Text = "this is quite a long comment" };
			story.Comments.Add(comment);
			_context.Entry(story).State = EntityState.Modified;
			_context.SaveChanges();

			var result = await _service.GetCommentsForStory(story.Id);
			Assert.AreEqual(1, result.ResponseOk.Entities.Count);
		}

		[Test]
		public async Task TestGetComment()
		{
			var story = await _context.Stories
				.Include(s => s.Comments)
				.FirstOrDefaultAsync();

			var comment = new Comment { Text = "this is quite a long comment" };
			story.Comments.Add(comment);
			_context.Entry(story).State = EntityState.Modified;
			_context.SaveChanges();

			var result = await _service.GetComment(comment.Id);
			Assert.True(result.ResponseOk.Text == "this is quite a long comment");
		}

		[Test]
		public async Task TestUpdateComment()
		{
			var story = await _context.Stories.FirstOrDefaultAsync();
			
			var comment = new Comment { Text = "this is a comment", StoryId = story.Id };
			_context.Comments.Add(comment);
			_context.SaveChanges();

			var loadedComment = await _context.Comments.FindAsync(comment.Id);
			loadedComment.Text = "New comment text";

			var result = await _service.UpdateComment(loadedComment);
			Assert.True(result.ResponseError == null);

			var reloadedComment = await _context.Comments.FindAsync(comment.Id);
			Assert.AreEqual("New comment text", loadedComment.Text);
		}

		[Test]
		public async Task TestDeleteComment()
		{
			var story = await _context.Stories
							.Include(s => s.Comments)
							.FirstOrDefaultAsync();

			var comment = new Comment { Text = "this is quite a long comment" };
			story.Comments.Add(comment);
			_context.Entry(story).State = EntityState.Modified;
			_context.SaveChanges();

			var commentId = comment.Id;

			await _service.DeleteComment(comment.Id);

			Assert.False(await _context.Comments.AnyAsync(c => c.Id == commentId));
		}

		[Test]
		public async Task TestGetFragmentsForStory()
		{
			var story = await _context.Stories
				.Include(s => s.Fragments)
				.FirstOrDefaultAsync();

			var fragment = new Fragment { Text = "fragment fragment fragment" };
			story.Fragments.Add(fragment);
			_context.Entry(story).State = EntityState.Modified;
			_context.SaveChanges();

			var result = await _service.GetFragmentsForStory(story.Id);
			Assert.AreEqual(1, result.ResponseOk.Entities.Count);
		}

		[Test]
		public async Task TestGetFragment()
		{
			var story = await _context.Stories
				.Include(s => s.Fragments)
				.FirstOrDefaultAsync();

			var fragment = new Fragment { Text = "fragment fragment fragment" };
			story.Fragments.Add(fragment);
			_context.Entry(story).State = EntityState.Modified;
			_context.SaveChanges();

			var fragmentId = fragment.Id;
			var result = await _service.GetFragment(fragmentId);
			Assert.AreEqual(fragmentId, result.ResponseOk.Id);
		}

		[Test]
		public async Task TestAddCommentToStory()
		{
			var story = await _context.Stories
				.FirstOrDefaultAsync();

			var comment = new Comment { Text = "this is quite a long comment" };
			await _service.AddCommentToStory(story.Id, comment);

			var reloadedStory = await _context.Stories.FindAsync(story.Id);
			Assert.AreEqual(1, reloadedStory.Comments.Count);
		}

		[Test]
		public async Task TestAddFragmentToStory()
		{
			var story = await _context.Stories
				.FirstOrDefaultAsync();

			var author = await _context.ApplicationUsers.FirstOrDefaultAsync();
			var fragment = new Fragment { Text = "this is a relatively short long fragment", Position = 1, User = author };
			await _service.AddFragmentToStory(story.Id, fragment);

			var reloadedStory = await _context.Stories.FindAsync(story.Id);
			Assert.AreEqual(1, reloadedStory.Fragments.Count);
		}

		[Test]
		public async Task TestUpdateFragment()
		{
			var story = await _context.Stories.FirstOrDefaultAsync();

			var author = await _context.ApplicationUsers.FirstOrDefaultAsync();
			var fragment = new Fragment { Text = "this is a relatively short long fragment", Position = 1, User = author, StoryId = story.Id };
			_context.Fragments.Add(fragment);
			_context.SaveChanges();

			var loadedFragment = await _context.Fragments.FindAsync(fragment.Id);
			loadedFragment.Text = "New fragment text";

			var result = await _service.UpdateFragment(loadedFragment);
			Assert.True(result.ResponseError == null);

			var reloadedFragment = await _context.Fragments.FindAsync(fragment.Id);
			Assert.AreEqual("New fragment text", loadedFragment.Text);
		}

		[Test]
		public async Task TestDeleteFragment()
		{
			var story = await _context.Stories
							.FirstOrDefaultAsync();

			var author = await _context.ApplicationUsers.FirstOrDefaultAsync();
			var fragment = new Fragment { Text = "this is a relatively short fragment", Position = 1, User = author };
			var result = await _service.AddFragmentToStory(story.Id, fragment);

			Assert.True(result.ResponseError == null);
			await _service.DeleteFragment(fragment.Id);

			Assert.False(await _context.Fragments.AnyAsync(f => f.Id == fragment.Id));
		}


		[Test]
		public async Task TestGetTag()
		{
			var tag = new Tag { Name = "Tag" };
			_context.Tags.Add(tag);
			_context.SaveChanges();

			var loadedTag = await _service.GetTag(tag.Id);

			Assert.AreEqual("Tag", loadedTag.ResponseOk.Name);
		}

		[Test]
		public async Task TestGetTags()
		{
			var tag = new Tag { Name = "Tag" };
			_context.Tags.Add(tag);
			_context.SaveChanges();

			var loadedTags = await _service.GetTags();

			Assert.AreEqual(1, loadedTags.ResponseOk.Count);
		}

		[Test]
		public async Task TestCreateTag()
		{
			var tag = new Tag { Name = "Tag" };
			var result = await _service.CreateTag(tag);

			Assert.True(result.ResponseError == null);

			var tagExists = await _context.Tags.AnyAsync(tag => tag.Id == result.ResponseOk.Id);
			Assert.True(tagExists);
		}

		[Test]
		public async Task TestUpdateTag()
		{
			var tag = new Tag { Name = "Tag" };
			_context.Tags.Add(tag);
			_context.SaveChanges();

			var loadedTag = await _context.Tags.FindAsync(tag.Id);
			loadedTag.Name = "New Tag";

			var result = await _service.UpdateTag(loadedTag);
			Assert.True(result.ResponseError == null);

			var reloadedTag = await _context.Tags.FindAsync(tag.Id);
			Assert.AreEqual("New Tag", loadedTag.Name);
		}


		[Test]
		public async Task TestAddTagToStory()
		{
			var story = await _context.Stories
				.FirstOrDefaultAsync();

			var tag = new Tag { Name = "Tag" };
			var result = await _service.AddTagToStory(story.Id, tag);

			Assert.True(result.ResponseError == null);

			var reloadedStory = await _context.Stories.FindAsync(story.Id);
			Assert.AreEqual(1, reloadedStory.Tags.Count);
		}

		[Test]
		public async Task TestRemoveTagFromStory()
		{
			var story = await _context.Stories
				.FirstOrDefaultAsync();

			var tag = new Tag { Name = "Tag" };
			await _service.AddTagToStory(story.Id, tag);

			var result = await _service.RemoveTagFromStory(story.Id, tag.Id);
			Assert.True(result.ResponseError == null);

			var reloadedStory = await _context.Stories.FindAsync(story.Id);
			Assert.AreEqual(0, reloadedStory.Tags.Count);
		}

		[Test]
		public async Task TestDeleteTag()
		{
			var tag = new Tag { Name = "Tag" };
			_context.Tags.Add(tag);
			_context.SaveChanges();

			var tagId = tag.Id;

			await _service.DeleteTag(tag.Id);

			Assert.False(await _context.Tags.AnyAsync(t => t.Id == tagId));
		}

	}
}
