using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using AuthApiSesh.Database;
using AuthApiSesh.Service;
using AuthApiSesh.Constants;
using AuthApiSesh.Settings;


namespace AuthApiSesh
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
            services.AddDbContext<AppDbContext>(optionsAction => optionsAction.UseLazyLoadingProxies().UseSqlServer(Configuration["ConnectionStrings:DefaultConnection"]));

            services.AddSingleton<IJwtService, JwtService>();
            
            services.AddSingleton<IEmailService, EmailService>();

            services.AddControllersWithViews().AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            #region settings
            services.Configure<MailSettings>(Configuration.GetSection("MAIL"));
            services.Configure<JwtSettings>(Configuration.GetSection("JWT"));
            #endregion

            #region auth
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Access", policy =>{
                    policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireClaim(TokenClaims.Type, TokenTypes.Access);
                    });
                options.AddPolicy("Refresh", policy =>{
                    policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireClaim(TokenClaims.Type, TokenTypes.Refresh);
                    });
                options.AddPolicy("Confirm", policy =>{
                    policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireClaim(TokenClaims.Type, TokenTypes.Confirm);
                });
            });
            #endregion

            #region  jwt
            services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(
                options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = Configuration["JWT:Issuer"],

                        ValidateAudience = true,
                        ValidAudience = Configuration["JWT:Audience"],
                        // ValidAudiences = Configuration.GetSection("JWT").GetValue<List<string>>("AUDIENCE"),

                        ValidateLifetime = true,

                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["JWT:SecretKey"])),
                        ValidateIssuerSigningKey = true
                    };
                }
            );
            #endregion

            services.AddMemoryCache();
            
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            //app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
