namespace ChienVHShopOnline.Models
{
    public class Storage
    {
        public Storage()
        {
            Products = new HashSet<Product>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        // Navigation property
        public virtual ICollection<Product> Products { get; set; }
    }
}
