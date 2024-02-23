namespace BlogPlatform.DTO.ReponseObjects
{
    public class LoginResponse
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public object Permissions { get; set; }
        public string AccessToken { get; set; }
    }
}
