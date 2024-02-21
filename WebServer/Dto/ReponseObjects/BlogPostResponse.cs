namespace WebServer.Dto.ReponseObjects
{
    public class BlogPostResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Author { get; set; }
        public List<CommentResponse> Comments { get; set; }
    }
}
