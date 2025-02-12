using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Web_Project.Data;
using Web_Project.Models;
using Newtonsoft.Json;
using System.Security.Claims;
using Web_Project.Models.Interfaces;


namespace Web_Project.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IProductRepository _productRepository;
        private readonly IOrderHistoryRepository _orderHistoryRepository;



        public CartController(IOrderHistoryRepository orderHistoryRepository,ApplicationDbContext context ,IProductRepository productRepository)
        {
            _context = context;
            _productRepository = productRepository;
            _orderHistoryRepository = orderHistoryRepository;

        }
        public IActionResult AddToCart(int categoryId, int productId, int quantity = 1)
        {
            if (!User.Identity.IsAuthenticated)
            {
                // If not logged in, redirect to login page
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            // Get the user's email
            var userEmail = User.Identity.Name;

            // Sanitize the email to make it a valid cookie name (replace '@' with '_')
            var sanitizedEmail = userEmail.Replace('@', '_').Replace('.', '_'); // You can replace other characters if needed

            // Initialize a list to store cart items
            List<CartItemViewModel> cartItems;

            // Define CookieOptions object
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true // Ensures security by preventing client-side scripts from accessing the cookie
            };

            // Check if the cookie exists
            if (Request.Cookies.ContainsKey(sanitizedEmail))
            {
                // Deserialize the cookie value into a list of CartItemViewModel
                var cartCookie = Request.Cookies[sanitizedEmail];
                cartItems = JsonConvert.DeserializeObject<List<CartItemViewModel>>(cartCookie);

                // Check if the product already exists in the cart
                var existingItem = cartItems.FirstOrDefault(c => c.ProductId == productId);

                if (existingItem != null)
                {
                    // Update the quantity
                    existingItem.Quantity += quantity;
                }
                else
                {
                    // Add a new product to the cart
                    cartItems.Add(new CartItemViewModel
                    {
                        CategoryId = categoryId,
                        ProductId = productId,
                        Quantity = quantity
                    });
                }

                // Serialize back to the cookie without setting expiration again
                var updatedCartCookie = JsonConvert.SerializeObject(cartItems);
                Response.Cookies.Append(sanitizedEmail, updatedCartCookie, cookieOptions);
            }
            else
            {
                // Initialize a new cart for the user
                cartItems = new List<CartItemViewModel>
                {
                    new CartItemViewModel
                    {
                        CategoryId = categoryId,
                        ProductId = productId,
                        Quantity = quantity
                    }   
                 };

                // Set the expiration only during cookie creation
                cookieOptions.Expires = DateTimeOffset.Now.AddDays(7);

                // Serialize and create the new cookie
                var newCartCookie = JsonConvert.SerializeObject(cartItems);
                Response.Cookies.Append(sanitizedEmail, newCartCookie, cookieOptions);
            }

            // Redirect to the cart view or another appropriate page
            return RedirectToAction("Index", "Menu");
        }


        // POST: Main Cart
        public IActionResult MainCart()
        {
            // Check if the user is authenticated
            if (!User.Identity.IsAuthenticated)
            {
                // Redirect to the login page if not authenticated
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            // Get the user's email
            var userEmail = User.Identity.Name;

            // Sanitize the email to make it a valid cookie name
            var sanitizedEmail = userEmail.Replace('@', '_').Replace('.', '_');

            // Check if the cart cookie exists
            if (!Request.Cookies.ContainsKey(sanitizedEmail))
            {
                // If no cart cookie, pass an empty list to the view
                return View(new List<ShoppingCartItem>());
            }

            // Retrieve the cart items from the cookie
            var cartCookie = Request.Cookies[sanitizedEmail];
            var cartItems = JsonConvert.DeserializeObject<List<CartItemViewModel>>(cartCookie);

            // Check if cartItems is null or empty
            if (cartItems == null || !cartItems.Any())
            {
                return View(new List<ShoppingCartItem>());
            }

            // Get the product IDs from the cart items
            var productIds = cartItems.Select(ci => ci.ProductId).ToList();

            List<ShoppingCartItem> cartWithDetails = new List<ShoppingCartItem>();

            // Attach quantities and product details from the cart
            foreach (var cartItem in cartItems)
            {
                var product = _productRepository.GetProductById(cartItem.ProductId);

                // If the product exists, add it with its total price
                if (product != null)
                {
                    // Create a new ShoppingCartItem with the quantity and product details
                    cartWithDetails.Add(new ShoppingCartItem
                    {
                        ProductId = product.ProductId,
                        ProductName = product.Name,
                        ProductImage = product.ImagePath,
                        ProductPrice = product.Price,
                        Quantity = cartItem.Quantity, // Using the quantity from the cart cookie
                        TotalAmount= product.Price * cartItem.Quantity
                    });
                }
            }

            // Calculate grand total for the cart
            double grandTotal = cartWithDetails.Sum(ci => ci.TotalAmount);

            // Return the list of products with their total price and grand total
            ViewBag.GrandTotal = grandTotal;

            return View(cartWithDetails);
        }


        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            var userEmail = User.Identity.Name;
            var sanitizedEmail = userEmail.Replace('@', '_').Replace('.', '_');

            if (Request.Cookies.ContainsKey(sanitizedEmail))
            {
                var cartCookie = Request.Cookies[sanitizedEmail];
                var cartItems = JsonConvert.DeserializeObject<List<CartItemViewModel>>(cartCookie);

                // Find and remove the product
                var cartItem = cartItems.FirstOrDefault(ci => ci.ProductId == productId);
                if (cartItem != null)
                {
                    cartItems.Remove(cartItem);

                    // Serialize the updated cart and store it back in the cookie
                    var updatedCartCookie = JsonConvert.SerializeObject(cartItems);
                    Response.Cookies.Append(sanitizedEmail, updatedCartCookie);
                }
            }

            // After removing the item, redirect back to the cart page to reload
            return RedirectToAction("MainCart");
        }

        // Update Cart Quantity (AJAX handler for updating quantity)
        [HttpPost]
        public IActionResult UpdateCartQuantity(int productId, int quantity)
        {
            var userEmail = User.Identity.Name;
            var sanitizedEmail = userEmail.Replace('@', '_').Replace('.', '_');

            if (Request.Cookies.ContainsKey(sanitizedEmail))
            {
                var cartCookie = Request.Cookies[sanitizedEmail];
                var cartItems = JsonConvert.DeserializeObject<List<CartItemViewModel>>(cartCookie);

                var cartItem = cartItems.FirstOrDefault(ci => ci.ProductId == productId);
                if (cartItem != null)
                {
                    var product = _productRepository.GetProductById(productId);
                    if (product != null)
                    {
                        if (quantity > product.Stock)
                        {
                            quantity = product.Stock;  // Limit the quantity to available stock
                        }
                        cartItem.Quantity = quantity;
                    }

                    // Serialize the updated cart and store it in the cookie
                    var updatedCartCookie = JsonConvert.SerializeObject(cartItems);
                    Response.Cookies.Append(sanitizedEmail, updatedCartCookie, new CookieOptions
                    {
                        Expires = DateTimeOffset.Now.AddDays(7),
                        HttpOnly = true
                    });
                }
            }

            return RedirectToAction("MainCart");
        }



        // Inside your action method:
        public IActionResult Payment(string cartItems)
        {
            if (!string.IsNullOrEmpty(cartItems))
            {
                // Deserialize the JSON string into a List<ShoppingCartItem>
                var items = JsonConvert.DeserializeObject<List<ShoppingCartItem>>(cartItems);

                // Pass the deserialized items to the view
                return View(items);
            }

            // Handle case where cartItems is null or empty
            return View(new List<ShoppingCartItem>());
        }
    


        public IActionResult Index()
        {
            return View();
        }


        public IActionResult Checkout(string  cartItemsString)
        {
            List<ShoppingCartItem> cartItems = null;
            if (!string.IsNullOrEmpty(cartItemsString))
            {
                // Deserialize the JSON string into a List<ShoppingCartItem>
                 cartItems = JsonConvert.DeserializeObject<List<ShoppingCartItem>>(cartItemsString);

                // Pass the deserialized items to the view
            }

            // Handle case where cartItems is null or empty
            // Validate the cart items
            if (cartItems == null || !cartItems.Any())
            {
                return BadRequest("No items in the cart.");
            }

            // Get the current user's ID (assuming Identity is configured)
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not logged in.");
            }

            foreach (var item in cartItems)
            {
                // Get the product quantity from the database
                var productQuantity = _productRepository.GetProductQuantity(item.ProductId);

                // Check if there is enough stock
                if (productQuantity < item.Quantity)
                {
                    // Handle the case where there is insufficient stock
                    return BadRequest($"Not enough stock for product: {item.ProductName}. Available: {productQuantity}, Requested: {item.Quantity}");
                }

                // Subtract the purchased quantity from the stock
                int updatedQuantity = productQuantity - item.Quantity;

                // Update the product quantity in the database
                _productRepository.UpdateProductQuantity(item.ProductId, updatedQuantity);

                // Create an OrderHistory entry
                var orderHistory = new OrderHistory
                {
                    CustomerId = userId,
                    ItemName = item.ProductName,
                    UnitPrice = (float)item.ProductPrice,
                    Quantity = item.Quantity,
                    Date = DateTime.Now
                };

                // Save the order history to the database
                _orderHistoryRepository.AddOrderHistory(orderHistory);
            }
            var userEmail = User.Identity.Name;
            var sanitizedEmail = userEmail.Replace('@', '_').Replace('.', '_');
            Response.Cookies.Delete(sanitizedEmail);
            return RedirectToAction("DisplayOnlyCartItems", new { cartItemsString });
        }
        public IActionResult DisplayOnlyCartItems(string cartItemsString)
        {
            if (string.IsNullOrEmpty(cartItemsString))
            {
                return BadRequest("Cart items string is missing.");
            }

            // Deserialize the JSON string into a List<ShoppingCartItem>
            var cartItems = JsonConvert.DeserializeObject<List<ShoppingCartItem>>(cartItemsString);

            if (cartItems == null || !cartItems.Any())
            {
                return BadRequest("No items in the cart.");
            }

            // Convert ShoppingCartItem list to OrderHistory list
            var orderHistoryList = cartItems.Select(item => new OrderHistory
            {
                CustomerId = "DummyCustomerId", // Replace with actual CustomerId logic if needed
                ItemName = item.ProductName,
                UnitPrice = (float)item.ProductPrice,
                Quantity = item.Quantity,
                Date = DateTime.Now // Set the current date as the order date
            }).ToList();

            // Pass the OrderHistory list to the view
            return View("OrderHistory", orderHistoryList);
        }


        [HttpGet]
        public IActionResult OrderHistory()
        {
            // Get the current user's ID (assuming Identity is configured)
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not logged in.");
            }

            // Fetch order history for the current user
            var orderHistories = _orderHistoryRepository.GetOrderHistoryByCustomerId(userId);

            return View(orderHistories);
        }


        public class CartItemViewModel
        {
            public int CategoryId { get; set; }
            public int ProductId { get; set; }
            public int Quantity { get; set; }
        }

    }
}
