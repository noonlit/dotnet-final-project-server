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
		public Task<ServiceResponse<StoryViewModel, IEnumerable<EntityManagementError>>> CreateStory(Story story);
		public Task<ServiceResponse<bool, IEnumerable<EntityManagementError>>> UpdateStory(Story story);
		public Task<ServiceResponse<bool, IEnumerable<EntityManagementError>>> DeleteStory(int storyId);
		public Task<ServiceResponse<PaginatedResultSet<CommentViewModel>, IEnumerable<EntityManagementError>>> GetCommentsForStory(int id, int? page = 1, int? perPage = 10);
		public Task<ServiceResponse<CommentViewModel, IEnumerable<EntityManagementError>>> AddCommentToStory(int storyId, Comment comment);
		public Task<ServiceResponse<bool, IEnumerable<EntityManagementError>>> UpdateComment(Comment comment);
		public Task<ServiceResponse<bool, IEnumerable<EntityManagementError>>> DeleteComment(int commentId);
		public Task<ServiceResponse<TagViewModel, IEnumerable<EntityManagementError>>> GetTagByName(string name);
		public Task<ServiceResponse<TagViewModel, IEnumerable<EntityManagementError>>> GetTag(int id);
		public Task<ServiceResponse<List<TagViewModel>, IEnumerable<EntityManagementError>>> GetTags();
		public Task<ServiceResponse<TagViewModel, IEnumerable<EntityManagementError>>> CreateTag(Tag tag);
		public Task<ServiceResponse<bool, IEnumerable<EntityManagementError>>> AddTagToStory(int storyId, Tag tag);
		public Task<ServiceResponse<bool, IEnumerable<EntityManagementError>>> RemoveTagFromStory(int storyId, int tagId);
		public Task<ServiceResponse<bool, IEnumerable<EntityManagementError>>> UpdateTag(Tag tag);
		public Task<ServiceResponse<bool, IEnumerable<EntityManagementError>>> DeleteTag(int tagId);
		public Task<ServiceResponse<PaginatedResultSet<FragmentViewModel>, IEnumerable<EntityManagementError>>> GetFragmentsForStory(int id, int? page = 1, int? perPage = 10);
		public Task<ServiceResponse<FragmentViewModel, IEnumerable<EntityManagementError>>> AddFragmentToStory(int storyId, Fragment fragment, bool isLast = false);
		public Task<ServiceResponse<bool, IEnumerable<EntityManagementError>>> UpdateFragment(Fragment fragment);
		public Task<ServiceResponse<bool, IEnumerable<EntityManagementError>>> DeleteFragment(int fragmentId);
		public bool StoryExists(int id);
		public bool CommentExists(int id);
		public bool FragmentExists(int fragmentId);
		public bool TagExists(int tagId);
	}
}
