using Application.Features.Auth.Commands.Login;
using Application.Features.Auth.Commands.Register;
using Domain.Interfaces;
using FluentValidation;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Presentation.Endpoints;

var builder = WebApplication.CreateBuilder(args);

var connectring = builder.Configuration.GetConnectionString("DB");
builder.Services.AddDbContext<AppDBContext>(options =>
    options.UseSqlServer(connectring));
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(RegisterUserCommand).Assembly));// Đăng ký MediatR là nó sẽ quét toàn bộ Handler trong project Application
builder.Services.AddValidatorsFromAssembly(typeof(RegisterUserCommand).Assembly);// Đăng ký FluentValidation

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(LoginCommand).Assembly));// Đăng ký MediatR là nó sẽ quét toàn bộ Handler trong project Application
builder.Services.AddValidatorsFromAssembly(typeof(LoginCommand).Assembly);// Đăng ký FluentValidation

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapAuthEndpoints();
app.Run();
