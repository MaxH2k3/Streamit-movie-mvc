namespace Streamit_movie_mvc.Models.DTO
{
    public class LoginWithDTO
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Avatar { get; set; }
    }
}
