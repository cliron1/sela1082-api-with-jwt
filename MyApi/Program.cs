global using MyApi.Config;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.EntityFrameworkCore;
global using MyApi.Data;
global using System.Text.Json;
global using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.AddSecurityDefinition(
        JwtBearerDefaults.AuthenticationScheme,
        new OpenApiSecurityScheme {
            Description = "Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey
        }
    );

    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
    {new OpenApiSecurityScheme{Reference = new OpenApiReference{Type = ReferenceType.SecurityScheme,Id = "Bearer"}},
    new string[] { }
}
  });
});

builder.Services.AddDbContext<MyContext>(opt => {
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Database"));
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
        o.TokenValidationParameters = new TokenValidationParameters {
            ValidIssuer = "liron",
            //ValidAudience = "webapp",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("the-secret-key-that-i-chose-to-get-rid-of-u")),

            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true
        }
    );
builder.Services.AddAuthorization();






var app = builder.Build();

await using(var scope = app.Services.CreateAsyncScope()) {
    using var db = scope.ServiceProvider.GetService<MyContext>();
    await db!.Database.MigrateAsync();
}

// Configure the HTTP request pipeline.
if(app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers()
    .RequireAuthorization();

app.Run();
