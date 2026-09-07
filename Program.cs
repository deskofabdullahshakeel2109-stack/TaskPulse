using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

using TaskPulse.Api.Data;
using TaskPulse.Api.Models;

using TaskPulse.Api.Repositories;
using TaskPulse.Api.Repositories.ActivityLogs;
using TaskPulse.Api.Repositories.Comments;
using TaskPulse.Api.Repositories.Dashboard;
using TaskPulse.Api.Repositories.Interfaces;
using TaskPulse.Api.Repositories.Notifications;
using TaskPulse.Api.Repositories.ProjectMembers;
using TaskPulse.Api.Repositories.Projects;
using TaskPulse.Api.Repositories.Reports;
using TaskPulse.Api.Repositories.Tasks;

using TaskPulse.Api.Services.ActivityLogs;
using TaskPulse.Api.Services.Auth;
using TaskPulse.Api.Services.Comments;
using TaskPulse.Api.Services.CurrentUser;
using TaskPulse.Api.Services.Dashboard;
using TaskPulse.Api.Services.Notifications;
using TaskPulse.Api.Services.ProjectMembers;
using TaskPulse.Api.Services.Projects;
using TaskPulse.Api.Services.Reports;
using TaskPulse.Api.Services.Tasks;
using TaskPulse.Api.Services.Users;

using TaskPulse.Api.Middleware;


var builder = WebApplication.CreateBuilder(args);


// ============================================================
// DATABASE
// ============================================================

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));


// ============================================================
// JWT CONFIGURATION
// ============================================================

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException(
        "JWT key is not configured.");

var jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException(
        "JWT issuer is not configured.");

var jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException(
        "JWT audience is not configured.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtKey)),

                ClockSkew = TimeSpan.Zero
            };
    });


// ============================================================
// AUTHORIZATION
// ============================================================

builder.Services.AddAuthorization();

builder.Services.AddHttpContextAccessor();


// ============================================================
// CORS
// ============================================================

builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactFrontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:3000",
                "https://localhost:3000",
                "http://localhost:5173",
                "https://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


// ============================================================
// REPOSITORIES
// ============================================================

builder.Services.AddScoped<
    IUserRepository,
    UserRepository>();

builder.Services.AddScoped<
    IProjectRepository,
    ProjectRepository>();

builder.Services.AddScoped<
    IProjectMemberRepository,
    ProjectMemberRepository>();

builder.Services.AddScoped<
    ITaskRepository,
    TaskRepository>();

builder.Services.AddScoped<
    IActivityLogRepository,
    ActivityLogRepository>();

builder.Services.AddScoped<
    IDashboardRepository,
    DashboardRepository>();

builder.Services.AddScoped<
    ICommentRepository,
    CommentRepository>();

builder.Services.AddScoped<
    INotificationRepository,
    NotificationRepository>();

builder.Services.AddScoped<
    IReportRepository,
    ReportRepository>();


// ============================================================
// SERVICES
// ============================================================

builder.Services.AddScoped<
    IAuthService,
    AuthService>();

builder.Services.AddScoped<
    IUserService,
    UserService>();

builder.Services.AddScoped<
    IProjectService,
    ProjectService>();

builder.Services.AddScoped<
    IProjectMemberService,
    ProjectMemberService>();

builder.Services.AddScoped<
    ITaskService,
    TaskService>();

builder.Services.AddScoped<
    IActivityLogService,
    ActivityLogService>();

builder.Services.AddScoped<
    IDashboardService,
    DashboardService>();

builder.Services.AddScoped<
    ICommentService,
    CommentService>();

builder.Services.AddScoped<
    INotificationService,
    NotificationService>();

builder.Services.AddScoped<
    ICurrentUserService,
    CurrentUserService>();

builder.Services.AddScoped<
    IReportService,
    ReportService>();


// ============================================================
// PASSWORD HASHING
// ============================================================

builder.Services.AddScoped<
    IPasswordHasher<User>,
    PasswordHasher<User>>();


// ============================================================
// CONTROLLERS
// ============================================================

builder.Services.AddControllers();


// ============================================================
// SWAGGER
// ============================================================

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description =
                "Enter your JWT token."
        });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [
                new OpenApiSecuritySchemeReference(
                    "Bearer",
                    document)
            ] = []
        });
});


// ============================================================
// BUILD APPLICATION
// ============================================================

var app = builder.Build();


// ============================================================
// DATABASE MIGRATION + SEEDING
// ============================================================

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var dbContext =
        services.GetRequiredService<ApplicationDbContext>();

    var passwordHasher =
        services.GetRequiredService<IPasswordHasher<User>>();

    await dbContext.Database.MigrateAsync();

    await DatabaseSeeder.SeedAsync(
        dbContext,
        passwordHasher,
        app.Configuration);
}


// ============================================================
// SWAGGER
// ============================================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// ============================================================
// HTTP PIPELINE
// ============================================================

app.UseHttpsRedirection();

app.UseCors("ReactFrontend");

app.UseAuthentication();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseAuthorization();

app.MapControllers();


// ============================================================
// RUN APPLICATION
// ============================================================

app.Run();