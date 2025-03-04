namespace WebApi.Model
{
    public class ProductGroup
    {
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? ParentID { get; set; }
        public ProductGroup? ParentGroup { get; set; }
        public List<Product> Products { get; set; } = new();
    }

}
