namespace BlogPlatform.DTO.ReponseObjects
{
    public class RegistrationResponse
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public object Permissions { get; set; }
    }
}
