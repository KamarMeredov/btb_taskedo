using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using BlogPlatform.DTO;
using BlogPlatform.DTO.ReponseObjects;
using BlogPlatform.Services;

namespace BlogPlatform.Controllers
{
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class PostController : Controller
    {
        private readonly IPostService _postService;
        public PostController(IPostService postService) 
        {
            _postService = postService;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost([FromBody] BlogPostDTO blogPost)
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult((new
                {
                    Message = ModelState.FirstOrDefault(x => x.Value!.ValidationState == ModelValidationState.Invalid)
                    .Value!.Errors.FirstOrDefault()!.ErrorMessage
                }))
                { StatusCode = StatusCodes.Status400BadRequest };
            }

            var result = await _postService.CreatePost(blogPost);
            return new JsonResult(result) { StatusCode = StatusCodes.Status201Created };
        }

        [HttpDelete]
        public async Task<IActionResult> DeletePost([FromQuery] int id)
        {
            var post = await _postService.GetPostById(id);
            
            if (post == null)
            {
                return NotFound();
            }

            await _postService.DeletePost(id);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetPostById([FromQuery] int id)
        {
            var result = await _postService.GetPostById(id);

            if (result == null)
            {
                return NotFound();
            }

            return new JsonResult(result) { StatusCode = StatusCodes.Status200OK };
        }

        [HttpGet]
        public async Task<IActionResult> GetPostByAuthor([FromQuery] int id)
        {
            var result = await _postService.GetPostsByAuthor(id);
            return new JsonResult(result) { StatusCode = StatusCodes.Status200OK };
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPosts()
        {
            var result = await _postService.GetAllPosts();
            return new JsonResult(result) { StatusCode = StatusCodes.Status200OK };
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePost([FromBody] BlogPostDTO blogPost, [FromQuery] int id)
        {
            if( !ModelState.IsValid )
            {
                return new JsonResult((new
                {
                    Message = ModelState.FirstOrDefault(x => x.Value!.ValidationState == ModelValidationState.Invalid)
                    .Value!.Errors.FirstOrDefault()!.ErrorMessage
                }))
                { StatusCode = StatusCodes.Status400BadRequest };
            }

            var post = await _postService.GetPostById(id);
            
            if ( post == null )
            {
                return NotFound();
            }

            var result = await _postService.UpdatePost(blogPost, id);
            return new JsonResult(result) { StatusCode = StatusCodes.Status200OK };
        }


        [HttpPost]
        public async Task<IActionResult> CreateComment([FromBody] CommentDto comment, [FromQuery] int postId)
        {
            if( !ModelState.IsValid )
            {
                return new JsonResult((new
                {
                    Message = ModelState.FirstOrDefault(x => x.Value!.ValidationState == ModelValidationState.Invalid)
                    .Value!.Errors.FirstOrDefault()!.ErrorMessage
                }))
                { StatusCode = StatusCodes.Status400BadRequest };
            }

            var post = await _postService.GetPostById(postId);
            
            if (post == null )
            {
                return NotFound();
            }

            var result = await _postService.CreateComment(comment, postId);
            return new JsonResult(result) { StatusCode = StatusCodes.Status200OK };
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteComment([FromQuery] int id)
        {
            var comment = await _postService.GetCommentById(id);
            
            if (comment == null)
            {
                return NotFound();
            }

            await _postService.DeleteComment(id);
            return StatusCode(StatusCodes.Status200OK);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateComment([FromBody] CommentDto comment, int id)
        {
            if( !ModelState.IsValid )
            {
                return new JsonResult((new
                {
                    Message = ModelState.FirstOrDefault(x => x.Value!.ValidationState == ModelValidationState.Invalid)
                    .Value!.Errors.FirstOrDefault()!.ErrorMessage
                }))
                { StatusCode = StatusCodes.Status400BadRequest };
            }

            var dbComment = await _postService.GetCommentById(id);

            if (dbComment == null)
            {
                return NotFound();
            }

            var result = await _postService.UpdateComment(comment, id);
            return new JsonResult(result) { StatusCode = StatusCodes.Status200OK };
        }
    }
}
