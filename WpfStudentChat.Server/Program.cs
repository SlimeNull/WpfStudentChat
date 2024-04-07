using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StudentChat.Server.Models;
using StudentChat.Server.Models.Database;
using StudentChat.Server.Services;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme()
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Authentication token",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer",
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme()
            {
                Reference = new OpenApiReference()
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            } , new string[]{}
        }
    });
});

builder.Services.AddSingleton<NotifyService>();
builder.Services.AddScoped<AuthService>();

#if DEBUG
builder.Services.AddSqlite<ChatServerDbContext>("Data Source=bin/ChatServer.db");
#else
builder.Services.AddSqlite<ChatServerDbContext>("Data Source=ChatServer.db");
#endif

builder.Services.Configure<AppConfig>(builder.Configuration.GetSection("AppConfig"));

// ��Ȩ
builder.Services.AddAuthentication(options => options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["AppConfig:JwtIssuer"],
            ValidAudience = configuration["AppConfig:JwtAudience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["AppConfig:JwtSecret"] ?? string.Empty)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30),
            RequireExpirationTime = true,
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.StartAsync();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ChatServerDbContext>();
    await dbContext.Database.EnsureCreatedAsync();

    if (!dbContext.Users.Any(u => u.UserName == "Test"))
    {
        dbContext.Users.Add(new User()
        {
            UserName = "Test",
            PasswordHash = "TestHash"
        });
    }

    if (!dbContext.Users.Any(u => u.UserName == "Test2"))
    {
        dbContext.Users.Add(new User()
        {
            UserName = "Test2",
            PasswordHash = "TestHash"
        });
    }

    await dbContext.SaveChangesAsync();
}

await app.WaitForShutdownAsync();
