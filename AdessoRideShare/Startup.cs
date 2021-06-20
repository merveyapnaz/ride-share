using System.Globalization;
using AdessoRideShare.infrastructure.Repository;
using AdessoRideShare.infrastructure.Services;
using AdessoRideShare.infrastructure.Services.Interfaces;
using AdessoRideShare.infrastructure.UnitOfWork;
using AdessoRideShare.model.context;
using AdessoRideShare.model.entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AdessoRideShare
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
            services.AddCors(o => o.AddPolicy("AdessoPolicy", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

            services.AddControllers();

            var sqlConnectionBuilder = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(Configuration["ConnectionStrings:DefaultConnection"]);
            services.AddDbContext<AdessoRideShareContext>(options => options.UseSqlServer(sqlConnectionBuilder.ConnectionString, sqlOptions => sqlOptions.CommandTimeout(600)).EnableDetailedErrors());

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddAntiforgery(o => o.SuppressXFrameOptionsHeader = true);

            InjectRepositories(ref services);
            InjectServices(ref services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseCors("AdessoPolicy");

            app.Use(async (context, next) =>
            {
                context.Features.Get<IHttpMaxRequestBodySizeFeature>().MaxRequestBodySize = 100_000_000;
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Add("X-Permitted-Cross-Domain-Policies", "none");
                context.Response.Headers.Add("X-Xss-Protection", "1; mode=block");
                await next();
            });

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        public void InjectRepositories(ref IServiceCollection services)
        {
            services.AddScoped<IRepository<TravelPlan>, Repository<TravelPlan>>();
        }

        public void InjectServices(ref IServiceCollection services)
        {
            services.AddScoped<ITravelPlanService, TravelPlanService>();
        }
    }
}
