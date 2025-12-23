using AccessControlAPI.Authorization;
using AccessControlAPI.Database;
using AccessControlAPI.Middlewares;
using AccessControlAPI.Repositories;
using AccessControlAPI.Repositories.Interface;
using AccessControlAPI.Services;
using AccessControlAPI.Services.Interface;
using AccessControlAPI.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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

//đăng ký Authentication (xác thực)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) // sử dụng Bearer token
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true, //kiểm tra chữ ký token 
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),  //key dùng để verify chữ ký
            ValidateIssuer = false,  //không kiểm tra nhà phát hành
            ValidateAudience = false, //không kiểm tra người nhận
            ValidateLifetime = true,  //kiểm tra thời gian hết hạn
            ClockSkew = TimeSpan.Zero  //không cho phép sai lệch thời gian
        };
    });

//đăng ký Authorization (phân quyền)
builder.Services.AddAuthorization();
//Custom Policy Provider 
builder.Services.AddSingleton<IAuthorizationPolicyProvider, CustomPolicyProvider>();


//đăng ký CORS 
builder.Services.AddCors(options =>
{
    options.AddPolicy("Development", policy =>
    {
        policy.WithOrigins("http://localhost:5173")  // ← Chỉ định cụ thể domain được phép gọi API
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();  //cho phép gửi cookie, header xác thực
    });
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("Development");

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UsePermissionMiddleware(); 


app.MapControllers();
app.Run();
