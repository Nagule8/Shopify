// This Startup file is based on ASP.NET Core new project templates and is included
// as a starting point for DI registration and HTTP request processing pipeline configuration.
// This file will need updated according to the specific scenario of the application being upgraded.
// For more information on ASP.NET Core startup files, see https://docs.microsoft.com/aspnet/core/fundamentals/startup

using ShopApi.Helpers;
using ShopApi.Interface;
using ShopApi.Models;
using ShopApi.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using ShopApi.Authorize;
using ShopApi.Data;
using System.Text.Json.Serialization;
using Serilog;

namespace ShopApi
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
            services.AddControllers().AddJsonOptions(x => {
                // serialize enums as strings in api responses (e.g. Role)
                x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            /**services.AddControllers(options =>
            {
                options.Filters.Add<CustomExceptionFiler>();
            });**/

            services.AddCors();

            //configure DI for application services
            services.AddScoped<ICommonRepository<Category>,CategoryRepository>();
            services.AddScoped<ICommonRepository<Item>, ItemRepository>();
            services.AddScoped<ICommonRepository<RegisterUser>, UserRepository>();
            services.AddScoped<ICommonRepository<CartItem>, CartItemRepository>();


            services.AddScoped<IJwtUtils, JWTUtils>();

            services.AddScoped<CategoryApiContext>();
            services.AddScoped<IUserRepository,UserRepository>();
            services.AddScoped<ICategoryRepository,CategoryRepository>();
            services.AddScoped<IItemRepository,ItemRepository>();
            services.AddScoped<ICartItemRepository,CartItemRepository>();

            services.AddScoped<UserTracker>();

            services.AddDbContext<CategoryApiContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DockerDb")));

            // configure strongly typed settings object
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            //configure HttpContextAccessor 
            services.AddHttpContextAccessor();

            services.AddDistributedMemoryCache();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(ConfigurationPath.Combine(env.WebRootPath + "\\Photos\\")),
                RequestPath = "/Photos"
            }) ;

            app.UseSerilogRequestLogging();
            app.UseRouting();

            app.UseCors(options => options
                .WithOrigins(new []{ "http://localhost:3000", "http://localhost:5000" })
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
            );

            //Global error handler
            app.UseMiddleware<ErrorHandlerMiddleware>();
            //Custom jwt auth middleware
            app.UseMiddleware<JWTMiddleware>();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
