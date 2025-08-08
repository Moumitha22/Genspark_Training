using ChienVHShopOnline.Models.Enums;

namespace ChienVHShopOnline.Models
{
    public class User
    {
        public User()
        {
            News = new HashSet<News>();
            Products = new HashSet<Product>();
            Orders = new HashSet<Order>(); 
        }

        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public UserRole Role { get; set; }

        public virtual ICollection<News> News { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<Order> Orders { get; set; } = new HashSet<Order>();

    }
}
