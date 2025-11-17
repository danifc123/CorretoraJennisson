namespace CorretoraJenissonLuckwuAPI.Models.DTOs
{
    public class LoginResponse
    {
        #region Properties
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
        public int UserId { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        #endregion
    }
}

