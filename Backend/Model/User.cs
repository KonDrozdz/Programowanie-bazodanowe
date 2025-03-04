namespace WebApi.Model
{
    public class User
    {
        public int ID { get; set; }
        public string Login { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserType Type { get; set; } = UserType.Casual; // Enum: Admin, Casual
        public bool IsActive { get; set; }
        public int GroupID { get; set; }
        public UserGroup? UserGroup { get; set; }
        public List<Order> Orders { get; set; }
        public List<BasketPosition> BasketPosition { get; set; }

    }
    public enum UserType
    {
        Admin,
        Casual
    }

}
