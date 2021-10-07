using API.Gateway.AuthOptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace API.Gateway
{
    public class Startup
    {
        private readonly AuthParams _authParams;

        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _authParams = Configuration.GetSection("AuthParams").Get<AuthParams>();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            

            //services.Configure<AuthParams>(Configuration.GetSection("AuthParams"));

            services.AddAuthentication()
                .AddJwtBearer("Key", _ =>
                {
                    _.RequireHttpsMetadata = false;
                    _.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = bool.Parse(_authParams.ValidateIssuer),

                        ValidIssuer = _authParams.Issuer,

                        ValidateAudience = bool.Parse(_authParams.ValidateAudience),

                        ValidAudience = _authParams.Audience,

                        ValidateLifetime = bool.Parse(_authParams.ValidateLifetime),

                        IssuerSigningKey = _authParams.GetSymmetricSecurityKey(),

                        ValidateIssuerSigningKey = bool.Parse(_authParams.ValidateIssuerSigningKey)
                    };
                });

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:3000").AllowAnyMethod().AllowAnyHeader();
                    });
            });
            services.AddOcelot();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseOcelot();
        }
    }
}