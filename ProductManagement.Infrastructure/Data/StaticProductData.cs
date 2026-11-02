#region Copyright Baker Hughes Company 2026
//
// All rights are reserved. Reproduction or transmission in whole or in part, in
// any form or by any means, electronic, mechanical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
//
// File              : StaticProductData.cs
// Description       : Provides static seed data for initializing the product
//                     collection in the in-memory repository.
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
    /// Provides methods for creating the initial collection of product data.
    /// </summary>
    #region Static Product Data
    public static class StaticProductData
    {
        #region Public Method(s)

        /// <summary>
        /// Creates a list containing the initial seeded products.
        /// </summary>
        /// <returns>A list of products populated with the initial seed data.</returns>
        public static List<Product> CreateProducts()
        {
            return new List<Product>
            {
                new Product(1, "Laptop", "Business laptop", 65000m, "Electronics"),
                new Product(2, "Smartphone", "Android smartphone", 25000m, "Mobiles"),
                new Product(3, "Monitor", "24-inch monitor", 12000m, "Accessories")
            };
        }

        #endregion
    }
    #endregion
}