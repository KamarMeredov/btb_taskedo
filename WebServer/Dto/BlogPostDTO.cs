using System.ComponentModel.DataAnnotations;

namespace WebServer.Dto
{
    public class BlogPostDTO
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Title is required")]
        [MinLength(10)]
        public string Title { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage ="Description is required")]
        public string Description { get; set; }
    }
}
