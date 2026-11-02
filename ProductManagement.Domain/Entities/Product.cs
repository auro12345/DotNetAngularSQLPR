#region Copyright Baker Hughes Company 2026
//
// All rights are reserved. Reproduction or transmission in whole or in part, in
// any form or by any means, electronic, mechanical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
//
// File              : Product.cs
// Description       : Domain entity representing a product and enforcing product
//                     business validation rules.
// Author            : Aurobinda Tripathy
// Created           : 27-10-2026
// Traceability      : Product Management
//
// Revision History:
// Version    Date         Modification Tag    Modification Description
// V1.0       27-10-2026   INITIAL             Initial coding activities
//
#endregion

namespace ProductManagement.Domain.Entities
{
    /// <summary>
    /// Represents a product and encapsulates its core business validation rules.
    /// </summary>
    #region Product Entity
    public class Product
    {
        #region Public Properties

        /// <summary>
        /// Gets the unique identifier of the product.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Gets the name of the product.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the description of the product.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets the price of the product.
        /// </summary>
        public decimal Price { get; private set; }

        /// <summary>
        /// Gets the category to which the product belongs.
        /// </summary>
        public string Category { get; private set; }

        #endregion

        #region Public Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="Product"/> class.
        /// </summary>
        /// <param name="id">The unique identifier of the product.</param>
        /// <param name="name">The name of the product.</param>
        /// <param name="description">The description of the product, if available.</param>
        /// <param name="price">The price of the product.</param>
        /// <param name="category">The category to which the product belongs.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the product name or category is null, empty, or whitespace.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the product price is less than or equal to zero.
        /// </exception>
        public Product(int id, string name, string? description, decimal price, string category)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(
                    "Product name is required.", nameof(name));

            if (string.IsNullOrWhiteSpace(category))
                throw new ArgumentException(
                    "Product category is required.", nameof(category));

            if (price <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(price), "Price must be greater than zero.");

            Id = id;
            Name = name;
            Description = description ?? string.Empty;
            Price = price;
            Category = category;
        }

        #endregion

        #region Public Method(s)

        /// <summary>
        /// Updates the name, description, price, and category of the product.
        /// </summary>
        /// <param name="name">The updated name of the product.</param>
        /// <param name="description">The updated description of the product, if available.</param>
        /// <param name="price">The updated price of the product.</param>
        /// <param name="category">The updated category of the product.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the product name or category is null, empty, or whitespace.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the product price is less than or equal to zero.
        /// </exception>
        public void UpdateDetails(string name, string? description, decimal price, string category)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(
                    "Product name is required.", nameof(name));

            if (string.IsNullOrWhiteSpace(category))
                throw new ArgumentException(
                    "Product category is required.", nameof(category));

            if (price <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(price), "Price must be greater than zero.");

            Name = name;
            Description = description ?? string.Empty;
            Price = price;
            Category = category;
        }

        #endregion
    }
    #endregion
}