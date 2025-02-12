using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Web_Project.Models;

namespace Web_Project.Controllers
{
    public class MenuController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productRepository;

        public MenuController(ICategoryRepository categoryRepository, IProductRepository productRepository)
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
        }

        public IActionResult Index()
        {
            // Retrieve data
            var categories = _categoryRepository.GetAllCategories();
            var products = _productRepository.GetAllProducts();

            // Combine data into a ViewModel or ViewBag
            var viewModel = new MenuViewModel
            {
                Categories = categories,
                Products = products
            };

            // Pass data to the view
            return View(viewModel);
        }
        [HttpGet]
        public IActionResult GetProductsByCategory(int categoryId)
        {
            // Fetch products based on categoryId
            var products = categoryId == 0
                ? _productRepository.GetAllProducts() // All products when categoryId is 0
                : _productRepository.GetProductsByCategory(categoryId);

            // Return products as JSON
            return Json(products);
        }
    }

    // ViewModel to send data to the View
    public class MenuViewModel
    {
        public List<Category> Categories { get; set; }
        public List<Product> Products { get; set; }
    }
}
