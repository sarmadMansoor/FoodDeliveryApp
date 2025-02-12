using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_Project.Data;
using Web_Project.Models;
using Web_Project.Models.ViewModels;
using Web_Project.Models.Interfaces;


namespace Web_Project.Controllers
{
   

    public class AdminController : Controller
    {
        private readonly IOrderHistoryRepository _orderHistoryRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productRepository;
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;



        public AdminController(IOrderHistoryRepository orderHistoryRepository ,ApplicationDbContext context, UserManager<User> userManager,ICategoryRepository categoryRepository, IProductRepository productRepository)
        {
            _orderHistoryRepository = orderHistoryRepository;
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _userManager=   userManager;
            _context = context;
        }


        // GET: Display all categories
        public IActionResult Categories()
        {
            // Fetch all categories from the Categories table
            var categories = _context.Categories.Select(category => new CategoryViewModel
            {
                Id = category.CategoryId,
                Name = category.Name,
                Description = category.Description
            }).ToList();

            return View(categories);
        }

        // POST: Remove category by Id
        [HttpPost]
        public IActionResult RemoveCategory(int id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.CategoryId == id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Category removed successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Category not found.";
            }

            return RedirectToAction("Categories");
        }

        // GET: Display all users
        public IActionResult Users()
        {
            // Fetch all users from the AspNetUsers table
            var users = _context.Users.Select(user => new UserViewModel
            {
                Id = user.Id,
                FullName = user.FullName, // Assuming you have this field in the database
                UserName = user.UserName,
                Address = user.Address // Assuming you have an Address field
            }).ToList();

            return View(users);
        }

        // POST: Remove user by Id
        [HttpPost]
        public async Task<IActionResult> RemoveUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "User removed successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Error occurred while removing the user.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "User not found.";
            }

            return RedirectToAction("Users");
        }
        public IActionResult Index()
        {
            ViewData["TotalOrders"] = _orderHistoryRepository.GetAllOrderHistories().Count;
            ViewData["TotalUsers"] = _context.Users.Count(); // AspNetUsers table
            ViewData["TotalProducts"] = _productRepository.GetAllProducts().Count;
            ViewData["TotalCategories"] = _categoryRepository.GetAllCategories().Count;

            return View();
        }


        [HttpGet]
        public IActionResult AddCategory()
        {
            return View();
        }

        // POST: Add a new category
        [HttpPost]
        public IActionResult AddCategory(Category category)
        {
             category.Products = null;
                _categoryRepository.AddCategory(category);
                TempData["SuccessMessage"] = "Category added successfully!";
                return RedirectToAction("AddCategory");
            

            return View(category);
        }
        [HttpGet]
        public IActionResult AddProduct()
        {
            var categories = _categoryRepository.GetAllCategories(); // Fetch all categories
            ViewBag.Categories = categories; // Pass categories to the view
            return View();
        }
        [HttpPost]
        public IActionResult AddProduct(Product product, IFormFile productImage, int CategoryId)
        {
    
            // Handle file upload if an image is provided
            if (productImage != null && productImage.Length > 0)
            {
                string wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                string uploadFolderPath = Path.Combine(wwwRootPath, "Uploads");

                if (!Directory.Exists(uploadFolderPath))
                {
                    Directory.CreateDirectory(uploadFolderPath);
                }

                string uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(productImage.FileName)}";
                string filePath = Path.Combine(uploadFolderPath, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    productImage.CopyTo(stream);
                }

                product.ImagePath = $"/Uploads/{uniqueFileName}";
            }


            // Assign the selected category
            var selectedCategory = _categoryRepository.GetCategoryById(CategoryId);
            if (selectedCategory == null)
            {
                ModelState.AddModelError("", "Invalid category selected.");
                ViewBag.Categories = _categoryRepository.GetAllCategories();
                return View(product);
            }
            product.Category = selectedCategory;

            // Save product to repository
            try
            {
                _productRepository.AddProduct(product);
                TempData["SuccessMessage"] = "Product added successfully!";
                return RedirectToAction("AddProduct");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error saving product: {ex.Message}");
                ViewBag.Categories = _categoryRepository.GetAllCategories();
                return View(product);
            }
        }





        public IActionResult Products()
        {
            // Fetch products grouped by category
            var categories = _context.Categories
                .Select(category => new CategoryWithProductsViewModel
                {
                    CategoryName = category.Name,
                    Products = category.Products.Select(product => new ProductViewModel
                    {
                        Id = product.ProductId,
                        Name = product.Name,
                        Price = product.Price,
                        PhotoPath = product.ImagePath,
                        Quantity = product.Stock
                    }).ToList()
                }).ToList();

            return View(categories);
        }

        [HttpPost]
        public IActionResult RemoveProduct(int productId)
        {
            var product = _context.products.Find(productId);
            if (product != null)
            {
                _context.products.Remove(product);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Product removed successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Product not found.";
            }

            return RedirectToAction("Products");
        }
        // GET: Edit product (for demonstration purposes, a detailed edit logic would be added)
      




        public async Task<IActionResult> OrderHistory(string id)
        {
            // Fetch the user details asynchronously
            User user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("Index"); // Redirect to the admin dashboard or users list
            }

            // Fetch the order history for the user
            var orders = _orderHistoryRepository.GetOrderHistoryByCustomerId(id);

            // Create a ViewModel to pass data to the view
            var model = new UserOrderHistoryViewModel
            {
                FullName = user.FullName, // Assuming the user model has FirstName and LastName properties
                Email = user.Email,
                PhotoUrl = user.ProfilePicturePath, // Assuming the user model has a PhotoUrl property
                OrderHistory = orders
            };

            return View(model);
        }
        // GET: Edit Product
        public ActionResult EditProduct(int id)
        {
            var product = _context.products.FirstOrDefault(p => p.ProductId == id);
            if (product == null)
            {
                TempData["ErrorMessage"] = "Product not found.";
                return RedirectToAction("Index");
            }

            return View(product);
        }

        // POST: Update Product
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProduct(Product model)
        {
            var product = _context.products.FirstOrDefault(p => p.ProductId == model.ProductId);
            if (product != null)
            {
                product.Name = model.Name;
                product.Price = model.Price;
                product.Stock = model.Stock;
                product.Description = model.Description;

                _context.SaveChanges();

                TempData["SuccessMessage"] = "Product updated successfully.";
                return RedirectToAction("Products");
            }

            TempData["ErrorMessage"] = "Error updating the product.";
            return View(model);
        }





    }

}
