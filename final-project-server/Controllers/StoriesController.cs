using AutoMapper;
using FinalProject.Models;
using FinalProject.Services;
using FinalProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FinalProject.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Produces("application/json")]
	public class StoriesController : ControllerBase
	{
		private readonly IMapper _mapper;
		private readonly IStoryManagementService _storyService;
		private readonly UserManager<ApplicationUser> _userManager;

		public StoriesController(IMapper mapper, IStoryManagementService storyService, UserManager<ApplicationUser> userManager)
		{
			_mapper = mapper;
			_storyService = storyService;
			_userManager = userManager;
		}

		/// <summary>
		/// Retrieves a list of stories, filtered by a tag ID.
		/// </summary>
		/// <remarks>
		/// Sample request:
		/// GET /api/Stories/filter/1
		/// </remarks>
		/// <param name="tagId"></param>
		/// <param name="page"></param>
		/// <param name="perPage"></param>
		/// <response code="200">The filtered stories.</response>
		[HttpGet]
		[Route("filter/{tagId}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<ActionResult<PaginatedResultSet<StoryViewModel>>> GetFilteredStories(int tagId, int? page = 1, int? perPage = 10)
		{
			var result = await _storyService.GetFilteredStories(tagId, page, perPage);
			return result.ResponseOk;
		}

		/// <summary>
		/// Retrieves a list of stories.
		/// </summary>
		/// <remarks>
		/// Sample request:
		/// GET /api/Stories
		/// </remarks>
		/// <param name="page"></param>
		/// <param name="perPage"></param>
		/// <response code="200">The stories.</response>
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<ActionResult<PaginatedResultSet<StoryViewModel>>> GetStories(int? page = 1, int? perPage = 10)
		{
			var result = await _storyService.GetStories();
			return result.ResponseOk;
		}

		/// <summary>
		/// Retrieves a story by ID.
		/// </summary>
		/// <remarks>
		/// Sample request:
		/// GET api/Stories/5
		/// </remarks>
		/// <param name="id">The story ID</param>
		/// <response code="200">The story.</response>
		/// <response code="404">If the story is not found.</response>
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[HttpGet("{id}")]
		public async Task<ActionResult<PaginatedStoryViewModel>> GetStory(int id)
		{
			var response = await _storyService.GetStory(id);
			var story = response.ResponseOk;

			if (story == null)
			{
				return NotFound();
			}

			return story;
		}

		/// <summary>
		/// Updates a story.
		/// </summary>
		/// <remarks>
		/// Sample request:
		///
		/// PUT /api/Stories/5
		/// {
		///	   "id": 5
		///    "title": "Title",
		///    "description": "Description!",
		///    "genre": "Humour",
		/// }
		///
		/// </remarks>
		/// <param name="id">The story ID</param>
		/// <param name="story">The story body.</param>
		/// <response code="204">If the item was successfully added.</response>
		/// <response code="400">If the ID in the URL doesn't match the one in the body.</response>
		/// <response code="404">If the item is not found after it is added.</response>
		/// <response code="500">If something goes wrong.</response>
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[HttpPut("{id}")]
		[Authorize(AuthenticationSchemes = "Identity.Application,Bearer")]
		public async Task<IActionResult> PutStory(int id, StoryViewModel story)
		{
			if (id != story.Id)
			{
				return BadRequest();
			}

			var response = await _storyService.UpdateStory(_mapper.Map<Story>(story));

			if (response.ResponseError == null)
			{
				return NoContent();
			}

			if (!_storyService.StoryExists(id))
			{
				return NotFound();
			}

			return StatusCode(500);
		}

		// POST: api/Stories
		/// <summary>
		/// Creates a story.
		/// </summary>
		/// <remarks>
		/// Sample request:
		///
		/// POST /api/Stories
		/// {
		///    "title": "Title",
		///    "description": "Description!",
		///    "genre": "Humour"
		/// }
		///
		/// </remarks>
		/// <param name="story"></param>
		/// <response code="201">Returns the newly created item</response>
		/// <response code="500">If something goes wrong.</response>
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[HttpPost]
		[Authorize(AuthenticationSchemes = "Identity.Application,Bearer")]
		public async Task<ActionResult<Story>> PostStory(StoryViewModel story)
		{
			var user = await _userManager.FindByNameAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value);

			var storyEntity = _mapper.Map<Story>(story);
			storyEntity.Owner = user;

			var storyResponse = await _storyService.CreateStory(storyEntity);

			if (storyResponse.ResponseError == null)
			{
				return CreatedAtAction("GetStory", new { id = storyEntity.Id }, story);
			}

			return StatusCode(500);
		}

		// DELETE: api/Stories/5
		/// <summary>
		/// Deletes a story.
		/// </summary>
		/// <remarks>
		/// Sample request:
		///
		/// DELETE api/Stories/1
		///
		/// </remarks>
		/// <param name="id"></param>
		/// <response code="204">No content if successful.</response>
		/// <response code="404">If the story doesn't exist.</response>  
		/// <response code="500">If something goes wrong.</response> 
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[HttpDelete("{id}")]
		[Authorize(AuthenticationSchemes = "Identity.Application,Bearer")]
		public async Task<IActionResult> DeleteStory(int id)
		{
			if (!_storyService.StoryExists(id))
			{
				return NotFound();
			}

			var result = await _storyService.DeleteStory(id);

			if (result.ResponseError == null)
			{
				return NoContent();
			}


			return StatusCode(500);
		}

		/// <summary>
		/// Retrieves a story's comments by its ID.
		/// </summary>
		/// <remarks>
		/// Sample request:
		/// GET api/Stories/5/Comments
		/// </remarks>
		/// <param name="id">The story ID</param>
		/// <response code="200">The story.</response>
		/// <response code="404">If the story is not found.</response>
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[HttpGet("{id}/Comments")]
		public async Task<ActionResult<PaginatedResultSet<CommentViewModel>>> GetCommentsForStory(int id, int? page = 1, int? perPage = 10)
		{
			if (!_storyService.StoryExists(id))
			{
				return NotFound();
			}

			var response = await _storyService.GetStory(id);
			var story = response.ResponseOk;

			if (story == null)
			{
				return NotFound();
			}

			var commentsResponse = await _storyService.GetCommentsForStory(id, page, perPage);
			return commentsResponse.ResponseOk;
		}

		/// <summary>
		/// Retrieves a comment by ID.
		/// </summary>
		/// <remarks>
		/// Sample request:
		/// GET api/Comments/5
		/// </remarks>
		/// <param name="id">The comment ID</param>
		/// <response code="200">The comment.</response>
		/// <response code="404">If the comment is not found.</response>
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[HttpGet("Comments/{id}")]
		public async Task<ActionResult<CommentViewModel>> GetComment(int id)
		{
			var response = await _storyService.GetComment(id);
			var comment = response.ResponseOk;

			if (comment == null)
			{
				return NotFound();
			}

			return comment;
		}

		/// <summary>
		/// Updates a story comment.
		/// </summary>
		/// <remarks>
		/// Sample request:
		///
		/// PUT: api/Stories/1/Comments/2
		/// {
		///    "text": "some comment",
		///    "storyId": 3,
		/// }
		///
		/// </remarks>
		/// <param name="commentId">The comment ID</param>
		/// <param name="comment">The comment body</param>
		/// <response code="204">If the item was successfully added.</response>
		/// <response code="400">If the ID in the URL doesn't match the one in the body.</response>
		/// <response code="404">If the item is not found.</response>
		/// /// <response code="404">If something goes wrong.</response>
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[HttpPut("{id}/Comments/{commentId}")]
		[Authorize(AuthenticationSchemes = "Identity.Application,Bearer")]
		public async Task<IActionResult> PutComment(int commentId, CommentViewModel comment)
		{
			if (commentId != comment.Id)
			{
				return BadRequest();
			}

			if (!_storyService.StoryExists(comment.StoryId))
			{
				return NotFound();
			}

			var commentResponse = await _storyService.UpdateComment(_mapper.Map<Comment>(comment));

			if (commentResponse.ResponseError == null)
			{
				return NoContent();
			}

			if (!_storyService.CommentExists(commentId))
			{
				return NotFound();
			}

			return StatusCode(500);
		}

		/// <summary>
		/// Creates a story comment.
		/// </summary>
		/// <remarks>
		/// Sample request:
		///
		/// POST /api/Stories/3/Comments
		/// {
		///    "text": "some comment",
		///    "storyId": 3,
		/// }
		///
		/// </remarks>
		/// <param name="id">The story ID</param>
		/// <param name="comment">The comment body</param>
		/// <response code="200">If the item was successfully added.</response>
		/// <response code="404">If story is not found.</response>  
		/// <response code="500">If something goes wrong.</response>  
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[HttpPost("{id}/Comments")]
		public async Task<IActionResult> PostCommentForStory(int id, CommentViewModel comment)
		{
			var commentResponse = await _storyService.AddCommentToStory(id, _mapper.Map<Comment>(comment));

			if (commentResponse.ResponseError == null)
			{
				return Ok();
			}

			return StatusCode(500);
		}

		// DELETE: api/Stories/1/Comments/5
		/// <summary>
		/// Deletes a story comment.
		/// </summary>
		/// <remarks>
		/// Sample request:
		///
		/// DELETE api/Stories/1/Comments/5
		///
		/// </remarks>
		/// <param name="commentId"></param>
		/// <response code="204">No content if successful.</response>
		/// <response code="404">If the comment doesn't exist.</response>  
		/// <response code="500">If something goes wrong.</response>  
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[HttpDelete("{id}/Comments/{commentId}")]
		[Authorize(AuthenticationSchemes = "Identity.Application,Bearer")]
		public async Task<IActionResult> DeleteComment(int commentId)
		{
			if (!_storyService.CommentExists(commentId))
			{
				return NotFound();
			}

			var result = await _storyService.DeleteComment(commentId);

			if (result.ResponseError == null)
			{
				return NoContent();
			}

			return StatusCode(500);
		}

		/// <summary>
		/// Retrieves a list of tags.
		/// </summary>
		/// <remarks>
		/// Sample request:
		/// GET /api/Stories/Tags
		/// </remarks>
		/// <response code="200">The tags.</response>
		[HttpGet("Tags")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<ActionResult<List<TagViewModel>>> GetTags()
		{
			var response = await _storyService.GetTags();
			var tags = response.ResponseOk;

			if (tags == null)
			{
				return NotFound();
			}

			return tags;
		}

		/// <summary>
		/// Retrieves a tag by ID.
		/// </summary>
		/// <remarks>
		/// Sample request:
		/// GET api/Stories/Tags/5
		/// </remarks>
		/// <param name="id">The tag ID</param>
		/// <response code="200">The tag.</response>
		/// <response code="404">If the tag is not found.</response>
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[HttpGet("Tags/{id}")]
		public async Task<ActionResult<TagViewModel>> GetTag(int id)
		{
			var response = await _storyService.GetTag(id);
			var tag = response.ResponseOk;

			if (tag == null)
			{
				return NotFound();
			}

			return tag;
		}

		/// <summary>
		/// Updates a tag.
		/// </summary>
		/// <remarks>
		/// Sample request:
		///
		/// PUT: api/Stories/Tags/2
		/// {
		///    "name": "some name"
		/// }
		///
		/// </remarks>
		/// <param name="tagId">The tag ID</param>
		/// <param name="tag">The tag body</param>
		/// <response code="204">If the item was successfully added.</response>
		/// <response code="400">If the ID in the URL doesn't match the one in the body.</response>
		/// <response code="404">If the item is not found.</response>
		/// /// <response code="404">If something goes wrong.</response>
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[HttpPut("Tags/{tagId}")]
		[Authorize(AuthenticationSchemes = "Identity.Application,Bearer")]
		public async Task<IActionResult> PutTag(int tagId, TagViewModel tag)
		{
			if (tagId != tag.Id)
			{
				return BadRequest();
			}

			var commentResponse = await _storyService.UpdateTag(_mapper.Map<Tag>(tag));

			if (commentResponse.ResponseError == null)
			{
				return NoContent();
			}

			return StatusCode(500);
		}

		/// <summary>
		/// Creates a tag.
		/// </summary>
		/// <remarks>
		/// Sample request:
		///
		/// POST /api/Stories/Tags
		/// {
		///    "name": "tag",
		/// }
		///
		/// </remarks>
		/// <param name="tag">The tag.</param>
		/// <response code="200">If the item was successfully added.</response>
		/// <response code="500">If something goes wrong.</response>  
		[HttpPost("Tags")]
		[Authorize(AuthenticationSchemes = "Identity.Application,Bearer")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<Story>> PostTag(TagViewModel tag)
		{
			var response = await _storyService.CreateTag(_mapper.Map<Tag>(tag));

			if (response.ResponseError == null)
			{
				return CreatedAtAction("GetTag", new { id = tag.Id }, tag);
			}

			return StatusCode(500);
		}

		/// <summary>
		/// Creates a tag for a particular story.
		/// </summary>
		/// <remarks>
		/// Sample request:
		///
		/// POST /api/Stories/1/Tags
		/// {
		///    "name": "tag",
		/// }
		///
		/// </remarks>
		/// <param name="id">The story ID</param>
		/// <param name="tag">The tag.</param>
		/// <response code="200">If the item was successfully added.</response>
		/// <response code="500">If something goes wrong.</response>  
		[Authorize(AuthenticationSchemes = "Identity.Application,Bearer")]
		[HttpPost("{id}/Tags")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> PostTagForStory(int id, TagViewModel tag)
		{
			var commentResponse = await _storyService.AddTagToStory(id, _mapper.Map<Tag>(tag));

			if (commentResponse.ResponseError == null)
			{
				return Ok();
			}

			return StatusCode(500);
		}

		/// <summary>
		/// Removes a tag from a particular story.
		/// </summary>
		/// <remarks>
		/// Sample request:
		///
		/// DELETE /api/Stories/1/Tags/2
		///
		/// </remarks>
		/// <param name="id">The story ID</param>
		/// <param name="tagId">The tagId.</param>
		/// <response code="200">If the item was successfully added.</response>
		/// <response code="500">If something goes wrong.</response> 
		[Authorize(AuthenticationSchemes = "Identity.Application,Bearer")]
		[HttpDelete("{id}/Tags/{tagId}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> RemoveTagFromStory(int id, int tagId)
		{
			var commentResponse = await _storyService.RemoveTagFromStory(id, tagId);

			if (commentResponse.ResponseError == null)
			{
				return Ok();
			}

			return StatusCode(500);
		}

		/// <summary>
		/// Deletes a tag from the system.
		/// </summary>
		/// <remarks>
		/// Sample request:
		///
		/// DELETE /api/Stories/Tags/2
		///
		/// </remarks>
		/// <param name="tagId">The tagId.</param>
		/// <response code="204">No content if successful.</response>
		/// <response code="404">If the tag doesn't exist.</response>  
		/// <response code="500">If something goes wrong.</response>  
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[HttpDelete("Tags/{tagId}")]
		[Authorize(AuthenticationSchemes = "Identity.Application,Bearer")]
		public async Task<IActionResult> DeleteTag(int tagId)
		{
			if (!_storyService.TagExists(tagId))
			{
				return NotFound();
			}

			var result = await _storyService.DeleteTag(tagId);

			if (result.ResponseError == null)
			{
				return NoContent();
			}

			return StatusCode(500);
		}

		/// <summary>
		/// Retrieves a story's fragments by the story ID.
		/// </summary>
		/// <remarks>
		/// Sample request:
		/// GET api/Stories/5/Fragments
		/// </remarks>
		/// <param name="id">The story ID</param>
		/// <param name="page"></param>
		/// <param name="perPage"></param>
		/// <response code="200">The story.</response>
		/// <response code="404">If the story is not found.</response>
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[HttpGet("{id}/Fragments")]
		public async Task<ActionResult<PaginatedResultSet<FragmentViewModel>>> GetFragmentsForStory(int id, int? page = 1, int? perPage = 10)
		{
			if (!_storyService.StoryExists(id))
			{
				return NotFound();
			}

			var response = await _storyService.GetStory(id);
			var story = response.ResponseOk;

			if (story == null)
			{
				return NotFound();
			}

			var fResponse = await _storyService.GetFragmentsForStory(id, page, perPage);
			return fResponse.ResponseOk;
		}

		/// <summary>
		/// Retrieves a fragment by ID.
		/// </summary>
		/// <remarks>
		/// Sample request:
		/// GET api/Stories/Fragments/5
		/// </remarks>
		/// <param name="id">The fragment ID</param>
		/// <response code="200">The fragment.</response>
		/// <response code="404">If the fragment is not found.</response>
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[HttpGet("Fragments/{id}")]
		public async Task<ActionResult<FragmentViewModel>> GetFragment(int id)
		{
			if (!_storyService.FragmentExists(id))
			{
				return NotFound();
			}

			var fResponse = await _storyService.GetFragment(id);
			return fResponse.ResponseOk;
		}

		/// <summary>
		/// Updates a fragment.
		/// </summary>
		/// <remarks>
		/// Sample request:
		///
		/// PUT /api/Stories/1/Fragments/2
		/// {
		///    "text": "some text"
		/// }
		///
		/// </remarks>
		/// <param name="fragmentId">The fragment ID</param>
		/// <param name="fragment">The fragment.</param>
		/// <response code="204">If the item was successfully added.</response>
		/// <response code="400">If the ID in the URL doesn't match the one in the body.</response>
		/// <response code="404">If the item is not found.</response>
		/// /// <response code="404">If something goes wrong.</response>
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[Authorize(AuthenticationSchemes = "Identity.Application,Bearer")]
		[HttpPut("{id}/Fragments/{fragmentId}")]
		public async Task<IActionResult> PutFragment(int fragmentId, FragmentViewModel fragment)
		{
			if (fragmentId != fragment.Id)
			{
				return BadRequest();
			}

			if (!_storyService.StoryExists(fragment.StoryId))
			{
				return NotFound();
			}

			if (!_storyService.FragmentExists(fragmentId))
			{
				return NotFound();
			}

			var fragmentResponse = await _storyService.UpdateFragment(_mapper.Map<Fragment>(fragment));

			if (fragmentResponse.ResponseError == null)
			{
				return NoContent();
			}

			return StatusCode(500);
		}

		/// <summary>
		/// Creates a fragment for a particular story.
		/// </summary>
		/// <remarks>
		/// Sample request:
		///
		/// POST /api/Stories/1/Fragments
		/// {
		///    "text": "some text"
		/// }
		///
		/// </remarks>
		/// <param name="id">The story ID</param>
		/// <param name="fragment">The fragment.</param>
		/// <response code="200">If the item was successfully added.</response>
		/// <response code="500">If something goes wrong.</response>  
		[Authorize(AuthenticationSchemes = "Identity.Application,Bearer")]
		[HttpPost("{id}/Fragments")]
		public async Task<IActionResult> PostFragmentForStory(int id, FragmentViewModel fragment)
		{
			var user = await _userManager.FindByNameAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value);
			var fragmentEntity = _mapper.Map<Fragment>(fragment);
			fragmentEntity.User = user;

			var commentResponse = await _storyService.AddFragmentToStory(id, fragmentEntity, fragment.IsLast);

			if (commentResponse.ResponseError == null)
			{
				return Ok();
			}

			return StatusCode(500);
		}

		/// <summary>
		/// Deletes a fragment from a story.
		/// </summary>
		/// <remarks>
		/// Sample request:
		///
		/// DELETE /api/Stories/1/Fragments/2
		///
		/// </remarks>
		/// <param name="fragmentId">The fragment ID.</param>
		/// <response code="204">No content if successful.</response>
		/// <response code="404">If the tag doesn't exist.</response>  
		/// <response code="500">If something goes wrong.</response>  
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[HttpDelete("{id}/Fragments/{fragmentId}")]
		[Authorize(AuthenticationSchemes = "Identity.Application,Bearer")]
		public async Task<IActionResult> DeleteFragment(int fragmentId)
		{
			if (!_storyService.FragmentExists(fragmentId))
			{
				return NotFound();
			}

			var result = await _storyService.DeleteFragment(fragmentId);

			if (result.ResponseError == null)
			{
				return NoContent();
			}

			return StatusCode(500);
		}

	}
}
