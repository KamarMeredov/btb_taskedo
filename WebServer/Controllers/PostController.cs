using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebServer.Dto;
using WebServer.Services;

namespace WebServer.Controllers
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

        // Posts CRUD logic
        [HttpPost]
        public async Task<IActionResult> CreatePost([FromBody] BlogPostDTO blogPost)
        {
            try
            {
                var result = await _postService.CreatePost(blogPost);
                return new JsonResult(result.response) { StatusCode = result.statusCode };
            } 
            catch (Exception ex) 
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeletePost([FromQuery] int id)
        {
            try
            {
                var result = await _postService.DeletePost(id);
                return StatusCode(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPostById([FromQuery] int id)
        {
            try
            {
                var result = await _postService.GetPostById(id);
                return new JsonResult(result.response) { StatusCode = result.statusCode };
            }
            catch( Exception ex )
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPostByAuthor([FromQuery] int id)
        {
            try
            {
                var result = await _postService.GetPostsByAuthor(id);
                return new JsonResult(result.response) { StatusCode = result.statusCode };
            }
            catch (Exception ex) 
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPosts()
        {
            try
            {
                var result = await _postService.GetAllPosts();
                return new JsonResult(result.response) { StatusCode = result.statusCode };
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePost([FromBody] BlogPostDTO blogPost, [FromQuery] int id)
        {
            try
            {
                var result = await _postService.UpdatePost(blogPost, id);
                return new JsonResult(result.response) { StatusCode = result.statusCode };
            }
            catch(Exception ex )
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        // Comments CRUD logic
        [HttpPost]
        public async Task<IActionResult> CreateComment([FromBody] CommentDto comment, [FromQuery] int postId)
        {
            try
            {
                var result = await _postService.CreateComment(comment, postId);
                return new JsonResult(result.response) { StatusCode = result.statusCode };
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteComment([FromQuery] int id)
        {
            try
            {
                var result = await _postService.DeleteComment(id);
                return StatusCode(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateComment([FromBody] CommentDto comment, int id)
        {
            try
            {
                var result = await _postService.UpdateComment(comment, id);
                return new JsonResult(result.response) { StatusCode = result.statusCode };
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
