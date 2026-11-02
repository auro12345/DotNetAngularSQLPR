#region Copyright Baker Hughes Company 2026
//
// All rights are reserved. Reproduction or transmission in whole or in part, in
// any form or by any means, electronic, mechanical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
//
// File              : DependencyInjection.cs
// Description       : Registers infrastructure services and database dependencies.
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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
#endregion

#region Custom Directives
using ProductManagement.Application.Interfaces;
using ProductManagement.Infrastructure.Data;
using ProductManagement.Infrastructure.Repositories;
#endregion

namespace ProductManagement.Infrastructure
{
    /// <summary>
    /// Provides extension methods for registering infrastructure dependencies.
    /// </summary>
    public static class DependencyInjection
    {
        #region Public Method(s)

        /// <summary>
        /// Registers infrastructure services and the SQL Server database context.
        /// </summary>
        /// <param name="services">The service collection to which dependencies are added.</param>
        /// <param name="configuration">The application configuration containing the database connection string.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ProductManagementDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IProductRepository, SqlProductRepository>();
            return services;
        }

        #endregion
    }
}
