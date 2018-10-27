using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PasswordManager.Model;
using System.Security.Claims;

namespace PasswordManager
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

            string connection = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connection));

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;

            });
            services.AddAuthentication(options =>
            {
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie(options => { options.LoginPath = "/Login"; });

            services.AddMvc().AddRazorPagesOptions(options =>
                {
                    options.Conventions.AuthorizeFolder("/");
                    options.Conventions.AllowAnonymousToPage("/Login");
                    options.Conventions.AllowAnonymousToPage("/Register");
                    options.Conventions.AllowAnonymousToPage("/About");
                    options.Conventions.AllowAnonymousToPage("/Contact");
                    options.Conventions.AllowAnonymousToPage("/Privacy");
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
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
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }
            app.Use(async (context, next) =>
            {
                //Если обнаружена xss-атака, браузер удалит опасные фрагменты 
                context.Response.Headers.Add("x-xss-protection", "1");
                //Разрешение запуска скриптов только с текущего сайта
                context.Response.Headers.Add("content-security-policy", "script-src 'self'");
                //Блокировка фреймов
                context.Response.Headers.Add("X-Frame-Options", "DENY");
                //Проверка загружаемых файлов
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                //Скрытие имени нашего сайта при переходе на чужой сайт 
                context.Response.Headers.Add("Referrer-Policy", "no-referrer");
                //HTTPS only
                context.Response.Headers.Add("Strict-Transport-Securit", "max-age=31536000; includeSubDomains");
                await next();
            });
            
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            
            app.UseAuthentication();
            app.UseMvc();

        }
    }
}
