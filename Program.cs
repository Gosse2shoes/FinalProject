using FinalProject.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProject
{
    class Program
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {
            logger.Info("Program started");
            try
            {
                string choice;
                do
                {
                    Console.WriteLine("Enter your selection: ");
                    Console.WriteLine("1) Add a Product");
                    Console.WriteLine("2) Edit a Product");
                    Console.WriteLine("3) Display Products");
                    Console.WriteLine("4) Add a Category");
                    Console.WriteLine("5) Edit a Category");
                    Console.WriteLine("6) Display all Categories");
                    Console.WriteLine("7) Display all Categories and their related active Product");
                    Console.WriteLine("8) Display specific Categories and their related active Product");
                    Console.WriteLine("Enter q to quit");
                    choice = Console.ReadLine();
                    Console.Clear();
                    logger.Info("Option {choice} selected", choice);

                    if (choice == "1")
                    {
                        Product product = new Product();
                        var db = new NorthWindContext();
                        Console.WriteLine("Enter a name for a new product: ");
                        product.ProductName = Console.ReadLine();
                        Console.WriteLine("Please enter the quantity per unit for the product: ");
                        product.QuantityPerUnit = Console.ReadLine();
                        Console.WriteLine("Please enter the unit price for the product: ");
                        product.UnitPrice = Convert.ToDecimal(Console.ReadLine());
                        Console.WriteLine("Please enter how many units are in stock: ");
                        product.UnitsInStock = Convert.ToInt16(Console.ReadLine());
                        Console.WriteLine("Please enter how many units are on order: ");
                        product.UnitsOnOrder = Convert.ToInt16(Console.ReadLine());
                        Console.WriteLine("Please enter the reorder level: ");
                        product.ReorderLevel = Convert.ToInt16(Console.ReadLine());
                        Console.WriteLine("Please enter if this product is discontinued: ");
                        product.Discontinued = Convert.ToBoolean(Console.ReadLine());
                        Console.WriteLine("Please enter the category the product is in: \n");
                        var categories = db.Categories.OrderBy(p => p.CategoryID);
                        foreach (Category c in categories)
                        {
                            Console.WriteLine($"{c.CategoryID}) {c.CategoryName}");
                        }
                        product.CategoryId = Convert.ToInt32(Console.ReadLine());

                        ValidationContext context = new ValidationContext(product, null, null);
                        List<ValidationResult> results = new List<ValidationResult>();

                        var isVaild = Validator.TryValidateObject(product, context, results, true);
                        if (isVaild)
                        {
                            db = new NorthWindContext();
                            if (db.Products.Any(p => p.ProductName == product.ProductName))
                            {
                                isVaild = false;
                                results.Add(new ValidationResult("Product name exists", new string[] { "ProductName" }));
                            }
                            else
                            {
                                logger.Info("Validation passed");
                                db.AddProduct(product);
                                logger.Info("Product added - {name}", product.ProductName);
                            }
                        }
                        if (!isVaild)
                        {
                            foreach (var result in results)
                            {
                                logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                            }
                        }
                    }
                    else if (choice == "2")
                    {
                        Console.WriteLine("Choose the product to edit: ");
                        var db = new NorthWindContext();
                        var product = GetProduct(db);
                        if (product != null)
                        {
                            Product UpdateProduct = InputProduct(db);
                            if (UpdateProduct != null)
                            {
                                UpdateProduct.ProductID = product.ProductID;
                                db.EditProduct(UpdateProduct);
                                logger.Info("Product (id: {ProductID}) updated", UpdateProduct.ProductID);
                            }
                        }
                    }
                    else if (choice == "3")
                    {
                        var db = new NorthWindContext();
                        var query = db.Products.OrderBy(p => p.ProductID);
                        Console.WriteLine("Select the product to display:");
                        Console.WriteLine("1) Display all products");
                        Console.WriteLine("2) Display discontinued products");
                        Console.WriteLine("3) Display active products");

                        if (int.TryParse(Console.ReadLine(), out int userinput))
                        {
                            
                            if (userinput != 1 && userinput != 2 && userinput != 3)
                            {
                                logger.Error("Invalid Input");
                            }
                            else
                            {
                                if (userinput == 1)
                                {
                                    DisplayProducts(); 
                                }
                                else if (userinput == 2)
                                {
                                    DisplayDiscontinued(); 
                                }
                                else if (userinput == 3)
                                {
                                    DisplayActive();
                                }
                            }
                        }

                    }
                    else if (choice == "4")
                    {
                        Category category = new Category();
                        Console.WriteLine("Enter a name for a new category: ");
                        category.CategoryName = Console.ReadLine();
                        Console.WriteLine("Please enter the quantity per unit for the product: ");
                        category.Description = Console.ReadLine();

                        ValidationContext context = new ValidationContext(category, null, null);
                        List<ValidationResult> results = new List<ValidationResult>();

                        var isVaild = Validator.TryValidateObject(category, context, results, true);
                        if (isVaild)
                        {
                            var db = new NorthWindContext();
                            if (db.Categories.Any(p => p.CategoryName == category.CategoryName))
                            {
                                isVaild = false;
                                results.Add(new ValidationResult("Product name exists", new string[] { "ProductName" }));
                            }
                            else
                            {
                                logger.Info("Validation passed");
                                db.AddCategory(category);
                                logger.Info("Product added - {name}", category.CategoryName);
                            }
                        }
                        if (!isVaild)
                        {
                            foreach (var result in results)
                            {
                                logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                            }
                        }
                    }
                    else if (choice == "5")
                    {
                        Console.WriteLine("Choose the category to edit: ");
                        var db = new NorthWindContext();
                        var category = GetCategory(db);
                        if (category != null)
                        {
                            Category UpdateCategory = InputCategory(db);
                            if (UpdateCategory != null)
                            {
                                UpdateCategory.CategoryID = category.CategoryID;
                                db.EditCategory(UpdateCategory);
                                logger.Info("Product (id: {ProductID}) updated", UpdateCategory.CategoryID);
                            }
                        }
                    }
                    else if (choice == "6")
                    {
                        DisplayCategory();
                    }
                    else if(choice == "7")
                    {
                        var db = new NorthWindContext();
                        var categories = db.Categories.Include("Products").OrderBy(p => p.CategoryID);
                        foreach (var item in categories)
                        {
                            Console.WriteLine($"{item.CategoryName}");
                            foreach (Product p in item.Products)
                            {
                                Console.WriteLine($"\t{p.ProductName}");
                            }
                        }
                    }
                    else if(choice == "8")
                    {
                        var db = new NorthWindContext();
                        string user;
                        DisplayCategory();
                        Console.WriteLine("Which category would you like to view: ");
                        user = Console.ReadLine();
                        int categoryID = int.Parse(user);

                        var category = db.Categories.Include("Products").OrderBy(p => p.CategoryID).Where(c => c.CategoryID == categoryID);
                        foreach (var item in category)
                        {
                            Console.WriteLine($"{item.CategoryName}");
                            foreach (Product p in item.Products)
                            {
                                Console.WriteLine($"\t{p.ProductName}");
                            }
                        }
                    }
                    Console.WriteLine();
                } while (choice.ToLower() != "q");
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            logger.Info("Program ended");
        }
        public static Product GetProduct(NorthWindContext db)
        {
            var products = db.Products.OrderBy(p => p.ProductID);
            foreach (Product p in products)
            {
                Console.WriteLine($"{p.ProductID}) {p.ProductName}");
            }
            if (int.TryParse(Console.ReadLine(), out int ProductID))
            {
                Product product = db.Products.FirstOrDefault(p => p.ProductID == ProductID);
                if (product != null)
                {
                    return product;
                }
            }
            logger.Error("Invalid Product Id");
            return null;
        }
        public static Product InputProduct(NorthWindContext db)
        {
            Product products = new Product();
            Console.WriteLine("Enter a name for a product: ");
            products.ProductName = Console.ReadLine();
            Console.WriteLine("Please enter the quantity per unit for the product: ");
            products.QuantityPerUnit = Console.ReadLine();
            Console.WriteLine("Please enter the unit price for the product: ");
            products.UnitPrice = Convert.ToDecimal(Console.ReadLine());
            Console.WriteLine("Please enter how many units are in stock: ");
            products.UnitsInStock = Convert.ToInt16(Console.ReadLine());
            Console.WriteLine("Please enter how many units are on order: ");
            products.UnitsOnOrder = Convert.ToInt16(Console.ReadLine());
            Console.WriteLine("Please enter the reorder level: ");
            products.ReorderLevel = Convert.ToInt16(Console.ReadLine());
            Console.WriteLine("Please enter if this product is discontinued: ");
            products.Discontinued = Convert.ToBoolean(Console.ReadLine());
            Console.WriteLine("Please enter the category the product is in: \n");
            var categories = db.Categories.OrderBy(p => p.CategoryID);
            foreach (Category c in categories)
            {
                Console.WriteLine($"{c.CategoryID}) {c.CategoryName}");
            }
            products.CategoryId = Convert.ToInt32(Console.ReadLine());

            ValidationContext context = new ValidationContext(products, null, null);
            List<ValidationResult> results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(products, context, results, true);
            if (isValid)
            {
                return products;
            }
            else
            {
                foreach (var result in results)
                {
                    logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                }
            }
            return null;
        }
        public static void DisplayProducts()
        {
            try
            {
                var db = new NorthWindContext();
                List<Product> products = db.Products.OrderBy(p => p.ProductID).ToList();
                Console.Clear();
                Console.WriteLine($"{products.Count()} product(s) returned");
                Console.WriteLine("All products in the database: ");
                foreach(var product in products)
                {
                    Console.WriteLine($"{product.ProductID}) {product.ProductName}");
                }
                DisplaySpecifc();
            }
            catch (InvalidOperationException ex)
            {
                logger.Error(ex.Message);
            }
        }
        public static void DisplayDiscontinued()
        {
            try
            {
                var db = new NorthWindContext();
                List<Product> products = db.Products.Where(p => p.Discontinued == true).OrderBy(p => p.ProductID).ToList();
                Console.WriteLine($"{products.Count()} product(s) returned");
                Console.WriteLine("All discontinued products in the database: ");
                foreach (var product in products)
                {
                    Console.WriteLine($"{product.ProductID}) {product.ProductName}");
                }
                DisplaySpecifc();
            }
            catch (InvalidOperationException ex)
            {
                logger.Error(ex.Message);
            }
        }
        public static void DisplayActive()
        {
            try
            {
                var db = new NorthWindContext();
                List<Product> products = db.Products.Where(p => p.Discontinued == false).OrderBy(p => p.ProductID).ToList();
                Console.WriteLine($"{products.Count()} product(s) returned");
                Console.WriteLine("All active products in the database: ");
                foreach (var product in products)
                {
                    Console.WriteLine($"{product.ProductID}) {product.ProductName}");
                }
                DisplaySpecifc();
            }
            catch (InvalidOperationException ex)
            {
                logger.Error(ex.Message);
            }
        }
        public static void DisplaySpecifc()
        {
            var db = new NorthWindContext();
            string user;
            Console.WriteLine("Which product would you like to view: ");
            user = Console.ReadLine();
            int productID = int.Parse(user);

            var product = db.Products.Where(p => p.ProductID == productID);
            
            foreach(var p in product)
            {
                Console.Clear();
                Console.WriteLine($"Product ID: {p.ProductID}\nName: {p.ProductName}\nQuantity Per Unit: {p.QuantityPerUnit}\n" +
                                  $"Unit Price: {p.UnitPrice}\nUnits In Stock: {p.UnitsInStock}\nUnits On Order: {p.UnitsOnOrder}\n" +
                                  $"Reorder Level: {p.ReorderLevel}\nDiscontinued: {p.Discontinued}\nCategory ID: {p.CategoryId}");
            }
        }
        public static Category GetCategory(NorthWindContext db)
        {
            var category = db.Categories.OrderBy(p => p.CategoryID);
            foreach (Category p in category)
            {
                Console.WriteLine($"{p.CategoryID}) {p.CategoryName}");
            }
            if (int.TryParse(Console.ReadLine(), out int CategoryID))
            {
                Category categories = db.Categories.FirstOrDefault(p => p.CategoryID == CategoryID);
                if (categories != null)
                {
                    return categories;
                }
            }
            logger.Error("Invalid CategoryID Id");
            return null;
        }
        public static Category InputCategory(NorthWindContext db)
        {
            Category category = new Category();
            Console.WriteLine("Enter a new name for a product: ");
            category.CategoryName = Console.ReadLine();
            Console.WriteLine("What is the new description: ");
            category.Description = Console.ReadLine();

            ValidationContext context = new ValidationContext(category, null, null);
            List<ValidationResult> results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(category, context, results, true);
            if (isValid)
            {
                return category;
            }
            else
            {
                foreach (var result in results)
                {
                    logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                }
            }
            return null;
        }
        public static void DisplayCategory()
        {
            try
            {
                var db = new NorthWindContext();
                List<Category> categories = db.Categories.OrderBy(c => c.CategoryID).ToList();
                Console.Clear();
                Console.WriteLine($"{categories.Count()} categories returned");
                Console.WriteLine("All categories in the database: ");
                foreach (var category in categories)
                {
                    Console.WriteLine($"{category.CategoryID}) {category.CategoryName}\nDescription: {category.Description}");
                }
            }
            catch (InvalidOperationException ex)
            {
                logger.Error(ex.Message);
            }
        }
    } 
}
