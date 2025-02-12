namespace Web_Project.Models
{
    public class OrderHistory
    {
        public int Id { get; set; }
        public string  CustomerId { get; set; }
        public string  ItemName { get; set; }
        public float UnitPrice  { get; set; }
        public int Quantity  { get; set; }
        public DateTime Date  { get; set; }
    }
}
