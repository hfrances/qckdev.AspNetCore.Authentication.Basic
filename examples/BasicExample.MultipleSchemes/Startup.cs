using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using qckdev.AspNetCore.Authentication.Basic;

namespace BasicExample.MultipleSchemes
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();

            services
                .AddAuthentication("ApiKey")
                .AddBasicAuthentication<CredentialsBasedValidator, CredentialsBasedOptions>(
                    "ApiKey",
                    options =>
                    {
                        options.Realm = "Legacy API";
                        options.Username = "ApiKey";
                        options.Password = "your-secret-api-key-token";
                    })
                .AddBasicAuthentication<DatabaseValidator>(
                    "Database",
                    options =>
                    {
                        options.Realm = "Enterprise API";
                    });

            services.AddAuthorization();
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
