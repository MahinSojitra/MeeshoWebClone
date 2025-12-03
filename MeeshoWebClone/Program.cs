using MeeshoWebClone.Data;
using MeeshoWebClone.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add MediatR for CQRS pattern
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// Add database service.
builder.Services.AddDbContext<MeeshoAppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MeeshoDbContext")));

// Add Identity services.
builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
    .AddEntityFrameworkStores<MeeshoAppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/Account/AccessDenied";
});

var app = builder.Build();

// Apply database migrations and seed roles & admin user
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<MeeshoAppDbContext>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    var userManager = services.GetRequiredService<UserManager<User>>();

    dbContext.Database.Migrate();

    await EnsureRolesExist(roleManager);
    await EnsureAdminAndSellerUserExists(userManager, roleManager);
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

/// Ensure required roles exist
async Task EnsureRolesExist(RoleManager<IdentityRole<Guid>> roleManager)
{
    string[] roles = { "Admin", "User", "Seller" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole<Guid> { Name = role });
        }
    }
}

/// Ensure Admin and Seller Users Exist
async Task EnsureAdminAndSellerUserExists(UserManager<User> userManager, RoleManager<IdentityRole<Guid>> roleManager)
{
    // Admin User Details
    string adminEmail = "admin@meesho.com";
    string adminPassword = "Admin@123";
    string adminRole = "Admin";

    // Seller User Details
    string sellerEmail = "seller@meesho.com";
    string sellerPassword = "Seller@123";
    string sellerRole = "Seller";

    // Create Admin User
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new User
        {
            Id = Guid.NewGuid(),
            UserName = "Admin",
            Email = adminEmail,
            Status = MeeshoWebClone.Enums.VerificationStatus.Approved,
            PhoneNumber = "9104851608",
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(adminUser, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, adminRole);
            Console.WriteLine("Admin user created.");
        }
        else
        {
            Console.WriteLine("Failed to create admin user:");
            foreach (var error in result.Errors)
            {
                Console.WriteLine($"- {error.Description}");
            }
        }
    }
    else
    {
        Console.WriteLine("Admin user already exists.");
    }

    // Create Seller User
    var sellerUser = await userManager.FindByEmailAsync(sellerEmail);
    if (sellerUser == null)
    {
        sellerUser = new User
        {
            Id = Guid.NewGuid(),
            UserName = "SystemSeller",
            Email = sellerEmail,
            Status = MeeshoWebClone.Enums.VerificationStatus.Approved,
            PhoneNumber = "9974851608",
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(sellerUser, sellerPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(sellerUser, sellerRole);
            Console.WriteLine("Seller user created.");
        }
        else
        {
            Console.WriteLine("Failed to create seller user:");
            foreach (var error in result.Errors)
            {
                Console.WriteLine($"- {error.Description}");
            }
        }
    }
    else
    {
        Console.WriteLine("Seller user already exists.");
    }
}