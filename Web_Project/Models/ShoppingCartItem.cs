namespace Web_Project.Models
{
    public class ShoppingCartItem
    {


            public int ProductId { get; set; }
            public string ProductName { get; set; }  // Renamed 'Name' to 'ProductName'
            public string ProductImage { get; set; }  // Renamed 'ImagePath' to 'ProductImage'
            public double ProductPrice { get; set; }  // Renamed 'Price' to 'ProductPrice'
            public int Quantity { get; set; }
            public double TotalAmount { get; set; }  // Renamed 'TotalPrice' to 'TotalAmount'
        
    }
}
