using FinalProject.Errors;
using FinalProject.Models;
using FinalProject.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinalProject.Services
{
	public interface IStoryManagementService
	{
		public Task<ServiceResponse<PaginatedResultSet<StoryViewModel>, IEnumerable<EntityManagementError>>> GetStories(int? page = 1, int? perPage = 10);
		public Task<ServiceResponse<PaginatedResultSet<StoryViewModel>, IEnumerable<EntityManagementError>>> GetFilteredStories(int tagId, int? page = 1, int? perPage = 10);
		public Task<ServiceResponse<PaginatedStoryViewModel, IEnumerable<EntityManagementError>>> GetStory(int id);
		public Task<ServiceResponse<PaginatedResultSet<CommentViewModel>, IEnumerable<EntityManagementError>>> GetCommentsForStory(int id, int? page = 1, int? perPage = 10);
		public Task<ServiceResponse<bool, IEnumerable<EntityManagementError>>> UpdateStory(Story story);
		public Task<ServiceResponse<bool, IEnumerable<EntityManagementError>>> UpdateComment(Comment comment);
		public Task<ServiceResponse<bool, IEnumerable<EntityManagementError>>> CreateStory(Story story);
		public Task<ServiceResponse<bool, IEnumerable<EntityManagementError>>> AddCommentToStory(int storyId, Comment comment);
		public Task<ServiceResponse<bool, IEnumerable<EntityManagementError>>> DeleteStory(int storyId);
		public Task<ServiceResponse<bool, IEnumerable<EntityManagementError>>> DeleteComment(int commentId);
		public Task<ServiceResponse<bool, IEnumerable<EntityManagementError>>> UpdateFragment(Fragment fragment);
		public Task<ServiceResponse<bool, IEnumerable<EntityManagementError>>> AddFragmentToStory(int storyId, Fragment fragment, bool isLast = false);
		public Task<ServiceResponse<PaginatedResultSet<FragmentViewModel>, IEnumerable<EntityManagementError>>> GetFragmentsForStory(int id, int? page = 1, int? perPage = 10);
		public Task<ServiceResponse<bool, IEnumerable<EntityManagementError>>> DeleteFragment(int fragmentId);
		public bool StoryExists(int id);
		public bool CommentExists(int id);
		public bool FragmentExists(int fragmentId);
	}
}
