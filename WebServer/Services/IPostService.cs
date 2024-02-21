﻿using WebServer.Dto;
using WebServer.Dto.ReponseObjects;

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

    }
}
