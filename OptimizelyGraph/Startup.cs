using OptimizelyGraph.Extensions;
using EPiServer.Cms.Shell;
using EPiServer.Cms.UI.AspNetIdentity;
using EPiServer.Scheduler;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using EPiServer.OpenIDConnect;
using System.Runtime.InteropServices;
using EPiServer.ContentApi.Cms;
using EPiServer.ContentApi.Core.DependencyInjection;
using EPiServer.ContentDefinitionsApi;
using EPiServer.Core;
using EPiServer.Data;
using EPiServer.DependencyInjection;
using EPiServer.Web;
using Baaijte.Optimizely.ImageSharp.Web;
using Verndale.Sitemap.Robots.Generator.Extensions;
using Vite.AspNetCore.Extensions;

namespace OptimizelyGraph;

public class Startup
{
    private readonly IWebHostEnvironment _webHostingEnvironment;
    private readonly IConfiguration _configuration;
    
    public Startup(IWebHostEnvironment webHostingEnvironment, IConfiguration configuration)
    {
        _webHostingEnvironment = webHostingEnvironment;
        _configuration = configuration;

    }

    
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


        if (_webHostingEnvironment.IsDevelopment())
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", Path.Combine(_webHostingEnvironment.ContentRootPath, "App_Data"));

            services.Configure<SchedulerOptions>(options => options.Enabled = false);

        }

        services.AddBaaijteOptimizelyImageSharp();

        services
            .AddCmsAspNetIdentity<ApplicationUser>()
            .AddCms()
            .AddFind()
            .AddAlloy()
            .AddAdminUserRegistration()
            .AddEmbeddedLocalization<Program>()
            .ConfigureForExternalTemplates()
            .Configure<ExternalApplicationOptions>(options => options.OptimizeForDelivery = true);
        // Required by Wangkanai.Detection
        services.AddDetection();

        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromSeconds(10);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;

        });

        services.Configure<SchedulerOptions>(o =>
        {
            o.Enabled = true;
        });

        services.AddOpenIDConnectUI();

        services.AddOpenIddict()
            .AddServer(options => options.DisableAccessTokenEncryption());

        services.AddContentDefinitionsApi(OpenIDConnectOptionsDefaults.AuthenticationScheme);

        services.AddContentDeliveryApi(OpenIDConnectOptionsDefaults.AuthenticationScheme);

        services.AddContentManagementApi(OpenIDConnectOptionsDefaults.AuthenticationScheme, options =>
        {
            options.DisableScopeValidation = false;
            options.RequiredRole = "WebAdmins";
        });

        services.AddOpenIddict()
            .AddServer(options =>
            {
                options.DisableAccessTokenEncryption();
            });

        services.ConfigureForContentDeliveryClient();

        services.ConfigureContentApiOptions(o =>
        {
            o.FlattenPropertyModel = true;
            o.EnablePreviewFeatures = true;
            o.SetValidateTemplateForContentUrl(true);
            o.IncludeSiteHosts = true;
            o.IncludeInternalContentRoots = true;
            o.IncludeNumericContentIdentifier = true;
        });

        services.AddContentGraph(OpenIDConnectOptionsDefaults.AuthenticationScheme);
        services.AddControllersWithViews().AddRazorRuntimeCompilation();
        // Add the Vite Manifest Service.
        services.AddViteServices();

    }



    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Add the image processing middleware.
        app.UseBaaijteOptimizelyImageSharp();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }


        app.UseDetection();
        app.UseSession();
        app.UseSitemapRobotsMiddleware();


        app.UseStaticFiles();
        app.UseRouting();
        app.UseCors(b => b
            .WithExposedContentDeliveryApiHeaders()
            .WithExposedContentDefinitionApiHeaders()
            .WithHeaders("Authorization")
            .AllowAnyMethod()
            .AllowCredentials());

        app.UseAuthentication();
        app.UseAuthorization();

        app.Use(async (context, next) => {
            context.Request.EnableBuffering();
            await next.Invoke();
        });

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapContent();
        });

        app.UseStatusCodePages(context =>
        {
            if (context.HttpContext.Response.HasStarted == false &&
                context.HttpContext.Response.StatusCode == StatusCodes.Status404NotFound &&
                context.HttpContext.Request.Path == "/")
            {
                context.HttpContext.Response.Redirect("/episerver/cms");
            }

            return Task.CompletedTask;
        });

        app.UseViteDevMiddleware();
    }
}
