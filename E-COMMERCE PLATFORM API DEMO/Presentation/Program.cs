using Application.Common.Behaviors;
using Application.Common.Lalamove;
using Application.Common.Lalamove.Service;
using Application.Common.Pricing;
using Application.Features.Auth.Commands.Login;
using Application.Features.Auth.Commands.Register;
using Application.Interfaces;
using FluentValidation;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Loyalty.Grpc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Presentation.Endpoints;
using Presentation.Extensions;
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
builder.Services.AddScoped<IOrderRepository,  OrderRepository>();
builder.Services.AddScoped<ICouponRepository, CouponRepository>();
builder.Services.AddScoped<ILoyaltyClient, GrpcLoyaltyClient>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IVariantRepository, VariantRepository>();
builder.Services.AddScoped<IPromotionRuleRepository, PromotionRuleRepository>();
builder.Services.AddScoped<IPricingEngine, PricingEngine>();
builder.Services.AddScoped<IShipmentRepository, ShipmentRepository>();
builder.Services.AddScoped<IShipmentQuotationRepository, ShipmentQuotationRepository>();
builder.Services.AddScoped<IShipmentService, ShipmentService>();
    
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(RegisterUserCommand).Assembly);// Đăng ký MediatR là nó sẽ quét toàn bộ Handler trong project Application
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
});
    builder.Services.AddValidatorsFromAssembly(typeof(RegisterUserCommand).Assembly);// Đăng ký FluentValidation

//builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(LoginCommand).Assembly));// Đăng ký MediatR là nó sẽ quét toàn bộ Handler trong project Application
//builder.Services.AddValidatorsFromAssembly(typeof(LoginCommand).Assembly);// Đăng ký FluentValidation

builder.Services.AddGrpcClient<LoyaltyService.LoyaltyServiceClient>(options =>
{
    options.Address = new Uri("https://localhost:7231");
});

builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.Configure<LalamoveOptions>(
    builder.Configuration.GetSection("Lalamove"));

builder.Services.AddHttpClient<ILalamoveClient, LalamoveClient>((sp, client) =>
{
    var options = sp.GetRequiredService<
        Microsoft.Extensions.Options.IOptions<LalamoveOptions>>().Value;

    client.BaseAddress = new Uri(options.BaseUrl);
});


Application.Common.MapsterConfig.Register();

var app = builder.Build();
app.UseValidationExceptionHandler();
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
app.MapOrderEndpoints();
app.MapCouponEndpoints();
app.MapReviewEndpoints();
app.MapVariantEndpoints();
app.MapPromotionEndpoints();
app.MapShippingEndpoints();
app.Run();
