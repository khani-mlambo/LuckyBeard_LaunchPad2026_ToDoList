using LuckyBeardAPI_Launchpad2026.Controllers;
using LuckyBeardAPI_Launchpad2026.Data;
using LuckyBeardAPI_Launchpad2026.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text;
using LuckyBeardAPI_Launchpad2026.Data;
using LuckyBeardAPI_Launchpad2026.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using static Microsoft.Extensions.Configuration.IConfiguration;

var builder = WebApplication.CreateBuilder(args);

//config
var cfg = builder.Configuration.GetSection("Jwt");

//db context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//services
builder.Services.AddScoped<TodoService>();
builder.Services.AddScoped<AuthController>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//authentication / jwt
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience    = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = cfg["Issuer"],
            ValidAudience = cfg["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(cfg["Key"]!))

        };
    });


builder.Services.AddAuthorization();
// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "LuckyBeard Launchpad 2026 API",
    });

    // JWT in Swagger
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Paste your JWT here (no 'Bearer ' prefix)'",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    c.AddSecurityDefinition("Bearer", securityScheme);

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, Array.Empty<string>() }
    });
});

builder.Services.AddControllers();

builder.Services.AddMvc();
builder.Services.AddControllers();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();



app.MapControllers();


app.Run();
