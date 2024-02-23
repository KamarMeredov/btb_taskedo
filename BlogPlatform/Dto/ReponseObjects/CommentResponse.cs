namespace BlogPlatform.DTO.ReponseObjects
{
    public class CommentResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Author { get; set; }
        public int PostId { get; set; }
    }
}
