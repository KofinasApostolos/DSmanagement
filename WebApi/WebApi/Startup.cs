using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using WebApi.Data;
using WebApi.Helpers;
using WebApi.Models;

namespace WebApi
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
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ElevatedRights", policy =>
                  policy.RequireRole("0", "2"));
            });
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(Options =>
                {

                    var resolver = Options.SerializerSettings.ContractResolver;

                    if (resolver != null)
                        (resolver as DefaultContractResolver).NamingStrategy = null;

                });

            services.AddDbContext<DanceSchoolContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DevConnection")));
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddCors();
            services.Configure<CloudinarySettings>(Configuration.GetSection("CloudinarySettings"));
            services.AddScoped<IUsersRepository, UsersRepository>();
            services.AddScoped<IUsersubscriptionRepository, UsersubscriptionRepository>();
            services.AddScoped<ITeachingProgramRepository, TeachingProgramRepository>();
            services.AddScoped<IRolesRepository, RolesRepository>();
            services.AddScoped<ISubscriptionsRepository, SubscriptionsRepository>();
            services.AddScoped<ILessonsRepository, LessonsRepository>();
            services.AddScoped<IBookingsRepository, BookingsRepository>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(builder =>
                {
                    builder.Run(async context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                        var error = context.Features.Get<IExceptionHandlerFeature>();

                        if (error != null)
                        {
                            context.Response.AddApplicationError(error.Error.Message);
                            await context.Response.WriteAsync(error.Error.Message);

                        }
                    });
                });
            }
            app.UseAuthentication();
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseMvc();
        }
    }
}
