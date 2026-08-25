using AssetManagement.Business.Services;
using AssetManagement.DataAccess.Context;
using AssetManagement.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// DbContext
builder.Services.AddDbContext<AssetDbContext>(options =>
		options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Dependency Injection
builder.Services.AddScoped<IAssetRepository, AssetRepository>();
builder.Services.AddScoped<IAssetService, AssetService>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
	app.MapOpenApi();
	// Scalar UI Yapýlandýrmasý
	app.MapScalarApiReference(options =>
	{
		options.WithTitle("Asset Management API")
					 .WithTheme(ScalarTheme.Purple)
					 .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
	});
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
