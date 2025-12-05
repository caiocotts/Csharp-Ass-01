using Assignment_1.Services;
using Assignment01.Data;
using Assignment01.Models;
using Assignment01.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

namespace Assignment01;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        builder.Services.AddRazorPages(); // <-- REQUIRED FOR IDENTITY PAGES

        builder.Services.AddTransient<IEmailSender, EmailSender>();

        // Session for shopping cart
        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<ICartService, SessionCartService>();

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddDefaultIdentity<User>(options => { options.SignIn.RequireConfirmedAccount = true; })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>();


        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var userManager = services.GetRequiredService<UserManager<User>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roleNames = { "Admin", "Organizer", "Attendee" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                    await roleManager.CreateAsync(new IdentityRole(roleName));
            }


            //SEEDING USERS
            var usersToSeed = new List<(string Email, string Name, string Role)>
            {
                ("admin@example.com", "Admin User", "Admin"),
                ("organizer@example.com", "Organizer User", "Organizer"),
                ("user@example.com", "Normal User", "Attendee")
            };

            foreach (var (email, name, role) in usersToSeed)
            {
                var user = await userManager.FindByEmailAsync(email);
                if (user != null) continue;
                user = new User { UserName = email, Email = email, FullName = name, EmailConfirmed = true };
                var result = await userManager.CreateAsync(user, "SecurePassword123!");
                if (!result.Succeeded) throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
                await userManager.AddToRoleAsync(user, role);
            }
        }
        // =============================================

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        
        app.UseHttpsRedirection();
        app.UseRouting();

        app.UseSession(); // <-- REQUIRED FOR SHOPPING CART

        // for the razor pages
        app.UseAuthentication(); // <-- REQUIRED BEFORE Authorization

        app.UseAuthorization();

        app.MapStaticAssets();

        app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();

        app.MapRazorPages(); // <-- REQUIRED FOR Identity

        app.Run();
    }
}