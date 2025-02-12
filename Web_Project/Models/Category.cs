using System;
using System.Collections.Generic;
namespace Web_Project.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Product> Products { get; set; } 
     
    }

}
