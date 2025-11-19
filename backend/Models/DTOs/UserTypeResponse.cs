namespace CorretoraJenissonLuckwuAPI.Models.DTOs
{
    public class UserTypeResponse
    {
        #region Properties
        public string UserType { get; set; } // "Admin" ou "User"
        public bool Exists { get; set; }
        #endregion
    }
}

