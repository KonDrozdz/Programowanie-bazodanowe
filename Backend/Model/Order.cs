namespace WebApi.Model
{
    public class Order
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public User? User { get; set; }
        public DateTime Date { get; set; }
        public List<OrderPosition> OrderPositions { get; set; }
    }

}
