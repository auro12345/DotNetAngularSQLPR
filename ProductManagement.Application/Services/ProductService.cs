#region Copyright Baker Hughes Company 2026
//
// All rights are reserved. Reproduction or transmission in whole or in part, in
// any form or by any means, electronic, mechanical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
//
// File              : ProductService.cs
// Description       : Application service implementing product retrieval,
//                     creation, update, and DTO mapping operations.
// Author            : Aurobinda Tripathy
// Created           : 27-10-2026
// Traceability      : Product Management
//
// Revision History:
// Version    Date         Modification Tag    Modification Description
// V1.0       27-10-2026   INITIAL             Initial coding activities
//
#endregion

#region Custom Directives
using ProductManagement.Application.DTOs.Products;
using ProductManagement.Application.Interfaces;
using ProductManagement.Domain.Entities;
#endregion

namespace ProductManagement.Application.Services
{
    /// <summary>
    /// Implements the application service operations for product management.
    /// </summary>
    #region Product Service
    public class ProductService : IProductService
    {
        #region Private Field(s)

        /// <summary>
        /// Provides access to product data operations.
        /// </summary>
        private readonly IProductRepository _productRepository;

        #endregion

        #region Public Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductService"/> class.
        /// </summary>
        /// <param name="productRepository">The repository used to access product data.</param>
        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        #endregion

        #region Public Method(s)

        /// <summary>
        /// Retrieves all products and maps them to product DTOs.
        /// </summary>
        /// <returns>A read-only list of product DTOs.</returns>
        public IReadOnlyList<ProductDto> GetAll()
        {
            return _productRepository.GetAll().Select(MapToDto).ToList();
        }

        /// <summary>
        /// Retrieves a product by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the product.</param>
        /// <returns>The product DTO if found; otherwise, <see langword="null"/>.</returns>
        public ProductDto? GetById(int id)
        {
            var product = _productRepository.GetById(id);

            return product is null ? null : MapToDto(product);
        }

        /// <summary>
        /// Creates a product using the supplied request data.
        /// </summary>
        /// <param name="request">The request containing the product details.</param>
        /// <returns>The DTO representing the created product.</returns>
        public ProductDto Create(CreateProductRequest request)
        {
            var product = _productRepository.Add(request.Name, request.Description, request.Price, request.Category);

            return MapToDto(product);
        }

        /// <summary>
        /// Updates an existing product using the supplied request data.
        /// </summary>
        /// <param name="id">The unique identifier of the product to update.</param>
        /// <param name="request">The request containing the updated product details.</param>
        /// <returns>The updated product DTO if the product exists and the update succeeds; otherwise, <see langword="null"/>.</returns>
        public ProductDto? Update(int id, UpdateProductRequest request)
        {
            var product = _productRepository.GetById(id);

            if (product is null)
                return null;

            product.UpdateDetails(request.Name, request.Description, request.Price, request.Category);

            var updated = _productRepository.Update(product);

            return updated ? MapToDto(product) : null;
        }

        #endregion

        #region Private Method(s)

        /// <summary>
        /// Maps a domain product entity to a product DTO.
        /// </summary>
        /// <param name="product">The domain product entity to map.</param>
        /// <returns>A DTO containing the product information.</returns>
        private static ProductDto MapToDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Category = product.Category
            };
        }

        #endregion
    }
    #endregion
}