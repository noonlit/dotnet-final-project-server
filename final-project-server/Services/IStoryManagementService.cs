using FinalProject.Errors;
using FinalProject.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinalProject.Services
{
	public interface IStoryManagementService
	{
		public Task<ServiceResponse<List<Story>, IEnumerable<EntityManagementError>>> GetStories();
		public Task<ServiceResponse<List<Story>, IEnumerable<EntityManagementError>>> GetFilteredStories(string genre);
		public Task<ServiceResponse<Story, IEnumerable<EntityManagementError>>> GetStory(int id);
		public Task<ServiceResponse<List<Comment>, IEnumerable<EntityManagementError>>> GetCommentsForStory(int id);
		public Task<ServiceResponse<Story, IEnumerable<EntityManagementError>>> UpdateStory(Story story);
		public Task<ServiceResponse<Comment, IEnumerable<EntityManagementError>>> UpdateComment(Comment comment);
		public Task<ServiceResponse<Story, IEnumerable<EntityManagementError>>> CreateStory(Story story);
		public Task<ServiceResponse<Comment, IEnumerable<EntityManagementError>>> CreateComment(Comment comment);
		public Task<ServiceResponse<Comment, IEnumerable<EntityManagementError>>> AddCommentToStory(int storyId, Comment comment);
		public Task<ServiceResponse<bool, IEnumerable<EntityManagementError>>> DeleteStory(int storyId);
		public Task<ServiceResponse<bool, IEnumerable<EntityManagementError>>> DeleteComment(int commentId);
		public bool StoryExists(int id);
		public bool CommentExists(int id);
	}
}
