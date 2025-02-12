namespace Web_Project.Models.ViewModels
{
    public class UserOrderHistoryViewModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhotoUrl { get; set; }
        public List<OrderHistory> OrderHistory { get; set; }
    }
}
