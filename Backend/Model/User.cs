namespace WebApi.Model
{
    public class User
    {
        public int ID { get; set; }
        public string Login { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // Enum: Admin, Casual
        public bool IsActive { get; set; }
        public int GroupID { get; set; }
        public UserGroup? UserGroup { get; set; }
    }

}
