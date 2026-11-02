#region Copyright Baker Hughes Company 2026
//
// All rights are reserved. Reproduction or transmission in whole or in part, in
// any form or by any means, electronic, mechanical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
//
// File              : ProductsController.cs
// Description       : API controller exposing endpoints to retrieve, create,
//                     and update products.
// Author            : Aurobinda Tripathy
// Created           : 27-10-2026
// Traceability      : Product Management
//
// Revision History:
// Version    Date         Modification Tag    Modification Description
// V1.0       26-10-2026   INITIAL             Initial coding activities
//
#endregion

#region MS Directives
using Microsoft.AspNetCore.Mvc;
#endregion

#region Custom Directives
using ProductManagement.Application.DTOs.Products;
using ProductManagement.Application.Interfaces;
#endregion

namespace ProductManagement.API.Controllers
{
    /// <summary>
    /// Provides HTTP endpoints for product management operations.
    /// </summary>
    #region Products Controller

    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        #region Private Field(s)

        /// <summary>
        /// Provides access to product application services.
        /// </summary>
        private readonly IProductService _productService;

        #endregion

        #region Public Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductsController"/> class.
        /// </summary>
        /// <param name="productService">The service used to perform product operations.</param>
        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        #endregion

        #region Public Endpoints

        /// <summary>
        /// Retrieves all products.
        /// </summary>
        /// <returns>An HTTP 200 response containing the product list.</returns>
        [HttpGet]
        public ActionResult<IReadOnlyList<ProductDto>> GetAll()
        {
            return Ok(_productService.GetAll());
        }

        /// <summary>
        /// Retrieves a product by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the product.</param>
        /// <returns>An HTTP 200 response containing the product, or HTTP 404 if it is not found.</returns>
        [HttpGet("{id:int}")]
        public ActionResult<ProductDto> GetById(int id)
        {
            var product = _productService.GetById(id);
            return product is null ? NotFound() : Ok(product);
        }

        /// <summary>
        /// Creates a new product.
        /// </summary>
        /// <param name="request">The request containing the product details.</param>
        /// <returns>An HTTP 201 response containing the created product and its location.</returns>
        [HttpPost]
        public ActionResult<ProductDto> Create(CreateProductRequest request)
        {
            var product = _productService.Create(request);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        /// <summary>
        /// Updates an existing product.
        /// </summary>
        /// <param name="id">The unique identifier of the product to update.</param>
        /// <param name="request">The request containing the updated product details.</param>
        /// <returns>An HTTP 200 response containing the updated product, or HTTP 404 if it is not found.</returns>
        [HttpPut("{id:int}")]
        public ActionResult<ProductDto> Update(int id, UpdateProductRequest request)
        {
            var product = _productService.Update(id, request);
            return product is null ? NotFound() : Ok(product);
        }

        #endregion
    }
    #endregion
}