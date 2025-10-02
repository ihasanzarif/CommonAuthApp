using CommonAuthApp.API.Models;
using CommonAuthApp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonAuthApp.Services
{
    public interface IProductService
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

    public class ProductService(IProductRepository productRepository) : IProductService
    {
        public Task<ProductModel> CreateProduct(ProductModel productModel)
        {
            return productRepository.CreateProduct(productModel);
        }

        public Task<ProductModel> GetProduct(int id)
        {
            return productRepository.GetProduct(id);
        }

        public Task<List<ProductModel>> GetProducts()
        {
            return productRepository.GetProducts();
        }

        public Task<bool> ProductModelExists(int id)
        {
            return productRepository.ProductModelExists(id);
        }

        public Task UpdateProduct(ProductModel productModel)
        {
            return productRepository.UpdateProduct(productModel);
        }
        public Task DeleteProduct(int id)
        {
            return productRepository.DeleteProduct(id);
        }

        public Task<SchoolMenu> CreateSchoolMenu(SchoolMenu schoolModel)
        {
            return productRepository.CreateSchoolMenu(schoolModel);
        }

        public Task<List<SchoolModel>> GetSchools()
        {
            return productRepository.GetSchools();
        }

        public Task<SchoolModel> CreateSchool(SchoolModel schoolModel)
        {
            return productRepository.CreateSchool(schoolModel);
        }

        public Task<SchoolModel> GetSchool(int id)
        {
            return productRepository.GetSchool(id);
        }

        public Task<SchoolSystemDetails> CreateSchoolSystem(SchoolSystemDetails schoolSystem)
        {
            return productRepository.CreateSchoolSystem(schoolSystem);
        }

        public Task<List<SchoolMenu>> GetSchoolMenu()
        {
            return productRepository.GetSchoolMenu();
        }
    }
}
