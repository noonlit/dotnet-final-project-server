using AutoMapper;
using FinalProject.Data;
using FinalProject.Errors;
using FinalProject.Models;
using FinalProject.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Services
{
	public class StoryManagementService : IStoryManagementService
	{
		private ApplicationDbContext _context;
		private IMapper _mapper;

		public StoryManagementService(ApplicationDbContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		public async Task<ServiceResponse<PaginatedResultSet<StoryViewModel>, IEnumerable<EntityManagementError>>> GetStories(int? page = 1, int? perPage = 10)
		{
			var stories = await _context.Stories
				.Include(s => s.Tags)
				.Skip((page.Value - 1) * perPage.Value)
				.Take(perPage.Value)
				.OrderByDescending(s => s.CreatedAt)
				.ToListAsync();

			var storiesVMs = _mapper.Map<List<Story>, List<StoryViewModel>>(stories);

			var count = _context.Stories.Count();

			var result = new PaginatedResultSet<StoryViewModel>(storiesVMs, page.Value, count, perPage.Value);

			var serviceResponse = new ServiceResponse<PaginatedResultSet<StoryViewModel>, IEnumerable<EntityManagementError>>();
			serviceResponse.ResponseOk = result;
			return serviceResponse;
		}

		public async Task<ServiceResponse<PaginatedResultSet<StoryViewModel>, IEnumerable<EntityManagementError>>> GetFilteredStories(int tagId, int? page = 1, int? perPage = 10)
		{
			var tag = await _context.Tags.FindAsync(tagId);

			var stories = await _context.Stories
				.Include(s => s.Tags)
				.Where(s => s.Tags.Contains(tag))
				.Skip((page.Value - 1) * perPage.Value)
				.Take(perPage.Value)
				.OrderByDescending(s => s.CreatedAt)
				.ToListAsync();

			var storiesVMs = _mapper.Map<List<Story>, List<StoryViewModel>>(stories);

			var count = _context.Stories.Where(s => s.Tags.Contains(tag)).Count();

			var result = new PaginatedResultSet<StoryViewModel>(storiesVMs, page.Value, count, perPage.Value);

			var serviceResponse = new ServiceResponse<PaginatedResultSet<StoryViewModel>, IEnumerable<EntityManagementError>>();
			serviceResponse.ResponseOk = result;
			return serviceResponse;
		}

		public async Task<ServiceResponse<PaginatedStoryViewModel, IEnumerable<EntityManagementError>>> GetStory(int id)
		{
			var story = await _context.Stories
				.Where(s => s.Id == id)
				.Include(s => s.Tags)
				.Include(s => s.Owner)
				.FirstOrDefaultAsync();

			var fragmentsResponse = await GetFragmentsForStory(id);
			var commentsResponse = await GetCommentsForStory(id);

			var serviceResponse = new ServiceResponse<PaginatedStoryViewModel, IEnumerable<EntityManagementError>>();

			var paginatedStory = new PaginatedStoryViewModel();
			paginatedStory.Story = _mapper.Map<StoryViewModel>(story);
			paginatedStory.Fragments = fragmentsResponse.ResponseOk;
			paginatedStory.Comments = commentsResponse.ResponseOk;

			serviceResponse.ResponseOk = paginatedStory;

			return serviceResponse;
		}

		public async Task<ServiceResponse<PaginatedResultSet<CommentViewModel>, IEnumerable<EntityManagementError>>> GetCommentsForStory(int id, int? page = 1, int? perPage = 10)
		{
			var comments = await _context.Comments
				.Where(c => c.StoryId == id)
				.Skip((page.Value - 1) * perPage.Value)
				.Take(perPage.Value)
				.Include(s => s.User)
				.OrderBy(c => c.Id)
				.ToListAsync();

			var commentsVMs = _mapper.Map<List<Comment>, List<CommentViewModel>>(comments);

			var count = _context.Comments.Where(c => c.StoryId == id).Count();

			var result = new PaginatedResultSet<CommentViewModel>(commentsVMs, page.Value, count, perPage.Value);

			var serviceResponse = new ServiceResponse<PaginatedResultSet<CommentViewModel>, IEnumerable<EntityManagementError>>();
			serviceResponse.ResponseOk = result;
			return serviceResponse;
		}

		public async Task<ServiceResponse<PaginatedResultSet<FragmentViewModel>, IEnumerable<EntityManagementError>>> GetFragmentsForStory(int id, int? page = 1, int? perPage = 10)
		{
			var fragments = await _context.Fragments
				.Where(f => f.StoryId == id)
				.Skip((page.Value - 1) * perPage.Value)
				.Take(perPage.Value)
				.Include(s => s.User)
				.OrderBy(c => c.Id)
				.ToListAsync();

			var fragmentsVMs = _mapper.Map<List<Fragment>, List<FragmentViewModel>>(fragments);

			var count = _context.Fragments.Where(f => f.StoryId == id).Count();

			var result = new PaginatedResultSet<FragmentViewModel>(fragmentsVMs, page.Value, count, perPage.Value);

			var serviceResponse = new ServiceResponse<PaginatedResultSet<FragmentViewModel>, IEnumerable<EntityManagementError>>();
			serviceResponse.ResponseOk = result;
			return serviceResponse;
		}

		public async Task<ServiceResponse<bool, IEnumerable<EntityManagementError>>> UpdateStory(Story story)
		{
			var loadedStory = await _context.Stories.Where(s => s.Id == story.Id)
				.Include(s => s.Tags)
				.FirstOrDefaultAsync();

			loadedStory.Tags.RemoveAll(t => t.Id > 0);
			story.Tags.ForEach(t => {
				var tag = _context.Tags.Find(t.Id);
				loadedStory.Tags.Add(tag);
			});

			var serviceResponse = new ServiceResponse<bool, IEnumerable<EntityManagementError>>();
			_context.Entry(loadedStory).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
				serviceResponse.ResponseOk = true;
			}
			catch (DbUpdateConcurrencyException e)
			{
				var errors = new List<EntityManagementError>();
				errors.Add(new EntityManagementError { Code = e.GetType().ToString(), Description = e.Message });
				serviceResponse.ResponseError = errors;
			}

			return serviceResponse;
		}

		public async Task<ServiceResponse<bool, IEnumerable<EntityManagementError>>> UpdateStoryTags(Story story)
		{
			var loadedTags = await _context.Tags
				.Include(s => s.Stories)
				.Where(s => s.Stories.Contains(story))
				.ToListAsync();

			var deleteTags = loadedTags.Except(story.Tags).ToList();
			deleteTags.ForEach(t => t.Stories.Remove(story));

			await _context.SaveChangesAsync();
			var serviceResponse = new ServiceResponse<bool, IEnumerable<EntityManagementError>>();

			try
			{
				await _context.SaveChangesAsync();
				serviceResponse.ResponseOk = true;
			}
			catch (DbUpdateConcurrencyException e)
			{
				var errors = new List<EntityManagementError>();
				errors.Add(new EntityManagementError { Code = e.GetType().ToString(), Description = e.Message });
				serviceResponse.ResponseError = errors;
			}

			return serviceResponse;
		}

		public async Task<ServiceResponse<bool, IEnumerable<EntityManagementError>>> UpdateComment(Comment comment)
		{
			_context.Entry(comment).State = EntityState.Modified;
			var serviceResponse = new ServiceResponse<bool, IEnumerable<EntityManagementError>>();

			try
			{
				await _context.SaveChangesAsync();
				serviceResponse.ResponseOk = true;
			}
			catch (DbUpdateConcurrencyException e)
			{
				var errors = new List<EntityManagementError>();
				errors.Add(new EntityManagementError { Code = e.GetType().ToString(), Description = e.Message });
				serviceResponse.ResponseError = errors;
			}

			return serviceResponse;
		}

		public async Task<ServiceResponse<bool, IEnumerable<EntityManagementError>>> UpdateTag(Tag tag)
		{
			_context.Entry(tag).State = EntityState.Modified;
			var serviceResponse = new ServiceResponse<bool, IEnumerable<EntityManagementError>>();

			try
			{
				await _context.SaveChangesAsync();
				serviceResponse.ResponseOk = true;
			}
			catch (DbUpdateConcurrencyException e)
			{
				var errors = new List<EntityManagementError>();
				errors.Add(new EntityManagementError { Code = e.GetType().ToString(), Description = e.Message });
				serviceResponse.ResponseError = errors;
			}

			return serviceResponse;
		}

		public async Task<ServiceResponse<StoryViewModel, IEnumerable<EntityManagementError>>> CreateStory(Story story)
		{
			var tags = story.Tags is List<Tag> ? story.Tags : new List<Tag>();
			story.Tags = null;

			_context.Stories.Add(story);

			var serviceResponse = new ServiceResponse<StoryViewModel, IEnumerable<EntityManagementError>>();

			try
			{
				await _context.SaveChangesAsync();
				tags.ForEach(
					async t => await AddTagToStory(story.Id, new Tag { Id = t.Id, Name = t.Name })
				);

				serviceResponse.ResponseOk = _mapper.Map<StoryViewModel>(story);
			}
			catch (Exception e)
			{
				var errors = new List<EntityManagementError>();
				errors.Add(new EntityManagementError { Code = e.GetType().ToString(), Description = e.Message });
				serviceResponse.ResponseError = errors;
			}

			return serviceResponse;
		}

		public async Task<ServiceResponse<TagViewModel, IEnumerable<EntityManagementError>>> CreateTag(Tag tag)
		{
			_context.Tags.Add(tag);
			var serviceResponse = new ServiceResponse<TagViewModel, IEnumerable<EntityManagementError>>();

			try
			{
				await _context.SaveChangesAsync();
				serviceResponse.ResponseOk = _mapper.Map<TagViewModel>(tag);
			}
			catch (Exception e)
			{
				var errors = new List<EntityManagementError>();
				errors.Add(new EntityManagementError { Code = e.GetType().ToString(), Description = e.Message });
				serviceResponse.ResponseError = errors;
			}

			return serviceResponse;
		}

		public async Task<ServiceResponse<CommentViewModel, IEnumerable<EntityManagementError>>> AddCommentToStory(int storyId, Comment comment)
		{
			var story = await _context.Stories
				.Where(s => s.Id == storyId)
				.Include(s => s.Comments)
				.FirstOrDefaultAsync();

			var serviceResponse = new ServiceResponse<CommentViewModel, IEnumerable<EntityManagementError>>();

			if (story == null)
			{
				var errors = new List<EntityManagementError>();
				errors.Add(new EntityManagementError { Description = "The story doesn't exist." });
				serviceResponse.ResponseError = errors;
				return serviceResponse;
			}

			try
			{
				story.Comments.Add(comment);
				_context.Entry(story).State = EntityState.Modified;
				_context.SaveChanges();
				serviceResponse.ResponseOk = _mapper.Map<CommentViewModel>(comment);
			}
			catch (Exception e)
			{
				var errors = new List<EntityManagementError>();
				errors.Add(new EntityManagementError { Code = e.GetType().ToString(), Description = e.Message });
				serviceResponse.ResponseError = errors;
			}

			return serviceResponse;
		}

		public async Task<ServiceResponse<bool, IEnumerable<EntityManagementError>>> DeleteComment(int commentId)
		{
			var serviceResponse = new ServiceResponse<bool, IEnumerable<EntityManagementError>>();

			try
			{
				var comment = await _context.Comments.FindAsync(commentId);
				_context.Comments.Remove(comment);
				await _context.SaveChangesAsync();
				serviceResponse.ResponseOk = true;
			}
			catch (Exception e)
			{
				var errors = new List<EntityManagementError>();
				errors.Add(new EntityManagementError { Code = e.GetType().ToString(), Description = e.Message });
				serviceResponse.ResponseError = errors;
			}

			return serviceResponse;
		}

		public async Task<ServiceResponse<bool, IEnumerable<EntityManagementError>>> DeleteStory(int storyId)
		{
			var serviceResponse = new ServiceResponse<bool, IEnumerable<EntityManagementError>>();

			try
			{
				var story = await _context.Stories.FindAsync(storyId);
				_context.Stories.Remove(story);
				await _context.SaveChangesAsync();
				serviceResponse.ResponseOk = true;
			}
			catch (Exception e)
			{
				var errors = new List<EntityManagementError>();
				errors.Add(new EntityManagementError { Code = e.GetType().ToString(), Description = e.Message });
				serviceResponse.ResponseError = errors;
			}

			return serviceResponse;
		}

		public async Task<ServiceResponse<bool, IEnumerable<EntityManagementError>>> DeleteTag(int tagId)
		{
			var serviceResponse = new ServiceResponse<bool, IEnumerable<EntityManagementError>>();

			try
			{
				var tag = await _context.Tags.FindAsync(tagId);
				_context.Tags.Remove(tag);
				await _context.SaveChangesAsync();
				serviceResponse.ResponseOk = true;
			}
			catch (Exception e)
			{
				var errors = new List<EntityManagementError>();
				errors.Add(new EntityManagementError { Code = e.GetType().ToString(), Description = e.Message });
				serviceResponse.ResponseError = errors;
			}

			return serviceResponse;
		}

		public async Task<ServiceResponse<bool, IEnumerable<EntityManagementError>>> UpdateFragment(Fragment fragment)
		{
			_context.Entry(fragment).State = EntityState.Modified;

			var serviceResponse = new ServiceResponse<bool, IEnumerable<EntityManagementError>>();

			try
			{
				await _context.SaveChangesAsync();
				serviceResponse.ResponseOk = true;
			}
			catch (DbUpdateConcurrencyException e)
			{
				var errors = new List<EntityManagementError>();
				errors.Add(new EntityManagementError { Code = e.GetType().ToString(), Description = e.Message });
				serviceResponse.ResponseError = errors;
			}

			return serviceResponse;
		}

		public async Task<ServiceResponse<FragmentViewModel, IEnumerable<EntityManagementError>>> AddFragmentToStory(int storyId, Fragment fragment, bool isLast = false)
		{
			var story = await _context.Stories
				.Include(s => s.Fragments)
				.Where(s => s.Id == storyId)
				.FirstOrDefaultAsync();

			var serviceResponse = new ServiceResponse<FragmentViewModel, IEnumerable<EntityManagementError>>();

			if (story == null)
			{
				var errors = new List<EntityManagementError>();
				errors.Add(new EntityManagementError { Description = "The story doesn't exist." });
				serviceResponse.ResponseError = errors;
				return serviceResponse;
			}

			try
			{
				var storyFragments = await _context.Fragments.Where(f => f.StoryId == story.Id).ToListAsync();
				var lastPosition = storyFragments.Count > 0 ? storyFragments.Max(s => s.Position) : 0;
				fragment.Position = lastPosition + 1;
				story.Fragments.Add(fragment);

				if (isLast)
				{
					story.IsComplete = true;
				}

				_context.Entry(story).State = EntityState.Modified;
				_context.SaveChanges();
				serviceResponse.ResponseOk = _mapper.Map<FragmentViewModel>(fragment);
			}
			catch (Exception e)
			{
				var errors = new List<EntityManagementError>();
				errors.Add(new EntityManagementError { Code = e.GetType().ToString(), Description = e.Message });
			}

			return serviceResponse;
		}

		public async Task<ServiceResponse<bool, IEnumerable<EntityManagementError>>> AddTagToStory(int storyId, Tag tag)
		{
			var story = await _context.Stories
				.Include(s => s.Tags)
				.Where(s => s.Id == storyId)
				.FirstOrDefaultAsync();

			var serviceResponse = new ServiceResponse<bool, IEnumerable<EntityManagementError>>();

			if (story == null)
			{
				var errors = new List<EntityManagementError>();
				errors.Add(new EntityManagementError { Description = "The story doesn't exist." });
				serviceResponse.ResponseError = errors;
				return serviceResponse;
			}

			try
			{
				if (!story.Tags.Contains(tag)) {
					story.Tags.Add(tag);
					_context.Entry(story).State = EntityState.Modified;
					_context.SaveChanges();
				}
				serviceResponse.ResponseOk = true;
			}
			catch (Exception e)
			{
				var errors = new List<EntityManagementError>();
				errors.Add(new EntityManagementError { Code = e.GetType().ToString(), Description = e.Message });
			}

			return serviceResponse;
		}

		public async Task<ServiceResponse<bool, IEnumerable<EntityManagementError>>> DeleteFragment(int fragmentId)
		{
			var serviceResponse = new ServiceResponse<bool, IEnumerable<EntityManagementError>>();

			try
			{
				var fragment = await _context.Fragments.FindAsync(fragmentId);
				_context.Fragments.Remove(fragment);
				await _context.SaveChangesAsync();
				serviceResponse.ResponseOk = true;
			}
			catch (Exception e)
			{
				var errors = new List<EntityManagementError>();
				errors.Add(new EntityManagementError { Code = e.GetType().ToString(), Description = e.Message });
				serviceResponse.ResponseError = errors;
			}

			return serviceResponse;
		}

		public async Task<ServiceResponse<bool, IEnumerable<EntityManagementError>>> RemoveTagFromStory(int storyId, int tagId)
		{
			var story = await _context.Stories
							.Include(s => s.Tags)
							.Where(s => s.Id == storyId)
							.FirstOrDefaultAsync();

			var serviceResponse = new ServiceResponse<bool, IEnumerable<EntityManagementError>>();

			if (story == null)
			{
				var errors = new List<EntityManagementError>();
				errors.Add(new EntityManagementError { Description = "The story doesn't exist." });
				serviceResponse.ResponseError = errors;
				return serviceResponse;
			}

			try
			{
				var tag = await _context.Tags.FindAsync(tagId);
				story.Tags.Remove(tag);

				_context.Entry(story).State = EntityState.Modified;
				_context.SaveChanges();
				serviceResponse.ResponseOk = true;
			}
			catch (Exception e)
			{
				var errors = new List<EntityManagementError>();
				errors.Add(new EntityManagementError { Code = e.GetType().ToString(), Description = e.Message });
			}

			return serviceResponse;
		}

		public async Task<ServiceResponse<TagViewModel, IEnumerable<EntityManagementError>>> GetTag(int id)
		{
			var tag = await _context.Tags.FindAsync(id);

			var tagVM = _mapper.Map<TagViewModel>(tag);

			var serviceResponse = new ServiceResponse<TagViewModel, IEnumerable<EntityManagementError>>();
			serviceResponse.ResponseOk = tagVM;
			return serviceResponse;
		}

		public async Task<ServiceResponse<List<TagViewModel>, IEnumerable<EntityManagementError>>> GetTags()
		{
			var tags = await _context.Tags
				.ToListAsync();

			var tagsVMs = _mapper.Map<List<Tag>, List<TagViewModel>>(tags);

			var serviceResponse = new ServiceResponse<List<TagViewModel>, IEnumerable<EntityManagementError>>();
			serviceResponse.ResponseOk = tagsVMs;
			return serviceResponse;
		}

		public bool StoryExists(int id)
		{
			return _context.Stories.Any(e => e.Id == id);
		}

		public bool CommentExists(int id)
		{
			return _context.Comments.Any(e => e.Id == id);
		}

		public bool FragmentExists(int fragmentId)
		{
			return _context.Fragments.Any(e => e.Id == fragmentId);
		}

		public bool TagExists(int tagId)
		{
			return _context.Tags.Any(e => e.Id == tagId);
		}
	}
}