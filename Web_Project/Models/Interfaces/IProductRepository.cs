using Web_Project.Models;

public interface IProductRepository
{
    // Create
    Product AddProduct(Product product);
    public List<Product> GetProductsByCategory(int categoryId);


    // Read
    Product GetProductByName(string name);
    Product GetProductById(int  id);
    List<Product> GetAllProducts();
    List<Product> GetProductsByCategoryName(string categoryName);
    Product UpdateProductQuantity(int productId, int quantity);


    int GetProductQuantity(int productId);


    // Update
    Product UpdateProduct(Product product);

    // Delete
    bool DeleteProduct(string Name);
}
