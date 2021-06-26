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

		public async Task<ServiceResponse<PaginatedResultSet<StoryViewModel>, IEnumerable<EntityManagementError>>> GetFilteredStories(string genre, int? page = 1, int? perPage = 10)
		{
			var stories = await _context.Stories
				.Where(s => s.Genre.ToString() == genre)
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

		public async Task<ServiceResponse<PaginatedStoryViewModel, IEnumerable<EntityManagementError>>> GetStory(int id)
		{
			var story = await _context.Stories
				.Where(s => s.Id == id)
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
			_context.Entry(story).State = EntityState.Modified;
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

		public async Task<ServiceResponse<bool, IEnumerable<EntityManagementError>>> CreateStory(Story story)
		{
			_context.Stories.Add(story);
			var serviceResponse = new ServiceResponse<bool, IEnumerable<EntityManagementError>>();

			try
			{
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

		public async Task<ServiceResponse<bool, IEnumerable<EntityManagementError>>> AddCommentToStory(int storyId, Comment comment)
		{
			var story = await _context.Stories
				.Where(s => s.Id == storyId)
				.Include(s => s.Comments)
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
				story.Comments.Add(comment);
				_context.Entry(story).State = EntityState.Modified;
				_context.SaveChanges();
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

		public async Task<ServiceResponse<bool, IEnumerable<EntityManagementError>>> AddFragmentToStory(int storyId, Fragment fragment, bool isLast = false)
		{
			var story = await _context.Stories
				.Include(s => s.Fragments)
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
				var maxPosition = _context.Fragments.Max(f => f.Position);
				fragment.Position = maxPosition + 1;
				story.Fragments.Add(fragment);

				if (isLast)
				{
					story.IsComplete = true;
				}

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
	}
}