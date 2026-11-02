#region Copyright Baker Hughes Company 2026
//
// All rights are reserved. Reproduction or transmission in whole or in part, in
// any form or by any means, electronic, mechanical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
//
// File              : ProductDto.cs
// Description       : Data transfer object representing product information
//                     exchanged between application layers and API consumers.
// Author            : Aurobinda Tripathy
// Created           : 27-10-2026
// Traceability      : Product Management
//
// Revision History:
// Version    Date         Modification Tag    Modification Description
// V1.0       27-10-2026   INITIAL             Initial coding activities
//
#endregion

namespace ProductManagement.Application.DTOs.Products
{
    /// <summary>
    /// Represents product information transferred between application layers
    /// and API consumers.
    /// </summary>
    #region Product Data Transfer Object
    public class ProductDto
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the unique identifier of the product.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the product.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the product.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the price of the product.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets the category to which the product belongs.
        /// </summary>
        public string Category { get; set; } = string.Empty;

        #endregion
    }
    #endregion
}