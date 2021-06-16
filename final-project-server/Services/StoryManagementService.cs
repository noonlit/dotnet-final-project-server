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
			var stories = await _context.Stories.Include(f => f.Fragments).ToListAsync();
			var serviceResponse = new ServiceResponse<List<Story>, IEnumerable<EntityManagementError>>();
			serviceResponse.ResponseOk = stories;
			return serviceResponse;
		}

		public async Task<ServiceResponse<List<Story>, IEnumerable<EntityManagementError>>> GetFilteredStories(string genre)
		{
			var stories = await _context.Stories.Where(s => s.Genre.ToString() == genre)
				.Include(f => f.Fragments)
				.OrderByDescending(s => s.CreatedAt).ToListAsync();

			var serviceResponse = new ServiceResponse<List<Story>, IEnumerable<EntityManagementError>>();
			serviceResponse.ResponseOk = stories;
			return serviceResponse;
		}

		public async Task<ServiceResponse<Story, IEnumerable<EntityManagementError>>> GetStory(int id)
		{
			var story = await _context.Stories.Include(f => f.Fragments).Where(s => s.Id == id).FirstOrDefaultAsync();

			var serviceResponse = new ServiceResponse<Story, IEnumerable<EntityManagementError>>();
			serviceResponse.ResponseOk = story;
			return serviceResponse;
		}

		public async Task<ServiceResponse<List<Comment>, IEnumerable<EntityManagementError>>> GetCommentsForStory(int id)
		{
			var comments = await _context.Comments.Where(c => c.StoryId == id).ToListAsync();

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
		public async Task<ServiceResponse<Comment, IEnumerable<EntityManagementError>>> CreateComment(Comment comment)
		{
			_context.Comments.Add(comment);
			var serviceResponse = new ServiceResponse<Comment, IEnumerable<EntityManagementError>>();

			try
			{
				await _context.SaveChangesAsync();
				serviceResponse.ResponseOk = comment;
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
				.Include(s => s.Comments).FirstOrDefaultAsync();

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

				serviceResponse.ResponseOk = comment;
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

		public bool StoryExists(int id)
		{
			return _context.Stories.Any(e => e.Id == id);
		}

		public bool CommentExists(int id)
		{
			return _context.Comments.Any(e => e.Id == id);
		}
	}
}