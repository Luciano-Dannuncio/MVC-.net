using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MVCnetcore.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;


using MVCnetcore.Models.DB;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace MVCnetcore
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

            services.AddControllersWithViews();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(config =>
            {
                config.Cookie.Name = "Cookie.Basics";
                config.LoginPath = "/LogIn";
            });


            // services.AddScoped<VerifySession>();
            /* services.AddSession(options => {
                 options.IdleTimeout = TimeSpan.FromMinutes(1);//You can set Time   
             });
             services.AddMvc(config => {
                 var policy = new AuthorizationPolicyBuilder()
                                 .RequireAuthenticatedUser()
                                 .Build();
                 config.Filters.Add(new AuthorizeFilter(policy));
         
            
            services.AddDefaultIdentity<IdentityUser, IdentityRole>(
                options => options.SignIn.RequireConfirmedAccount = false)
             
             .AddDefaultTokenProviders()
             .AddEntityFrameworkStores<AlkemyChallengeCDBContext>();
              */

            //.AddUserStore<Models.DB.Users>()
            //.AddRoleStore<Models.DB.Roles>();
            services.AddSession();
            services.AddMvc();
            services.AddRazorPages();
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");

            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();
    
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=LogIn}/{action=Index}/");
            });
        }
    }
}
