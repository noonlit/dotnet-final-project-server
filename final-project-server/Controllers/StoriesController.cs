using AutoMapper;
using FinalProject.Models;
using FinalProject.Services;
using FinalProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
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

		public StoriesController(IMapper mapper, IStoryManagementService storyService)
		{
			_mapper = mapper;
			_storyService = storyService;
		}

		/// <summary>
		/// Retrieves a list of stories, filtered by genre.
		/// </summary>
		/// <remarks>
		/// Sample request:
		/// GET /api/Stories/filter/Humour
		/// </remarks>
		/// <param name="genre"></param>
		/// <response code="200">The filtered stories.</response>
		[HttpGet]
		[Route("filter/{genre}")]
		public async Task<ActionResult<PaginatedResultSet<StoryViewModel>>> GetFilteredStories(string genre, int? page = 1, int? perPage = 10)
		{
			var result = await _storyService.GetFilteredStories(genre, page, perPage);
			return result.ResponseOk;
		}

		/// <summary>
		/// Retrieves a list of stories.
		/// </summary>
		/// <remarks>
		/// Sample request:
		/// GET /api/Stories
		/// </remarks>
		/// <response code="200">The stories.</response>
		[HttpGet]
		public async Task<ActionResult<PaginatedResultSet<StoryViewModel>>> GetStories(int? page = 1, int? perPage = 10)
		{
			var result = await _storyService.GetStories();
			return result.ResponseOk;
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
		///		"id": 5
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
		/// <response code="404">If the item is not found.</response>
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
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

		/// <summary>
		/// Updates a story comment.
		/// </summary>
		/// <remarks>
		/// Sample request:
		///
		/// PUT: api/Stories/1/Comments/2
		/// {
		///    "text": "some comment",
		///    "important": false,
		///    "movieId": 3,
		/// }
		///
		/// </remarks>
		/// <param name="commentId">The comment ID</param>
		/// <param name="comment">The comment body</param>
		/// <response code="204">If the item was successfully added.</response>
		/// <response code="400">If the ID in the URL doesn't match the one in the body.</response>
		/// <response code="404">If the item is not found.</response>
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[HttpPut("{id}/Comments/{commentId}")]
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
		/// <response code="400">If the item is null or the rating is not a value between 1 and 10.</response>
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[HttpPost]
		[Authorize(AuthenticationSchemes = "Identity.Application,Bearer")]
		public async Task<ActionResult<Story>> PostStory(StoryViewModel story)
		{
			var response = await _storyService.CreateStory(_mapper.Map<Story>(story));

			if (response.ResponseError == null)
			{
				return CreatedAtAction("GetStory", new { id = story.Id }, story);
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
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
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

		[Authorize(AuthenticationSchemes = "Identity.Application,Bearer")]
		[HttpPost("{id}/Fragments")]
		public async Task<IActionResult> PostFragmentForStory(int id, FragmentViewModel fragment)
		{
			var commentResponse = await _storyService.AddFragmentToStory(id, _mapper.Map<Fragment>(fragment));

			if (commentResponse.ResponseError == null)
			{
				return Ok();
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
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
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
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
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


		/// <summary>
		/// Retrieves a story's fragments by its ID.
		/// </summary>
		/// <remarks>
		/// Sample request:
		/// GET api/Stories/5/Fragments
		/// </remarks>
		/// <param name="id">The story ID</param>
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

			var commentsResponse = await _storyService.GetFragmentsForStory(id, page, perPage);
			return commentsResponse.ResponseOk;
		}
	}
}
