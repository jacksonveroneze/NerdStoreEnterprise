﻿using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSE.Identidade.API.Services;

namespace NSE.Identidade.API.Configuration
{
    public static class ApiConfig
    {
        public static IServiceCollection AddApiConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(typeof(MappingProfile));
            
            services.AddTransient<IJWTService, JWTService>();

            services.AddTransient<IEmailSender, EmailSender>(i => 
                new EmailSender(
                    configuration["EmailSender:Host"],
                    configuration.GetValue<int>("EmailSender:Port"),
                    configuration.GetValue<bool>("EmailSender:EnableSSL"),
                    configuration["EmailSender:UserName"],
                    configuration["EmailSender:Password"]
                )
            );

            services.AddControllers();

            return services;
        }

        public static IApplicationBuilder UseApiConfiguration(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseIdentityConfiguration();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            return app;
        }
    }
}