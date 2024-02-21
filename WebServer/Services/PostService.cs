using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebServer.Data;
using WebServer.Data.Models;
using WebServer.Dto;
using WebServer.Dto.ReponseObjects;

namespace WebServer.Services
{
    public class PostService : IPostService
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PostService(ApplicationContext context, 
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<(BlogPostResponse response, int statusCode)> CreatePost(BlogPostDTO blogPost)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _context.BlogPosts.AddAsync(new Data.Models.BlogPost
            {
                AuthorId = int.Parse(userId),
                Description = blogPost.Description,
                Title = blogPost.Title,
            });

            await _context.SaveChangesAsync();
            var response = new BlogPostResponse
            {
                Title = result.Entity.Title,
                Author = result.Entity.AuthorId,
                Description = result.Entity.Description,
                Id = result.Entity.Id,
            };

            return (response, StatusCodes.Status201Created);
        }

        public async Task<int> DeletePost(int id)
        {
            var post = await _context.BlogPosts
                .Include(x => x.Comments)
                .FirstOrDefaultAsync(x => x.Id == id);
            
            if (post == null)
            {
                return StatusCodes.Status404NotFound;
            }

            _context.Comments.RemoveRange(post.Comments);
            _context.BlogPosts.Remove(post);
            await _context.SaveChangesAsync();
            return StatusCodes.Status200OK;
        }

        public async Task<(IEnumerable<BlogPostResponse> response, int statusCode)> GetAllPosts()
        {
            var result = await _context.BlogPosts
                .Include(x => x.Comments)
                .Select(x => new BlogPostResponse
                {
                    Author = x.AuthorId,
                    Description = x.Description,
                    Id = x.Id,
                    Title = x.Title,
                    Comments = x.Comments.Select(c => new CommentResponse
                    {
                        Author = c.AuthorId,
                        Description = c.Description,
                        Id = c.Id,
                        PostId = x.Id,
                        Title = c.Title
                    }).ToList(),
                }).ToListAsync();

            return (result, StatusCodes.Status200OK);
        }

        public async Task<(IEnumerable<BlogPostResponse> response, int statusCode)> GetPostsByAuthor(int id)
        {
            var result = await _context.BlogPosts
                .Where(x => x.AuthorId == id)
                .Include(x => x.Comments)
                .Select(x => new BlogPostResponse
                {
                    Author = x.AuthorId,
                    Description = x.Description,
                    Id = x.Id,
                    Title = x.Title,
                    Comments = x.Comments.Select(c => new CommentResponse
                    {
                        Author = c.AuthorId,
                        Description = c.Description,
                        Id = c.Id,
                        PostId = x.Id,
                        Title = c.Title
                    }).ToList(),
                }).ToListAsync();

            return (result, StatusCodes.Status200OK);
        }

        public async Task<(BlogPostResponse response, int statusCode)> GetPostById(int id)
        {
            var result = _context.BlogPosts
                .Where(x => x.Id == id)
                .Include(x => x.Comments)
                .Select(x => new BlogPostResponse
                {
                    Author = x.AuthorId,
                    Description = x.Description,
                    Id = x.Id,
                    Title = x.Title,
                    Comments = x.Comments.Select(c => new CommentResponse
                    {
                        Author = c.AuthorId,
                        Description = c.Description,
                        Id = c.Id,
                        PostId = x.Id,
                        Title = c.Title
                    }).ToList(),
                }).ToList().FirstOrDefault();

            return (result, StatusCodes.Status200OK);
        }

        public async Task<(BlogPostResponse response, int statusCode)> UpdatePost(BlogPostDTO blogPost, int id)
        {
            var post = await _context.BlogPosts
                .Include(x => x.Comments)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (post == null)
            {
                return (null, StatusCodes.Status404NotFound);
            }

            post.Description = blogPost.Description;
            post.Title = blogPost.Title;
            
            await _context.SaveChangesAsync();

            var result = new BlogPostResponse
            {
                Author = post.AuthorId,
                Description = post.Description,
                Id = post.Id,
                Title = post.Title,
                Comments = post.Comments.Select(c => new CommentResponse
                {
                    Author = c.AuthorId,
                    Description = c.Description,
                    Id = c.Id,
                    PostId = post.Id,
                    Title = c.Title
                }).ToList()
            };

            return (result, StatusCodes.Status200OK);
        }

        public async Task<(CommentResponse response, int statusCode)> CreateComment(CommentDto comment, int postId)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var post = _context.BlogPosts.FirstOrDefault(x => x.Id == postId);

            if (post == null)
            { 
                return (null, StatusCodes.Status404NotFound); 
            }

            var result = await _context.Comments.AddAsync(new Comment
            {
                AuthorId = int.Parse(userId),
                BlogPostId = postId,
                Description = comment.Description,
                Title = comment.Title
            });

            await _context.SaveChangesAsync();

            var response = new CommentResponse
            {
                Author = result.Entity.AuthorId,
                Description = result.Entity.Description,
                Id = result.Entity.Id,
                PostId = result.Entity.BlogPostId,
                Title = result.Entity.Title
            };

            return (response, StatusCodes.Status201Created);
        }

        public async Task<(CommentResponse response, int statusCode)> UpdateComment(CommentDto comment, int id)
        {
            var dbComment = await _context.Comments.FirstOrDefaultAsync(x => x.Id == id);
            if (dbComment == null)
            {
                return (null, StatusCodes.Status404NotFound);
            }

            dbComment.Title = comment.Title;
            dbComment.Description = comment.Description;

            await _context.SaveChangesAsync();

            var response = new CommentResponse
            {
                Author = dbComment.AuthorId,
                Title = dbComment.Title,
                Id = dbComment.Id,
                Description = dbComment.Description,
                PostId = dbComment.BlogPostId
            };

            return (response, StatusCodes.Status200OK);
        }

        public async Task<int> DeleteComment(int id)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(x => x.Id == id);

            if (comment == null)
            {
                return StatusCodes.Status404NotFound;
            }
            
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return StatusCodes.Status200OK;
        }
    }
}
