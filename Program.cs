using CommunityManager.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Options;
using CommunityManager.Service;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddTransient<IEmailService, EmailService>();

        // Add services to the container.
        builder.Services.AddRazorPages();
        builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("StripeSettings"));

        builder.Services.AddDbContext<CommunityContext>(options =>
        {
            options.UseSqlServer("Server=tcp:dt-database-server.database.windows.net,1433;Initial Catalog=communitydb;Persist Security Info=False;User ID=Dtgroup27;Password=Dtpass27;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
        });
        /* BUGGED, BUT WILL USE */
        //Add identities for user authorization
        builder.Services.AddIdentity<CommunityUser, IdentityRole>(options =>
        {
            // Customize Identity options if needed
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = true;
            options.Password.RequiredUniqueChars = 1;
            options.SignIn.RequireConfirmedAccount = false;
        })


            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<CommunityContext>()
            .AddDefaultTokenProviders();




        builder.Services.AddAzureClients(clientBuilder =>
        {
            clientBuilder.AddBlobServiceClient(builder.Configuration["Testing1Storage:blob"]!, preferMsi: true);
            clientBuilder.AddQueueServiceClient(builder.Configuration["Testing1Storage:queue"]!, preferMsi: true);
        });

        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/NewLogin/NewLogin";
                options.AccessDeniedPath = "/NewLogin/NewLogin";
            });



        var app = builder.Build();


        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapRazorPages();
            endpoints.MapGet("/", context =>
            {
                context.Response.Redirect("/NewLogin/NewLogin");
                return Task.CompletedTask;
            });
        });



        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }



        app.UseHttpsRedirection();
        app.UseStaticFiles();




        app.MapRazorPages();

        /* BUGGED, BUT WILL USE */
        //Seed the roles in their proper scope
        /*
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var Context = services.GetRequiredService<CommunityContext>();

            var userManager = services.GetRequiredService<UserManager<CommunityUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            await SeedData.Initialize(Context, userManager, roleManager);
            Context.Database.Migrate();
        }
        */

        app.Run();
    }
}