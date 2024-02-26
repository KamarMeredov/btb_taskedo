namespace BlogPlatform.Data.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public int BlogPostId { get; set; }
        public BlogPost BlogPost { get; set; }

        public int AuthorId { get; set; }
        public ApplicationUser Author { get; set; }
    }
}
