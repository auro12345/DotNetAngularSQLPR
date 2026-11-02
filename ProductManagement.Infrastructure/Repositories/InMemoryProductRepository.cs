#region Copyright Baker Hughes Company 2026
//
// All rights are reserved. Reproduction or transmission in whole or in part, in
// any form or by any means, electronic, mechanical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
//
// File              : InMemoryProductRepository.cs
// Description       : Implements product data access operations using an
//                     in-memory collection of products.
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
using ProductManagement.Application.Interfaces;
using ProductManagement.Domain.Entities;
using ProductManagement.Infrastructure.Data;
#endregion

namespace ProductManagement.Infrastructure.Repositories
{
    /// <summary>
    /// Implements the product repository using an in-memory collection.
    /// </summary>
    #region In-Memory Product Repository
    public class InMemoryProductRepository : IProductRepository
    {
        #region Private Field(s)

        /// <summary>
        /// Stores the products maintained by the in-memory repository.
        /// </summary>
        private readonly List<Product> _products = StaticProductData.CreateProducts();

        /// <summary>
        /// Stores the identifier to assign to the next product created.
        /// </summary>
        private int _nextId = 4;

        #endregion

        #region Public Method(s)

        /// <summary>
        /// Retrieves all products as a read-only collection.
        /// </summary>
        /// <returns>A read-only list of products.</returns>
        public IReadOnlyList<Product> GetAll() => _products.AsReadOnly();

        /// <summary>
        /// Retrieves a product by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the product.</param>
        /// <returns>The matching product if found; otherwise, <see langword="null"/>.</returns>
        public Product? GetById(int id) => _products.FirstOrDefault(p => p.Id == id);

        /// <summary>
        /// Creates and stores a new product in the in-memory collection.
        /// </summary>
        /// <param name="name">The name of the product.</param>
        /// <param name="description">The optional description of the product.</param>
        /// <param name="price">The price of the product.</param>
        /// <param name="category">The category of the product.</param>
        /// <returns>The newly created product.</returns>
        public Product Add(string name, string? description, decimal price, string category)
        {
            var product = new Product(_nextId++, name, description, price, category);
            _products.Add(product);
            return product;
        }

        /// <summary>
        /// Updates an existing product in the in-memory collection.
        /// </summary>
        /// <param name="product">The product containing the updated details.</param>
        /// <returns><see langword="true"/> if the product was updated; otherwise, <see langword="false"/>.</returns>
        public bool Update(Product product)
        {
            var index = _products.FindIndex(p => p.Id == product.Id);
            if (index < 0) return false;

            _products[index] = product;
            return true;
        }

        #endregion
    }
    #endregion
}