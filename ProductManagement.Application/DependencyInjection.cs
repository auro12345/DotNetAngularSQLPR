#region Copyright Baker Hughes Company 2026
//
// All rights are reserved. Reproduction or transmission in whole or in part, in
// any form or by any means, electronic, mechanical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
//
// File              : DependencyInjection.cs
// Description       : Registers application-layer services and their
//                     dependency injection configurations.
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
using Microsoft.Extensions.DependencyInjection;
#endregion

#region Custom Directives
using ProductManagement.Application.Interfaces;
using ProductManagement.Application.Services;
#endregion

namespace ProductManagement.Application
{
    /// <summary>
    /// Provides extension methods for registering application-layer dependencies.
    /// </summary>
    #region Dependency Injection
    public static class DependencyInjection
    {
        #region Public Method(s)

        /// <summary>
        /// Registers application services with the dependency injection container.
        /// </summary>
        /// <param name="services">The service collection to which dependencies are added.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IProductService, ProductService>();
            return services;
        }

        #endregion
    }
    #endregion
}