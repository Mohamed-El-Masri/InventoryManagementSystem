using InventoryManagementSystem.API.Extensions;
using InventoryManagementSystem.API.Filters;
using InventoryManagementSystem.Repository.Data;
using InventoryManagementSystem.Repository.UnitOfWork;
using InventoryManagementSystem.Service.BackgroundJobs;
using InventoryManagementSystem.Service.MappingProfiles;
using InventoryManagementSystem.Service.Services;
using Hangfire;
using Hangfire.Dashboard;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.ConfigureAuthentication(builder.Configuration);
            builder.Services.AddAutoMapper(typeof(MappingProfile));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IInventoryTransactionService, InventoryTransactionService>();
            builder.Services.AddScoped<IWarehouseService, WarehouseService>();
            builder.Services.AddScoped<INotificationService, NotificationService>();
            builder.Services.AddScoped<IReportService, ReportService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            
            builder.Services.AddScoped<InventoryJobs>();
            builder.Services.ConfigureHangfireJobs(builder.Configuration);

            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<ApiExceptionFilter>();
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangfireAuthorizationFilter() }
            });

            HangfireJobsExtension.ScheduleRecurringJobs();
            app.MapControllers();
            app.Run();
        }
    }

    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            return httpContext.User.Identity?.IsAuthenticated == true &&
                   httpContext.User.IsInRole("Admin");
        }
    }
}
