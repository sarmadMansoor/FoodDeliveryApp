using System.Collections.Generic;
using System.Linq;
using Web_Project.Data;
using Web_Project.Models;

public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _context;

    public CategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Category AddCategory(Category category)
    {
        _context.Categories.Add(category);
        _context.SaveChanges(); // Blocking call
        return category;
    }

    public Category GetCategoryByName(string name)
    {
        return _context.Categories.FirstOrDefault(c => c.Name == name); // Query by name
    }

    public List<Category> GetAllCategories()
    {
        return _context.Categories.ToList(); // Return all categories
    }

    public Category UpdateCategory(Category category)
    {
        _context.Categories.Update(category);
        _context.SaveChanges(); // Update and save changes
        return category;
    }

    public bool DeleteCategory(string name)
    {
        var category = _context.Categories.FirstOrDefault(c => c.Name == name); // Query by name
        if (category == null) return false;

        _context.Categories.Remove(category);
        _context.SaveChanges(); // Delete and save changes
        return true;
    }

    Category ICategoryRepository.GetCategoryById(int id)
    {
        return _context.Categories.FirstOrDefault(c => c.CategoryId == id); // Query by name
    }

    /* Category ICategoryRepository.AddCategory(Category category)
     {
         throw new NotImplementedException();
     }

     Category ICategoryRepository.GetCategoryByName(string name)
     {
         throw new NotImplementedException();
     }

     List<Category> ICategoryRepository.GetAllCategories()
     {
         throw new NotImplementedException();
     }

     Category ICategoryRepository.UpdateCategory(Category category)
     {
         throw new NotImplementedException();
     }

     bool ICategoryRepository.DeleteCategory(string Name)
     {
         throw new NotImplementedException();
     }*/
}
