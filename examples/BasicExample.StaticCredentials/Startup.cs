using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using qckdev.AspNetCore.Authentication.Basic;

namespace BasicExample.StaticCredentials
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
            services
                .AddAuthentication(BasicAuthenticationDefaults.AuthenticationScheme)
                .AddBasicAuthentication(options =>
                {
                    options.Realm = "Static Credentials API";
                    options.Username = "admin";
                    options.Password = "secretPassword123";
                });

            services.AddAuthorization();
            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Basic Static Credentials API", Version = "v1" });

                c.AddSecurityDefinition(BasicAuthenticationDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "basic",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Description = "Basic authentication header."
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = BasicAuthenticationDefaults.AuthenticationScheme
                            }
                        },
                        System.Array.Empty<string>()
                    }
                });
            });
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

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Basic Static Credentials API v1");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
