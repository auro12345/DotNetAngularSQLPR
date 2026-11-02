#region Copyright Baker Hughes Company 2026
//
// All rights are reserved. Reproduction or transmission in whole or in part, in
// any form or by any means, electronic, mechanical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
//
// File              : DatabaseInitializer.cs
// Description       : Initializes the product database with default product data when empty.
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

namespace ProductManagement.Infrastructure.Data
{
    /// <summary>
    /// Initializes the database with the default product records.
    /// </summary>
    public static class DatabaseInitializer
    {
        #region Public Method(s)

        /// <summary>
        /// Adds the default products when the database contains no products.
        /// </summary>
        /// <param name="context">The product database context.</param>
        public static void Seed(ProductManagementDbContext context)
        {
            if (context.Products.Any()) return;

            var products = StaticProductData.CreateProducts()
                .Select(product => new Product(0, product.Name, product.Description, product.Price, product.Category));

            context.Products.AddRange(products);
            context.SaveChanges();
        }

        #endregion
    }
}
