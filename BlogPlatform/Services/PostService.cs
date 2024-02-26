using Microsoft.EntityFrameworkCore;
using BlogPlatform.Data;
using BlogPlatform.Data.Models;
using BlogPlatform.DTO;
using BlogPlatform.DTO.ReponseObjects;
using BlogPlatform.Helpers;

namespace BlogPlatform.Services
{
    public class PostService : IPostService
    {
        private readonly ApplicationContext _context;
        private readonly IUserContext _userContext;

        public PostService(ApplicationContext context, 
            IUserContext userContext)
        {
            _userContext = userContext;
            _context = context;
        }

        public async Task<BlogPostResponse> CreatePost(BlogPostDTO blogPost)
        {
            var userId = _userContext.UserId;
            
            if (userId == null)
            {
                throw new ObjectNotFoundException(nameof(userId));
            }

            var result = await _context.BlogPosts.AddAsync(new BlogPost
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

            return response;
        }

        public async Task DeletePost(int id)
        {
            var post = await _context.BlogPosts
                .SingleOrDefaultAsync(x => x.Id == id);
            
            if (post == null)
            {
                throw new ObjectNotFoundException(nameof(post));
            }

            _context.BlogPosts.Remove(post);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<BlogPostResponse>> GetAllPosts()
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
                })
                .AsNoTracking()
                .ToListAsync();

            return result;
        }

        public async Task<IEnumerable<BlogPostResponse>> GetPostsByAuthor(int id)
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
                })
                .AsNoTracking()
                .ToListAsync();

            return result;
        }

        public async Task<BlogPostResponse?> GetPostById(int id)
        {
            var result = await _context.BlogPosts
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
                })
                .AsNoTracking()
                .SingleOrDefaultAsync();

            return result;
        }

        public async Task<BlogPostResponse> UpdatePost(BlogPostDTO blogPost, int id)
        {
            var post = await _context.BlogPosts
                .Include(x => x.Comments)
                .SingleOrDefaultAsync(x => x.Id == id);

            if (post == null)
            {
                throw new ObjectNotFoundException(nameof(post));
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

            return result;
        }

        public async Task<CommentResponse> CreateComment(CommentDto comment, int postId)
        {
            var userId = _userContext.UserId;
            var post = _context.BlogPosts.SingleOrDefault(x => x.Id == postId);

            if (userId == null)
            {
                throw new ObjectNotFoundException(nameof(userId));
            }

            if (post == null)
            {
                throw new ObjectNotFoundException(nameof(post));
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

            return response;
        }

        public async Task<CommentResponse> UpdateComment(CommentDto comment, int id)
        {
            var dbComment = await _context.Comments.SingleOrDefaultAsync(x => x.Id == id);
            
            if (dbComment == null)
            {
                throw new ObjectNotFoundException(nameof(dbComment));
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

            return response;
        }

        public async Task DeleteComment(int id)
        {
            var comment = await _context.Comments.SingleOrDefaultAsync(x => x.Id == id);
            
            if (comment == null)
            {
                throw new ObjectNotFoundException(nameof(comment));
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
        }

        public async Task<CommentResponse?> GetCommentById(int id)
        {
            var comment = await _context.Comments
                .Where(x => x.Id == id)
                .Select(x => new CommentResponse
                {
                    Author = x.AuthorId,
                    Title = x.Title,
                    Id = x.Id,
                    Description = x.Description,
                    PostId = x.BlogPostId
                })
                .SingleOrDefaultAsync();

            return comment;
        }
    }
}
