using Business.Helpers;
using Business.Implementations;
using Business.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Model;
using Model.Account;
using Repository;
using RestaurantServer.Middlewares;

namespace RestaurantServer;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    private IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.Configure<AuthorizationSettings>(Configuration.GetSection("AuthorizationSettings"));
        services.Configure<EncryptionSettings>(Configuration.GetSection("EncryptionSettings"));

        var connString = Configuration.GetConnectionString("DbConnection");
        if (connString == null) return;
        services.AddDbContext<DataContext>(options => options.UseSqlServer(connString));

        services.AddControllersWithViews();

        services.AddTransient<IHashingHelper, HashingHelper>();
        services.AddTransient<IAuthorizationHelper, AuthorizationHelper>();

        services.AddTransient<IUserService, UserService>();
        services.AddTransient<IAccountService, AccountService>();
        services.AddTransient<IBookingService, BookingService>();


        services.AddSwaggerGen(c => { c.SwaggerDoc("v1", 
            new OpenApiInfo { Title = "Restaurant", Version = "v1" }); });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Restaurant");
                options.RoutePrefix = "swagger";
            });
        }
        else
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Restaurant");
                options.RoutePrefix = "swagger";
            });
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseStaticFiles();
        app.UseRouting();

        app.UseMiddleware<AttachUserToContextMiddleware>();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                "default",
                "{controller}/{action=Index}/{id?}");
        });
    }
}