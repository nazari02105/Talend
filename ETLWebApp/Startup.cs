using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ETLLibrary;
using ETLLibrary.Authentication;
using ETLLibrary.Database;
using ETLLibrary.Database.Managers;
using ETLLibrary.Database.Utils;
using ETLLibrary.Interfaces;
using ETLLibrary.Serializers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;

namespace ETLWebApp
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
            services.AddCors(options =>
            {
                options.AddPolicy(
                    "CorsPolicy",
                    builder => builder.WithOrigins("http://localhost:4200", "https://localhost:5001")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });
            services.AddControllers();
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo {Title = "ETLWebApp", Version = "v1"}); });
            services.AddDbContext<EtlContext>();
            services.AddScoped(typeof(IAuthenticator), typeof(Authenticator));
            services.AddScoped(typeof(ICsvDatasetManager), typeof(CsvDatasetManager));
            services.AddScoped(typeof(ISqlServerDatasetManager), typeof(SqlServerDatasetManager));
            services.AddScoped(typeof(ICsvSerializer), typeof(CsvSerializer));
            services.AddScoped(typeof(ISqlServerSerializer), typeof(SqlServerSerializer));
            services.AddScoped(typeof(IPipelineManager), typeof(PipelineManager));
            services.AddScoped(typeof(IYmlManager), typeof(YmlManager));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors("CorsPolicy");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ETLWebApp v1"));
            }


            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}