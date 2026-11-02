#region Copyright Baker Hughes Company 2026
//
// All rights are reserved. Reproduction or transmission in whole or in part, in
// any form or by any means, electronic, mechanical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
//
// File              : SqlProductRepository.cs
// Description       : Implements product data access operations using SQL Server and Entity Framework Core.
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
using Microsoft.EntityFrameworkCore;
using ProductManagement.Application.Interfaces;
using ProductManagement.Domain.Entities;
using ProductManagement.Infrastructure.Data;
#endregion

namespace ProductManagement.Infrastructure.Repositories
{
    /// <summary>
    /// Implements the product repository using Entity Framework Core and SQL Server.
    /// </summary>
    public class SqlProductRepository : IProductRepository
    {
        #region Private Field(s)

        /// <summary>
        /// Provides access to product records in the database.
        /// </summary>
        private readonly ProductManagementDbContext _context;

        #endregion

        #region Public Method(s)

        /// <summary>
        /// Initializes the repository with the database context.
        /// </summary>
        /// <param name="context">The Entity Framework Core database context.</param>
        public SqlProductRepository(ProductManagementDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all products from the database.
        /// </summary>
        /// <returns>A read-only list of products.</returns>
        public IReadOnlyList<Product> GetAll() => _context.Products.AsNoTracking().ToList();

        /// <summary>
        /// Retrieves a product by its identifier.
        /// </summary>
        /// <param name="id">The product identifier.</param>
        /// <returns>The product if found; otherwise, null.</returns>
        public Product? GetById(int id) => _context.Products.FirstOrDefault(p => p.Id == id);

        /// <summary>
        /// Adds a product to the database.
        /// </summary>
        /// <param name="name">The product name.</param>
        /// <param name="description">The optional product description.</param>
        /// <param name="price">The product price.</param>
        /// <param name="category">The product category.</param>
        /// <returns>The newly created product.</returns>
        public Product Add(string name, string? description, decimal price, string category)
        {
            var product = new Product(0, name, description, price, category);
            _context.Products.Add(product);
            _context.SaveChanges();
            return product;
        }

        /// <summary>
        /// Updates an existing product in the database.
        /// </summary>
        /// <param name="product">The product containing the updated details.</param>
        /// <returns>True if the update was saved; otherwise, false.</returns>
        public bool Update(Product product)
        {
            _context.Products.Update(product);
            return _context.SaveChanges() > 0;
        }

        #endregion
    }
}
