using Application.Features.Auth.Commands.Login;
using Application.Features.Auth.Commands.Register;
using Application.Interfaces;
using FluentValidation;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Presentation.Endpoints;
using System.Text;
using WebAPI.Endpoints;
using WebAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

var connectring = builder.Configuration.GetConnectionString("DB");
builder.Services.AddDbContext<AppDBContext>(options =>
    options.UseSqlServer(connectring));
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(RegisterUserCommand).Assembly));// Đăng ký MediatR là nó sẽ quét toàn bộ Handler trong project Application
builder.Services.AddValidatorsFromAssembly(typeof(RegisterUserCommand).Assembly);// Đăng ký FluentValidation

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(LoginCommand).Assembly));// Đăng ký MediatR là nó sẽ quét toàn bộ Handler trong project Application
builder.Services.AddValidatorsFromAssembly(typeof(LoginCommand).Assembly);// Đăng ký FluentValidation


//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => //Declared that this syntax would be used for authenticate. And using the JWT bearer standard
//{                                                                                                  //And AddJwtBearer syntax is used for customing the options to check JWT
//    options.TokenValidationParameters = new TokenValidationParameters //List parameters to check
//    {
//        ValidateIssuerSigningKey = true, //Checking this Key is valid, and whether this key is provided by this app?
//        IssuerSigningKey = new SymmetricSecurityKey( //Providing a private key to decrypt the key, its decoding from Jwt to Bytes to compare.
//            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
//        ),
//        ValidateIssuer = true, //Checking the issuer whether did it match? Note: validate: Checking the origin and considering if it valid or not
//        ValidIssuer = builder.Configuration["Jwt:Issuer"],                      //valid: Checking if this value belongs to this app, provided by this app

//        ValidateAudience = true,//Checking this Token is provided to who? Its provied for this app, right?
//        ValidAudience = builder.Configuration["Jwt:Audience"],

//        ValidateLifetime = true,//Checking expireTime
//        ClockSkew = TimeSpan.Zero//Expired = 0 = timeout
//    };
//});

//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("AdminOnly", policy =>
//        policy.RequireRole("Admin"));

//    options.AddPolicy("UserOrAdmin", policy =>
//        policy.RequireRole("User", "Admin"));
//});

builder.Services.AddJwtAuthentication(builder.Configuration);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapAuthEndpoints();
app.MapProductEndpoints();
app.MapCartEndpoints();
app.Run();
