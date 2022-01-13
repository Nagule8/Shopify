namespace ShopApi.Models
{
    public class UserActivity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Method { get; set; }
        public string Description { get; set; }
    }
}
