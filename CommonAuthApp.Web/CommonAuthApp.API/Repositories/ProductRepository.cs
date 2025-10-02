using CommonAuthApp.API.Data;
using CommonAuthApp.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonAuthApp.Repositories
{
    public interface IProductRepository
    {
        Task<List<ProductModel>> GetProducts();
        Task<ProductModel> GetProduct(int id);
        Task UpdateProduct(ProductModel productModel);
        Task<ProductModel> CreateProduct(ProductModel productModel);
        Task<bool> ProductModelExists(int id);
        Task DeleteProduct(int id);
        Task<SchoolMenu> CreateSchoolMenu(SchoolMenu schoolModel);
        Task<List<SchoolModel>> GetSchools();
        Task<SchoolModel> CreateSchool(SchoolModel schoolModel);
        Task<SchoolModel> GetSchool(int id);
        Task<SchoolSystemDetails> CreateSchoolSystem(SchoolSystemDetails schoolSystem);
        Task<List<SchoolMenu>> GetSchoolMenu();
    }
    public class ProductRepository(AppDbContext dbContext) : IProductRepository
    {
        public Task<List<ProductModel>> GetProducts()
        {
            return dbContext.Products.ToListAsync();
        }

        public Task<ProductModel> GetProduct(int id)
        {
            return dbContext.Products.FirstOrDefaultAsync(n => n.ID == id);
        }

        public async Task<ProductModel> CreateProduct(ProductModel productModel)
        {
            dbContext.Products.Add(productModel);
            await dbContext.SaveChangesAsync();
            return productModel;
        }
        public async Task UpdateProduct(ProductModel productModel)
        {
            dbContext.Entry(productModel).State = EntityState.Modified;
            await dbContext.SaveChangesAsync();
        }
        public Task<bool> ProductModelExists(int id)
        {
            return dbContext.Products.AnyAsync(e => e.ID == id);
        }
        public async Task DeleteProduct(int id)
        {
            var product = dbContext.Products.FirstOrDefault(n => n.ID == id);
            dbContext.Products.Remove(product);
            await dbContext.SaveChangesAsync();
        }

        public async Task<SchoolMenu> CreateSchoolMenu(SchoolMenu schoolModel)
        {
            dbContext.SchoolMenus.Add(schoolModel);
            await dbContext.SaveChangesAsync();
            return schoolModel;
        }

        public Task<List<SchoolModel>> GetSchools()
        {
            return dbContext.Schools.ToListAsync();
        }

        public async Task<SchoolModel> CreateSchool(SchoolModel schoolModel)
        {
            dbContext.Schools.Add(schoolModel);
            await dbContext.SaveChangesAsync();
            return schoolModel;
        }

        public Task<SchoolModel> GetSchool(int id)
        {
            return dbContext.Schools.FirstOrDefaultAsync(n => n.SchoolId == id);
        }

        public async Task<SchoolSystemDetails> CreateSchoolSystem(SchoolSystemDetails schoolSystem)
        {
            dbContext.SchoolSystemDetails.Add(schoolSystem);
            await dbContext.SaveChangesAsync();
            return schoolSystem;
        }

        public Task<List<SchoolMenu>> GetSchoolMenu()
        {
            return dbContext.SchoolMenus.ToListAsync();
        }
    }
}
