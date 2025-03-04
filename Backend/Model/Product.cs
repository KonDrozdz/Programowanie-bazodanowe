namespace WebApi.Model
{
    public class Product
    {
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Price { get; set; }
        public string Image { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int GroupID { get; set; }
        public ProductGroup? ProductGroup { get; set; }
        public List<OrderPosition> OrderPositions { get; set; }
        public List<BasketPosition > BasketPositions { get; set; }
    }

}
