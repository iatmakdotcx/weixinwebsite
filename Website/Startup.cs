using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using App.Extensions;
using log4net;
using log4net.Config;
using log4net.Repository;
using MakC.Common;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Website
{
    public class Startup
    {
        public static ILoggerRepository Repository { get; set; }
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Repository = LogManager.CreateRepository(Configuration["Logging:Log4Net:Name"]);
            //指定配置文件，如果这里你遇到问题，应该是使用了InProcess模式，请查看.csproj,并删之
            XmlConfigurator.Configure(Repository, new FileInfo("log4net.config"));            
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            App.Data.DbContext.Init(Configuration["DbCfg:ConnectionString"], Configuration["DbCfg:DbType"]);

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, o =>
            {
                o.LoginPath = new PathString("/login");
                o.AccessDeniedPath = new PathString("/login");
            }).AddCookie(AdminAuthorizeAttribute.AuthenticationScheme, o =>
            {
                o.LoginPath = new PathString("/admin/login");
                o.AccessDeniedPath = new PathString("/admin/login");
            });
            services.AddSingleton<ILoggerHelper, LogHelper>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddSession();
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
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseMiddleware<ExceptionFilter>();

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseCookiePolicy();
            app.UseSession();
            //  app.UseHttpsRedirection();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=home}/{action=Index}/{id?}");
            });
            //app.UseMvc();
        }
    }
}
