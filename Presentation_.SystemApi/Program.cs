using Infrastructure.System.Extention;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Threading.RateLimiting;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddPresistence(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    // Add JWT Bearer token support in Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization Example: 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

var app = builder.Build();
app.UseRateLimiter(); // يجب وضعه قبل 

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    // List of roles to seed
    var roles = new[] { "User", "Admin" };

    foreach (var role in roles)
    {
        // Check if the role already exists
        if (!await roleManager.RoleExistsAsync(role))
        {
            // Create the role if it doesn't exist
            await roleManager.CreateAsync(new IdentityRole(role));
            Console.WriteLine($"Role '{role}' created successfully.");
        }
        else
        {
            Console.WriteLine($"Role '{role}' already exists.");
        }
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapControllers();

app.Run();
