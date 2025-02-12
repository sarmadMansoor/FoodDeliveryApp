using Web_Project.Models;
using System.Collections.Generic;

namespace Web_Project.Models.Interfaces
{
    public interface IOrderHistoryRepository
    {
        // Create
        OrderHistory AddOrderHistory(OrderHistory orderHistory);

        // Read
        OrderHistory GetOrderHistoryById(int id);
        List<OrderHistory> GetOrderHistoryByCustomerId(string customerId);
        List<OrderHistory> GetAllOrderHistories();

        // Update
        OrderHistory UpdateOrderHistory(OrderHistory orderHistory);

        // Delete
        bool DeleteOrderHistory(int id);
    }
}
