using Web_Project.Models;
using System.Collections.Generic;
using System.Linq;
using Web_Project.Models.Interfaces;

namespace Web_Project.Data
{
    public class OrderHistoryRepository : IOrderHistoryRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderHistoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Create
        public OrderHistory AddOrderHistory(OrderHistory orderHistory)
        {
            _context.OrderHistories.Add(orderHistory);
            _context.SaveChanges();
            return orderHistory;
        }

        // Read
        public OrderHistory GetOrderHistoryById(int id)
        {
            return _context.OrderHistories.FirstOrDefault(o => o.Id == id);
        }

        public List<OrderHistory> GetOrderHistoryByCustomerId(string customerId)
        {
            return _context.OrderHistories.Where(o => o.CustomerId == customerId).ToList();
        }

        public List<OrderHistory> GetAllOrderHistories()
        {
            return _context.OrderHistories.ToList();
        }

        // Update
        public OrderHistory UpdateOrderHistory(OrderHistory orderHistory)
        {
            _context.OrderHistories.Update(orderHistory);
            _context.SaveChanges();
            return orderHistory;
        }

        // Delete
        public bool DeleteOrderHistory(int id)
        {
            var orderHistory = _context.OrderHistories.FirstOrDefault(o => o.Id == id);
            if (orderHistory == null)
                return false;

            _context.OrderHistories.Remove(orderHistory);
            _context.SaveChanges();
            return true;
        }
    }
}
