using AccessControlAPI.Database;
using AccessControlAPI.Repositories;
using AccessControlAPI.Repositories.Interface;
using AccessControlAPI.Services;
using AccessControlAPI.Services.Interface;
using AccessControlAPI.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Singleton: Tạo 1 instance duy nhất cho toàn bộ ứng dụng, dùng chung mọi nơi.
//Scoped: Tạo 1 instance cho mỗi HTTP request, dùng chung trong request đó.
//Transient: Tạo mới mỗi lần inject, không giữ state, lightweight.

//đăng ký DB & Utils
builder.Services.AddScoped<IOracleDb, OracleDb>();
builder.Services.AddScoped<JwtTokenHelper>();
builder.Services.AddSingleton<EncryptHelper>();
builder.Services.AddSingleton<ConfigurationHelper>();
builder.Services.AddSingleton<LogHelper>();


//đăng ký repository
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IFunctionRepository, FunctionReponsitory>();
builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
builder.Services.AddScoped<IUserFunctionRepository, UserFunctionRepository>();
builder.Services.AddScoped<IRoleFunctionRepository, RoleFunctionRepository>();

//đăng ký service
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IFunctionService, FunctionService>(); 
builder.Services.AddScoped<IUserFunctionService, UserFunctionService>();
builder.Services.AddScoped<IRoleFunctionService, RoleFunctionService>();
builder.Services.AddScoped<IUserRoleService, UserRoleService>();
builder.Services.AddScoped<IAuthService, AuthService>();



var jwtKey = builder.Configuration["Jwt:Key"];

if (string.IsNullOrEmpty(jwtKey))
{
    throw new Exception("JWT Key chưa được cấu hình trong appsettings.json");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// ========== CORS ==========
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");
app.UseAuthentication();  // ← Phải có và phải TRƯỚC UseAuthorization
app.UseAuthorization(); 

app.MapControllers();

app.Run();
