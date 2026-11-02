#region Copyright Baker Hughes Company 2026
//
// All rights are reserved. Reproduction or transmission in whole or in part, in
// any form or by any means, electronic, mechanical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
//
// File              : IProductService.cs
// Description       : Application service interface defining product retrieval,
//                     creation, and update operations.
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
#endregion

namespace ProductManagement.Application.Interfaces
{
    /// <summary>
    /// Defines the contract for product-related application operations.
    /// </summary>
    #region Product Service Interface
    public interface IProductService
    {
        #region Public Method(s)

        /// <summary>
        /// Retrieves all products.
        /// </summary>
        /// <returns>A read-only list of product DTOs.</returns>
        IReadOnlyList<ProductDto> GetAll();

        /// <summary>
        /// Retrieves a product by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the product.</param>
        /// <returns>The product DTO if found; otherwise, <see langword="null"/>.</returns>
        ProductDto? GetById(int id);

        /// <summary>
        /// Creates a new product using the supplied request data.
        /// </summary>
        /// <param name="request">The request containing the product details.</param>
        /// <returns>The DTO representing the created product.</returns>
        ProductDto Create(CreateProductRequest request);

        /// <summary>
        /// Updates an existing product using the supplied request data.
        /// </summary>
        /// <param name="id">The unique identifier of the product to update.</param>
        /// <param name="request">The request containing the updated product details.</param>
        /// <returns>The updated product DTO if found; otherwise, <see langword="null"/>.</returns>
        ProductDto? Update(int id, UpdateProductRequest request);

        #endregion
    }
    #endregion
}