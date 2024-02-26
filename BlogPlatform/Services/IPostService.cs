using BlogPlatform.DTO;
using BlogPlatform.DTO.ReponseObjects;

namespace BlogPlatform.Services
{
    public interface IPostService
    {
        Task<BlogPostResponse> CreatePost(BlogPostDTO blogPost);
        Task DeletePost(int id);
        Task<BlogPostResponse> UpdatePost(BlogPostDTO blogPost, int id);
        Task<BlogPostResponse?> GetPostById(int id);
        Task<IEnumerable<BlogPostResponse>> GetPostsByAuthor(int id);
        Task<IEnumerable<BlogPostResponse>> GetAllPosts();

        Task<CommentResponse> CreateComment(CommentDto comment, int postId);
        Task<CommentResponse> UpdateComment(CommentDto comment, int id);
        Task DeleteComment(int id);

        Task<CommentResponse?> GetCommentById(int id);

    }
}
