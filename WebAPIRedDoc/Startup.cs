using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace WebAPIRedDoc
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo() { Title = "WebAPIRedDoc", Version = "v1" });

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

            //app.UseSwagger(c =>
            //{
            //    c.PreSerializeFilters.Add((swagger, httpRequest) =>
            //    {
            //        swagger.Servers.Add(new OpenApiServer
            //        {
            //            Url = $"{httpRequest.Scheme}://{httpRequest.Host}"
            //        });
            //    });
            //});


            app.UseReDoc(c =>
            {
                c.DocumentTitle = "API Documentation";
                c.RoutePrefix = "redoc";
                c.SpecUrl("../swagger/v1/swagger.json");
               // c.SpecUrl("../v1/swagger.json");
            });

            //app.UseSwagger();
            //app.UseSwaggerUI(c =>
            //{
            //    c.SwaggerEndpoint("v1/swagger.json", "My API V1");
            //});


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