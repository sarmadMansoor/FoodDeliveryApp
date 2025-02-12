using System.Collections.Generic;
using System.Linq;
using Web_Project.Data;
using Web_Project.Models;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public List<Product> GetProductsByCategory(int categoryId)
    {
        // Check if the categoryId is valid
        return _context.products
                       // Include the navigation property
                       .Where(p => p.Category.CategoryId == categoryId)
                       .ToList();
    }


    public Product AddProduct(Product product)
    {
        _context.products.Add(product);
        _context.SaveChanges(); // Blocking call
        return product;
    }
    public int GetProductQuantity(int productId)
    {
        // Retrieve the product by its ID and return the stock quantity
        var product = _context.products.FirstOrDefault(p => p.ProductId == productId);

        // Check if the product exists
        if (product == null)
        {
            throw new ArgumentException("Product not found", nameof(productId));
        }

        return product.Stock;
    }
    public  Product GetProductById(int id)
    {
        return _context.products.Find(id);
    }

    public Product GetProductByName(string name)
    {
        return _context.products.FirstOrDefault(p => p.Name == name); // Blocking call
    }

    public List<Product> GetAllProducts()
    {
        return _context.products.ToList(); // Blocking call

    }
     public List<Product> GetProductsByCategoryName(string categoryName)
    {
        return _context.products.Where(p => p.Category.Name == categoryName).ToList(); // Blocking call

    }
    public Product UpdateProductQuantity(int productId, int quantity)
    {
        // Retrieve the product by its ID
        var product = _context.products.FirstOrDefault(p => p.ProductId == productId);

        // Check if the product exists
        if (product == null)
        {
            throw new ArgumentException("Product not found", nameof(productId));
        }

        // Update the product's stock with the new quantity
        product.Stock = quantity;

        // Save the changes
        _context.products.Update(product);
        _context.SaveChanges();

        return product;
    }



    public Product UpdateProduct(Product product)
    {
        _context.products.Update(product);
        _context.SaveChanges(); // Blocking call
        return product;
    }

    public bool DeleteProduct(string name )
    {
        var product = _context.products.FirstOrDefault(p => p.Name == name); // Blocking call
        if (product == null) return false;

        _context.products.Remove(product);
        _context.SaveChanges(); // Blocking call
        return true;
    }
}
