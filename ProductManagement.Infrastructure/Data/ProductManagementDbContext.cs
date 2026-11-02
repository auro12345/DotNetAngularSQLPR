#region Copyright Baker Hughes Company 2026
//
// All rights are reserved. Reproduction or transmission in whole or in part, in
// any form or by any means, electronic, mechanical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
//
// File              : ProductManagementDbContext.cs
// Description       : Defines the Entity Framework Core database context for product data.
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
using ProductManagement.Domain.Entities;
#endregion

namespace ProductManagement.Infrastructure.Data
{
    /// <summary>
    /// Represents the database session for product management operations.
    /// </summary>
    public class ProductManagementDbContext : DbContext
    {
        #region Public Method(s)

        /// <summary>
        /// Initializes the database context with the configured options.
        /// </summary>
        /// <param name="options">The database context configuration options.</param>
        public ProductManagementDbContext(DbContextOptions<ProductManagementDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Gets the products stored in the database.
        /// </summary>
        public DbSet<Product> Products => Set<Product>();

        /// <summary>
        /// Configures the database schema for product entities.
        /// </summary>
        /// <param name="modelBuilder">The model builder used to configure entity mappings.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>()
                .Property(product => product.Price)
                .HasPrecision(18, 2);
        }

        #endregion
    }
}
