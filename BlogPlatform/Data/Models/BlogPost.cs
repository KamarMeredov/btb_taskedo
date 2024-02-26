namespace BlogPlatform.Data.Models
{
    public class BlogPost
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        
        public int AuthorId { get; set; }
        public ApplicationUser Author { get; set; }
        public List<Comment> Comments { get; set; } = new List<Comment>();
    }
}
