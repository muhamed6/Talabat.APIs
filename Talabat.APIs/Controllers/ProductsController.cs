﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Dtos;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.Core.Entities.Product_Aggregate;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Services.Contract;
using Talabat.Core.Specifications;
using Talabat.Core.Specifications.ProductSpecs;

namespace Talabat.APIs.Controllers
{

    public class ProductsController : BaseApiController
    {
        private readonly IProductService _productService;


        private readonly IMapper _mapper;

        public ProductsController( 
        IProductService productService,
            IMapper mapper)
        {
            _productService = productService;

            _mapper = mapper;
        }

        [CachedAttribute(600)]
        [HttpGet]   
        public async Task <ActionResult<Pagination<ProductToReturnDto>>> GetProducts([FromQuery]ProductSpecParams specParams)
            {

            var products = await _productService.GetProductsAsync(specParams);

            var data = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products);

            var countSpec = new ProductWithFilterationForCountSpecifications(specParams);

            var count=await _productService.GetCountAsync(specParams);

            return Ok(new Pagination<ProductToReturnDto>(specParams.PageIndex, specParams.PageSize, count , data));
        }

        [ProducesResponseType(typeof(ProductToReturnDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task <ActionResult<ProductToReturnDto>> GetProduct(int id)
        {

            var product = await _productService.GetProductAsync(id);

            if (product is null)
                return NotFound(new ApiResponse(404));
                return Ok(_mapper.Map<Product,ProductToReturnDto>(product));
        }

        [CachedAttribute(600)]
        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetBrands()
        {
            var brands=await _productService.GetBrandsAsync();
            return Ok(brands);
        }
        [CachedAttribute(600)]
        [HttpGet("categories")]
        public async Task<ActionResult<IReadOnlyList<ProductCategory>>> GetCategories()
        {
            var categories=await _productService.GetCategoriesAsync();
            return Ok(categories);
        }
    }
}
