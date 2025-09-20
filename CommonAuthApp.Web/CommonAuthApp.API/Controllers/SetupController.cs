using CommonAuthApp.API.Models;
using CommonAuthApp.Resources;
using CommonAuthApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace CommonAuthApp.API.Controllers
{
    [Authorize(Roles="Admin", Policy = "GatewayOnly")]
    [Route("admin/[controller]")]
    [ApiController]
    public class SetupController(IProductService productService, IStringLocalizer<ProductTranslation> localizer) : ControllerBase
    {
        [HttpGet]
        [Route("products")]
        public async Task<ActionResult<BaseResponseModel>> GetProducts()
        {
            var products = await productService.GetProducts();
            foreach (var product in products)
            {
                product.Description = product.Description != null ? localizer[product.Description] : null;
            }
            return Ok(new BaseResponseModel { Success = true, Data = products });
        }

        [HttpPost]
        public async Task<ActionResult<ProductModel>> CreateProduct(ProductModel productModel)
        {
            await productService.CreateProduct(productModel);
            return Ok(new BaseResponseModel { Success = true });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BaseResponseModel>> GetProduct(int id)
        {
            var productModel = await productService.GetProduct(id);

            if (productModel == null)
            {
                return Ok(new BaseResponseModel { Success = false, ErrorMessage = "Not Found" });
            }
            return Ok(new BaseResponseModel { Success = true, Data = productModel });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, ProductModel productModel)
        {
            if (id != productModel.ID || !await productService.ProductModelExists(id))
            {
                return Ok(new BaseResponseModel { Success = false, ErrorMessage = "Bad request" });
            }

            await productService.UpdateProduct(productModel);
            return Ok(new BaseResponseModel { Success = true });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (!await productService.ProductModelExists(id))
            {
                return Ok(new BaseResponseModel { Success = false, ErrorMessage = "Not Found" });
            }
            await productService.DeleteProduct(id);
            return Ok(new BaseResponseModel { Success = true });
        }


        [HttpPost]
        [Route("school-menu")]
        public async Task<ActionResult<ProductModel>> CreateSchoolMenu(SchoolMenu schoolMenu)
        {
            await productService.CreateSchoolMenu(schoolMenu);
            return Ok(new BaseResponseModel { Success = true });
        }

        [HttpGet]
        [Route("school-list")]
        public async Task<ActionResult<BaseResponseModel>> GetSchools()
        {
            var schools = await productService.GetSchools();
         
            return Ok(new BaseResponseModel { Success = true, Data = schools });
        }

        [HttpPost]
        [Route("school-create")]
        public async Task<ActionResult<SchoolModel>> CreateSchool(SchoolModel schoolModel)
        {
            await productService.CreateSchool(schoolModel);
            return Ok(new BaseResponseModel { Success = true });
        }

        [HttpPost]
        [Route("school")]
        public async Task<ActionResult<BaseResponseModel>> GetSchool(int id)
        {
            var schoolModel = await productService.GetSchool(id);

            if (schoolModel == null)
            {
                return Ok(new BaseResponseModel { Success = false, ErrorMessage = "Not Found" });
            }
            return Ok(new BaseResponseModel { Success = true, Data = schoolModel });
        }

        [HttpPost]
        [Route("school-system")]
        public async Task<ActionResult<SchoolModel>> CreateSchoolSystem(SchoolSystemDetails schoolSystem)
        {
            await productService.CreateSchoolSystem(schoolSystem);
            return Ok(new BaseResponseModel { Success = true });
        }
    }
}
