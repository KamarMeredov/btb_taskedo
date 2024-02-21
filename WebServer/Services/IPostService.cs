using WebServer.DTO;
using WebServer.DTO.ReponseObjects;

namespace WebServer.Services
{
    public interface IPostService
    {
        Task<(BlogPostResponse response, int statusCode)> CreatePost(BlogPostDTO blogPost);
        Task<int> DeletePost(int id);
        Task<(BlogPostResponse response, int statusCode)> UpdatePost(BlogPostDTO blogPost, int id);
        Task<(BlogPostResponse response, int statusCode)> GetPostById(int id);
        Task<(IEnumerable<BlogPostResponse> response, int statusCode)> GetPostsByAuthor(int id);
        Task<(IEnumerable<BlogPostResponse> response, int statusCode)> GetAllPosts();

        Task<(CommentResponse response, int statusCode)> CreateComment(CommentDto comment, int postId);
        Task<(CommentResponse response, int statusCode)> UpdateComment(CommentDto comment, int id);
        Task<int> DeleteComment(int id);

    }
}
