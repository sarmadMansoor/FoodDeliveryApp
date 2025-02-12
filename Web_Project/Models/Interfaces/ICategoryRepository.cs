using System.Collections.Generic;
using Web_Project.Models;

public interface ICategoryRepository
{
    // Create
    Category AddCategory(Category category);

    // Read
    Category GetCategoryByName(string  name);
    List<Category> GetAllCategories();
    Category GetCategoryById(int id);

    // Update
    Category UpdateCategory(Category category);

    // Delete
    bool DeleteCategory(string Name);
}
