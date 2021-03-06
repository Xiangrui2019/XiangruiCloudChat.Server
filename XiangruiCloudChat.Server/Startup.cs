﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebPush;
using XiangruiCloudChat.Server.Core;
using XiangruiCloudChat.Server.Core.Services.ToAPIServer;
using XiangruiCloudChat.Server.Core.Services.ToOSSServer;
using XiangruiCloudChat.Server.Core.Services.ToStargateServer;
using XiangruiCloudChat.Server.Data;
using XiangruiCloudChat.Server.Middlewares;
using XiangruiCloudChat.Server.Models;
using XiangruiCloudChat.Server.Services;

namespace XiangruiCloudChat.Server
{
    public class Startup
    {
        private IHostingEnvironment Environment { get; }
        private IConfiguration Configuration { get; }
        private SameSiteMode Mode { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Environment = environment;
            Configuration = configuration;
            Mode = Convert.ToBoolean(configuration["LaxCookie"]) ? SameSiteMode.Lax : SameSiteMode.None;
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureLargeFileUpload();

            services.AddDbContext<ApplicationDbContext>(options =>
                {
                    if (Environment.IsDevelopment())
                    {
                        options.UseSqlServer(Configuration.GetConnectionString("DevlopementConnection"));
                    }
                    else
                    {
                        options.UseSqlServer(Configuration.GetConnectionString("ProductionConnection"));
                    }
                });

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(t => t.Cookie.SameSite = Mode);

            services.AddOpenAuth<ApplicationUser>();
            services.AddScoped<UserService>();
            services.AddScoped<SecretService>();
            services.AddScoped<VersionChecker>();
            services.AddScoped<WebPushClient>();
            services.AddScoped<ThirdPartyPushService>();
            services.AddScoped<ChannelService>();
            services.AddScoped<PushMessageService>();
            services.AddScoped<ChatPushService>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseAuthenticationFromConfiguration(Configuration, "Chat");
            app.UseMiddleware<HandleCorsOptionsMiddleware>();
            app.UseAuthentication();
            app.UseLanguageSwitcher();

            app.UseMvc(routes =>
                routes.MapRoute("Default", "/{controller=Home}/{action=Index}/{id?}"));
        }
    }
}
