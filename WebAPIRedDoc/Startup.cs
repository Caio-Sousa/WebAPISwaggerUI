using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;

namespace WebAPIRedDoc
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private static string GetApiLeadingText() => $"##Right now you are reading the documentation for version,  **this API version is not supported any more**.";

        private static string GetDescription()
        {
            var file = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "redoc", "description.md");

            string SendData = File.ReadAllText(file);

            return SendData;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Title = "Introdução",
                    Version = "v1",
                    Extensions = new Dictionary<string, IOpenApiExtension>
                    {
                        {"x-logo", new OpenApiObject
                            {
                                {"url", new OpenApiString("https://upload.wikimedia.org/wikipedia/pt/thumb/1/14/BHTrans-logo_%281%29.jpg/250px-BHTrans-logo_%281%29.jpg")},
                                { "altText", new OpenApiString("CODTRAN")}
                            }
                        }
                    },
                    //Description = $@"## Introdução "
                    Description = GetDescription()
                });

                //BEARER SECURITY SCHEME
                OpenApiSecurityScheme securityDefinition = new OpenApiSecurityScheme()
                {
                    Name = "Bearer",
                    BearerFormat = "JWT",
                    Scheme = "bearer",
                    Description = "Specify the authorization token.",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                };

                OpenApiSecurityRequirement securityRequirements = new OpenApiSecurityRequirement()
                {
                    {securityDefinition, new string[] { }},
                };

                c.AddSecurityDefinition("jwt_auth", securityDefinition);

                // Make sure swagger UI requires a Bearer token to be specified
                c.AddSecurityRequirement(securityRequirements);

                var xmlFile = $"{Assembly.GetEntryAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseReDoc(c =>
            {
                c.DocumentTitle = "API Documentation";
                c.SpecUrl = "/swagger/v1/swagger.json";
                c.InjectStylesheet("/redoc/custom.css");
                //c.IndexStream = () => GetType().Assembly
                //                               .GetManifestResourceStream("redoc.index.html"); // requires file to be added as an embedded resource
            });

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("v1/swagger.json", "My API V1");
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}