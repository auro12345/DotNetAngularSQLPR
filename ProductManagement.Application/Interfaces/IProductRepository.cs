#region Copyright Baker Hughes Company 2026
//
// All rights are reserved. Reproduction or transmission in whole or in part, in
// any form or by any means, electronic, mechanical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
//
// File              : IProductRepository.cs
// Description       : Repository interface defining operations for retrieving,
//                     adding, and updating products.
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
using ProductManagement.Domain.Entities;
#endregion

namespace ProductManagement.Application.Interfaces
{
    /// <summary>
    /// Defines the contract for product data access operations.
    /// </summary>
    #region Product Repository Interface
    public interface IProductRepository
    {
        #region Public Method(s)

        /// <summary>
        /// Retrieves all products.
        /// </summary>
        /// <returns>A read-only list of products.</returns>
        IReadOnlyList<Product> GetAll();

        /// <summary>
        /// Retrieves a product by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the product.</param>
        /// <returns>The product if found; otherwise, <see langword="null"/>.</returns>
        Product? GetById(int id);

        /// <summary>
        /// Adds a new product to the repository.
        /// </summary>
        /// <param name="name">The name of the product.</param>
        /// <param name="description">The optional description of the product.</param>
        /// <param name="price">The price of the product.</param>
        /// <param name="category">The category of the product.</param>
        /// <returns>The newly created product.</returns>
        Product Add(string name, string? description, decimal price, string category);

        /// <summary>
        /// Updates an existing product in the repository.
        /// </summary>
        /// <param name="product">The product containing the updated details.</param>
        /// <returns><see langword="true"/> if the product was updated; otherwise, <see langword="false"/>.</returns>
        bool Update(Product product);

        #endregion
    }
    #endregion
}