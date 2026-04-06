using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using PJGoFast.Data;
using PJGoFast.Services.Implementations;
using PJGoFast.Services.Interfaces;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IKhachHangService, KhachHangService>();
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<PJGoFastDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.")));
builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", options =>
    {
        // Ghi đè sự kiện chuyển hướng tự động của ASP.NET Core
        options.Events.OnRedirectToLogin = context =>
        {
            // Lấy đường dẫn mà người dùng đang cố gắng truy cập (ví dụ: /TaiXe/LichSu)
            var requestPath = context.Request.Path.Value;

            // Giữ lại đường dẫn cũ để đăng nhập xong quay lại đúng chỗ
            var returnUrl = Uri.EscapeDataString(context.Request.Path + context.Request.QueryString);

            // Kiểm tra xem URL bắt đầu bằng chữ gì để chuyển hướng cho đúng
            if (requestPath.StartsWith("/TaiXe", StringComparison.OrdinalIgnoreCase))
            {
                context.Response.Redirect($"/Login/TaiXeLogin?ReturnUrl={returnUrl}");
            }
            else if (requestPath.StartsWith("/KhachHang", StringComparison.OrdinalIgnoreCase))
            {
                context.Response.Redirect($"/Login/Index?ReturnUrl={returnUrl}");
            }
            else
            {
                // Mặc định các trang nội bộ (/DieuPhoi, /Admin...) thì về trang Admin
                context.Response.Redirect($"/Login/AdminLogin?ReturnUrl={returnUrl}");
            }

            return Task.CompletedTask;
        };

        // Ghi đè sự kiện khi ĐÃ đăng nhập nhưng KHÔNG ĐÚNG ROLE (Ví dụ: Khách hàng cố vào trang Tài xế)
        options.Events.OnRedirectToAccessDenied = context =>
        {
            // Tốt nhất bạn nên tạo 1 trang thông báo "Bạn không có quyền truy cập"
            context.Response.Redirect("/Login/AccessDenied");
            return Task.CompletedTask;
        };
    });




var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
