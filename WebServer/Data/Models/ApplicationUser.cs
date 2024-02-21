using Microsoft.AspNetCore.Identity;

namespace WebServer.Data.Models
{
    public class ApplicationUser : IdentityUser<int>
    {
        public List<BlogPost> BlogPosts { get; set; } = new List<BlogPost>();
        public List<Comment> Comments { get; set; } = new List<Comment>();
    }
}
