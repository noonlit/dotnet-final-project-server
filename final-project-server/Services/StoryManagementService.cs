using FinalProject.Data;
using FinalProject.Errors;
using FinalProject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Services
{
	public class StoryManagementService : IStoryManagementService
	{
		public ApplicationDbContext _context;
		public StoryManagementService(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<ServiceResponse<List<Story>, IEnumerable<EntityManagementError>>> GetStories()
		{
			var stories = await _context.Stories
				.ToListAsync();
			var serviceResponse = new ServiceResponse<List<Story>, IEnumerable<EntityManagementError>>();
			serviceResponse.ResponseOk = stories;
			return serviceResponse;
		}

		public async Task<ServiceResponse<List<Story>, IEnumerable<EntityManagementError>>> GetFilteredStories(string genre)
		{
			var stories = await _context.Stories.Where(s => s.Genre.ToString() == genre)
				.OrderByDescending(s => s.CreatedAt).ToListAsync();

			var serviceResponse = new ServiceResponse<List<Story>, IEnumerable<EntityManagementError>>();
			serviceResponse.ResponseOk = stories;
			return serviceResponse;
		}

		public async Task<ServiceResponse<Story, IEnumerable<EntityManagementError>>> GetStory(int id)
		{
			var story = await _context.Stories
				.Include(s => s.Comments)
				.Where(s => s.Id == id).FirstOrDefaultAsync();

			story.Fragments = await _context.Fragments.Where(f => f.StoryId == story.Id).Include(f => f.User).OrderBy(f => f.Position).ToListAsync();

			var serviceResponse = new ServiceResponse<Story, IEnumerable<EntityManagementError>>();
			serviceResponse.ResponseOk = story;
			return serviceResponse;
		}

		public async Task<ServiceResponse<List<Comment>, IEnumerable<EntityManagementError>>> GetCommentsForStory(int id)
		{
			var comments = await _context.Comments.Where(c => c.StoryId == id).Include(s => s.User).ToListAsync();

			var serviceResponse = new ServiceResponse<List<Comment>, IEnumerable<EntityManagementError>>();
			serviceResponse.ResponseOk = comments;
			return serviceResponse;
		}

		public async Task<ServiceResponse<Story, IEnumerable<EntityManagementError>>> UpdateStory(Story story)
		{
			_context.Entry(story).State = EntityState.Modified;
			var serviceResponse = new ServiceResponse<Story, IEnumerable<EntityManagementError>>();

			try
			{
				await _context.SaveChangesAsync();
				serviceResponse.ResponseOk = story;
			}
			catch (DbUpdateConcurrencyException e)
			{
				var errors = new List<EntityManagementError>();
				errors.Add(new EntityManagementError { Code = e.GetType().ToString(), Description = e.Message });
			}

			return serviceResponse;
		}

		public async Task<ServiceResponse<Comment, IEnumerable<EntityManagementError>>> UpdateComment(Comment comment)
		{
			_context.Entry(comment).State = EntityState.Modified;
			var serviceResponse = new ServiceResponse<Comment, IEnumerable<EntityManagementError>>();

			try
			{
				await _context.SaveChangesAsync();

				serviceResponse.ResponseOk = comment;
			}
			catch (DbUpdateConcurrencyException e)
			{
				var errors = new List<EntityManagementError>();
				errors.Add(new EntityManagementError { Code = e.GetType().ToString(), Description = e.Message });
			}

			return serviceResponse;
		}

		public async Task<ServiceResponse<Story, IEnumerable<EntityManagementError>>> CreateStory(Story story)
		{
			_context.Stories.Add(story);
			var serviceResponse = new ServiceResponse<Story, IEnumerable<EntityManagementError>>();

			try
			{
				await _context.SaveChangesAsync();
				serviceResponse.ResponseOk = story;
			}
			catch (Exception e)
			{
				var errors = new List<EntityManagementError>();
				errors.Add(new EntityManagementError { Code = e.GetType().ToString(), Description = e.Message });
			}

			return serviceResponse;
		}

		public async Task<ServiceResponse<Comment, IEnumerable<EntityManagementError>>> AddCommentToStory(int storyId, Comment comment)
		{
			var story = await _context.Stories
				.Where(s => s.Id == storyId)
				.Include(s => s.Comments)
				.FirstOrDefaultAsync();

			var serviceResponse = new ServiceResponse<Comment, IEnumerable<EntityManagementError>>();

			if (story == null)
			{
				var errors = new List<EntityManagementError>();
				errors.Add(new EntityManagementError { Description = "The story doesn't exist." });
				return serviceResponse;
			}

			try
			{
				story.Comments.Add(comment);
				_context.Entry(story).State = EntityState.Modified;
				_context.SaveChanges();
			}
			catch (Exception e)
			{
				var errors = new List<EntityManagementError>();
				errors.Add(new EntityManagementError { Code = e.GetType().ToString(), Description = e.Message });
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
			}

			return serviceResponse;
		}

		public async Task<ServiceResponse<Fragment, IEnumerable<EntityManagementError>>> UpdateFragment(Fragment fragment)
		{
			_context.Entry(fragment).State = EntityState.Modified;

			var serviceResponse = new ServiceResponse<Fragment, IEnumerable<EntityManagementError>>();

			try
			{
				await _context.SaveChangesAsync();

				serviceResponse.ResponseOk = fragment;
			}
			catch (DbUpdateConcurrencyException e)
			{
				var errors = new List<EntityManagementError>();
				errors.Add(new EntityManagementError { Code = e.GetType().ToString(), Description = e.Message });
			}

			return serviceResponse;
		}

		public async Task<ServiceResponse<Fragment, IEnumerable<EntityManagementError>>> AddFragmentToStory(int storyId, Fragment fragment)
		{
			var story = await _context.Stories
				.Include(s => s.Fragments)
				.Where(s => s.Id == storyId)
				.FirstOrDefaultAsync();

			var serviceResponse = new ServiceResponse<Fragment, IEnumerable<EntityManagementError>>();

			if (story == null)
			{
				var errors = new List<EntityManagementError>();
				errors.Add(new EntityManagementError { Description = "The story doesn't exist." });
				return serviceResponse;
			}

			try
			{
				var maxPosition = _context.Fragments.Max(f => f.Position);
				fragment.Position = maxPosition + 1;
				story.Fragments.Add(fragment);
				_context.Entry(story).State = EntityState.Modified;
				_context.SaveChanges();
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