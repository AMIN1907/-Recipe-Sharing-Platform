using RecipeSharing.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using RecipeSharing.Controllers;
using RecipeSharing;

using RecipeSharing.Repository;
using RecipeSharing.Repository.Services;
using RecipeSharing.models;
 using RecipeSharing.Repository.Repository;
using NuGet.Protocol.Core.Types;




namespace RecipeSharing
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
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });
            });
                services.AddCors(options =>
                    {
                     options.AddPolicy("AllowAllOrigins",
                     builder =>
                     {
                          builder.AllowAnyOrigin()
                   .AllowAnyHeader()
                   .AllowAnyMethod();
                     });
                    });



            //services.AddScoped<IappointmentRepository, appointmentRepository>();
            services.AddScoped<ImangeprofileRepository, mangeprofilerepository>();
            services.AddScoped<iRecipeRepository, RecipeRepository>();

            services.AddScoped<IAuthService, AuthService>();

            services.AddIdentity<ApplicationUser, IdentityRole>()
                 .AddEntityFrameworkStores<appDbcontext1>()
                 .AddDefaultTokenProviders();


          
            services.Configure<JWT>(Configuration.GetSection("JWT"));
            services.AddDbContext<appDbcontext1>(options =>
               options.UseSqlServer(Configuration.GetConnectionString("defaultsqlconection"))
               ,ServiceLifetime.Scoped
           );





            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(o =>
                {
                    o.RequireHttpsMetadata = false;
                    o.SaveToken = false;
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidIssuer = Configuration["JWT:Issuer"],
                        ValidAudience = Configuration["JWT:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Key"]))
                    };
                });

            services.AddControllers();


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "RecipeSharing", Version = "v1" });

                // Add JWT Authentication support in Swagger
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "RecipeSharing"));
            }

            app.UseHttpsRedirection();

            app.UseCors("AllowAll");
            app.UseCors("AllowAllOrigins");
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