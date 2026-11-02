#region Copyright Baker Hughes Company 2026
//
// All rights are reserved. Reproduction or transmission in whole or in part, in
// any form or by any means, electronic, mechanical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
//
// File              : CreateProductRequest.cs
// Description       : Request DTO for creating a product with validation rules
//                     applied to incoming product data.
// Author            : Aurobinda Tripathy
// Created           : 27-10-2026
// Traceability      : Product Management
//
// Revision History:
// Version    Date         Modification Tag    Modification Description
// V1.0       27-10-2026   INITIAL             Initial coding activities
//
#endregion

#region MS Directives
using System.ComponentModel.DataAnnotations;
#endregion

namespace ProductManagement.Application.DTOs.Products
{
    /// <summary>
    /// Represents the request data required to create a product.
    /// </summary>
    #region Create Product Request
    public class CreateProductRequest
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the product name.
        /// The name is required and must contain between 2 and 100 characters.
        /// </summary>
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the optional product description, up to 500 characters.
        /// </summary>
        [StringLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the product price, which must be between 0.01 and 999999999.
        /// </summary>
        [Range(typeof(decimal), "0.01", "999999999")]
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets the product category.
        /// The category is required and must contain between 2 and 50 characters.
        /// </summary>
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Category { get; set; } = string.Empty;

        #endregion
    }
    #endregion
}