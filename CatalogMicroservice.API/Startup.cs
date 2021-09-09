using AutoMapper;
using CatalogMicroservice.Repository.Context;
using CatalogMicroservice.Repository.Repository;
using CatalogMicroservice.Service.Mapper;
using CatalogMicroservice.Service.Services.CategoryService;
using CatalogMicroservice.Service.Services.ProductService;
using CatalogMicroservice.Service.Services.RabbitMqService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace CatalogMicroservice.API
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
            #region Mapper
            var mappingConfig = new MapperConfiguration(_ =>
            {
                _.AddProfile(new MappingProfile());
            });

            IMapper mapper = mappingConfig.CreateMapper();

            services.AddSingleton(mapper);
            #endregion

            #region DbContext
            services.AddDbContext<CatalogContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("CatalogMicroservice.Repository"));
                options.UseLazyLoadingProxies();
            });
            #endregion

            services.AddScoped(typeof(IRepositoryGeneric<>), typeof(RepositoryGeneric<>));

            services.AddTransient<IProductService, ProductService>();

            services.AddTransient<ICategoryService, CategoryService>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CatalogMicroservice.API", Version = "v1" });
            });

            services.AddHostedService<RabbitMqService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseExceptionHandler("/errors");
                //app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CatalogMicroservice.API v1"));
            }

            app.UseExceptionHandler("/errors");
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}