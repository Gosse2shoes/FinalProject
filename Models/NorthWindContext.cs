using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProject.Models
{
    public class NorthWindContext : DbContext
    {
        public NorthWindContext() : base("name=NorthWindContext") { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }

        public void AddProduct(Product product)
        {
            this.Products.Add(product);
            this.SaveChanges();
        }
        public void EditProduct(Product UpdatedProduct)
        {
            Product product = this.Products.Find(UpdatedProduct.ProductID);
            product.ProductName = UpdatedProduct.ProductName;
            product.QuantityPerUnit = UpdatedProduct.QuantityPerUnit;
            product.UnitPrice = UpdatedProduct.UnitPrice;
            product.UnitsInStock = UpdatedProduct.UnitsInStock;
            product.UnitsOnOrder = UpdatedProduct.UnitsOnOrder;
            product.ReorderLevel = UpdatedProduct.ReorderLevel;
            product.Discontinued = UpdatedProduct.Discontinued;
            product.CategoryId = UpdatedProduct.CategoryId;
            this.SaveChanges();
        }
        public void AddCategory(Category category)
        {
            this.Categories.Add(category);
            this.SaveChanges();
        }
        public void EditCategory(Category UpdatedCategory)
        {
            Category category = this.Categories.Find(UpdatedCategory.CategoryID);
            category.CategoryName = UpdatedCategory.CategoryName;
            category.Description = UpdatedCategory.Description;
            this.SaveChanges();
        }
    }
}
