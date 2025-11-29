using Assignment01.Data;
using Assignment01.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class Program {
    public static async Task Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        builder.Services.AddRazorPages(); // <-- REQUIRED FOR IDENTITY PAGES

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddDefaultIdentity<User>(options => { options.SignIn.RequireConfirmedAccount = true; })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>();



        var app = builder.Build();

        // ==== ADD SEEDING LOGIC HERE ====
        using (var scope = app.Services.CreateScope()) {
            var services = scope.ServiceProvider;
            var userManager = services.GetRequiredService<UserManager<User>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roleNames = { "Admin", "Organizer", "Attendee" };
            foreach (var roleName in roleNames) {
                if (!await roleManager.RoleExistsAsync(roleName))
                    await roleManager.CreateAsync(new IdentityRole(roleName));
            }

            // Seed admin user
            var adminUser = await userManager.FindByEmailAsync("admin@example.com");
            if (adminUser == null) {
                adminUser = new User {
                    UserName = "admin@example.com",
                    Email = "admin@example.com",
                    FullName = "Admin User",
                    EmailConfirmed = true
                };

                //ADMIN PASSWORD
                await userManager.CreateAsync(adminUser, "SecurePassword123!");
                
                //ADMIN ROLE DECLARATION
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
        // =============================================

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment()) {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }



        app.UseHttpsRedirection();
        app.UseRouting();

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