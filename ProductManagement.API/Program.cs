#region Copyright Baker Hughes Company 2026
//
// All rights are reserved. Reproduction or transmission in whole or in part, in
// any form or by any means, electronic, mechanical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
//
// File              : Program.cs
// Description       : Configures application services, middleware, CORS, Swagger,
//                     and the HTTP request pipeline for the Product Management API.
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
using ProductManagement.Application;
using ProductManagement.Infrastructure;
using ProductManagement.Infrastructure.Data;
#endregion

namespace ProductManagement.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Register application and infrastructure dependencies
            builder.Services.AddApplication();
            builder.Services.AddInfrastructure(builder.Configuration);

            //Configure CORS to allow requests from the Angular development server
            builder.Services.AddCors(options => options.AddPolicy("AngularClient",
                policy => policy.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod()));

            var app = builder.Build();

            // Initialize the database with default product data when empty.
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ProductManagementDbContext>();
                DatabaseInitializer.Seed(context);
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            //Apply the CORS policy to allow Angular to call the API
            app.UseCors("AngularClient");

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
