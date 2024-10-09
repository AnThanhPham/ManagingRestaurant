using ManagingRestaurant.Data;
using ManagingRestaurant.Models;
using ManagingRestaurant.Services;
using ManagingRestaurant.Upload;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using System;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions();                                        // Kích hoạt Options
var mailsettings = builder.Configuration.GetSection("MailSettings");  // đọc config
builder.Services.Configure<MailSettings>(mailsettings);               // đăng ký để Inject

builder.Services.AddTransient<IEmailSender, SendMailService>();        // Đăng ký dịch vụ Mail

// đăng ký dịch vụ upload file
builder.Services.AddTransient<IBufferedFileUploadService, BufferedFileUploadLocalService>();

//Đăng ký RestaurantContext là một DbContext của ứng dụng
//builder.Services.AddDbContext<RestaurantContext>(options => options
//.UseSqlServer(builder.Configuration.GetConnectionString("RestaurantContext")));
builder.Services.AddDbContext<RestaurantContext>(options => {
    string connectString = builder.Configuration.GetConnectionString("RestaurantContext");
    options.UseSqlServer(connectString);
});


//builder.Services.AddRazorPages();
//// services.AddTransient(typeof(ILogger<>), typeof(Logger<>)); //Serilog
//builder.Services.Configure<RazorViewEngineOptions>(options => {
//    // /View/Controller/Action.cshtml
//    // /MyView/Controller/Action.cshtml

//    // {0} -> ten Action
//    // {1} -> ten Controller
//    // {2} -> ten Area
//    options.ViewLocationFormats.Add("/MyView/{1}/{0}" + RazorViewEngine.ViewExtension);

//    options.AreaViewLocationFormats.Add("/My  /{2}/Views/{1}/{0}.cshtml");

//});

// services.AddSingleton<ProductService>();
// services.AddSingleton<ProductService, ProductService>();
// services.AddSingleton(typeof(ProductService));
//services.AddSingleton(typeof(ProductService), typeof(ProductService));
//services.AddSingleton<PlanetService>();

// Đăng ký các dịch vụ của Identity
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<RestaurantContext>()
    .AddDefaultTokenProviders();

// Truy cập IdentityOptions
builder.Services.Configure<IdentityOptions>(options => {
    // Thiết lập về Password
    options.Password.RequireDigit = false; // Không bắt phải có số
    options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
    options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
    options.Password.RequireUppercase = false; // Không bắt buộc chữ in
    options.Password.RequiredLength = 3; // Số ký tự tối thiểu của password
    options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

    // Cấu hình Lockout - khóa user
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Khóa 5 phút
    options.Lockout.MaxFailedAccessAttempts = 5; // Thất bại 5 lầ thì khóa
    options.Lockout.AllowedForNewUsers = true;

    // Cấu hình về User.
    options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true; // Email là duy nhất

    // Cấu hình đăng nhập.
    options.SignIn.RequireConfirmedEmail = true; // Cấu hình xác thực địa chỉ email (email phải tồn tại)
    options.SignIn.RequireConfirmedPhoneNumber = false; // Xác thực số điện thoại
    options.SignIn.RequireConfirmedAccount = true;

});

// Cấu hình Cookie
builder.Services.ConfigureApplicationCookie(options => {
    options.LoginPath = "/login/";
    options.LogoutPath = "/logout/";
    options.AccessDeniedPath = "/khongduoctruycap.html";
});

//Seed Data
/*SeedData seed = new(builder.Services.BuildServiceProvider().CreateScope().ServiceProvider.GetRequiredService<RestaurantContext>());
await seed.SeedRole();
await seed.SeedUser();*/

builder.Services.AddAuthentication()
                    .AddGoogle(options =>
                    {
                        var gconfig = builder.Configuration.GetSection("Authentication:Google");
                        options.ClientId = gconfig["ClientId"];
                        options.ClientSecret = gconfig["ClientSecret"];
                        // https://localhost:5001/signin-google
                        options.CallbackPath = "/dang-nhap-tu-google";
                    });
//.AddFacebook(options => {
//    var fconfig = builder.Configuration.GetSection("Authentication:Facebook");
//    options.AppId = fconfig["AppId"];
//    options.AppSecret = fconfig["AppSecret"];
//    options.CallbackPath = "/dang-nhap-tu-facebook";
//})
// .AddTwitter()
// .AddMicrosoftAccount()

builder.Services.AddSingleton<IdentityErrorDescriber, AppIdentityErrorDescriber>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ViewManageMenu", builder =>
    {
        builder.RequireAuthenticatedUser();
        builder.RequireRole(RoleName.Administrator);
    });
});

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
      name: "areas",
      pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );
});

app.MapAreaControllerRoute(
    name: "contact",
    pattern: "/{controller}/{action=Index}/{id?}",
    areaName: "ProductManage"
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
