using System.Collections.Generic;
using AngleSharp;
using AngleSharp.Html.Parser;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using tv.api.Sources;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace tv.api
{
    public delegate ISource SourceResolver(string key);

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
            //Use the default configuration for AngleSharp
            var config = AngleSharp.Configuration.Default.WithJs();

            //Create a new context for evaluating webpages with the given config
            var context = BrowsingContext.New(config);

            //Get Parser instance
            var parser = context.GetService<IHtmlParser>();

            //Register Parser instance for DI
            services.AddScoped(p => parser);

            //source handlers
            services.AddTransient<DF>();
            services.AddTransient<Z5>();
            services.AddTransient<VK>();
            //services.AddTransient<MX>();

            services.AddTransient<SourceResolver>(sourceProvider => key =>
            {
                switch (key)
                {
                    case "df":
                        return sourceProvider.GetService<DF>();
                    case "z5":
                        return sourceProvider.GetService<Z5>();
                    case "vk":
                        return sourceProvider.GetService<VK>();
                    //case "mx":
                    //    return sourceProvider.GetService<MX>();
                    default:
                        throw new KeyNotFoundException($"Unknown source requested - {key}!");
                }
            });

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddNewtonsoftJson();

            services.AddControllers();
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseExceptionHandler("/error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseEndpoints(e => e.MapControllers());
        }
    }
}
