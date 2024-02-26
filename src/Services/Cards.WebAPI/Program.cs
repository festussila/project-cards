using Microsoft.AspNetCore.Mvc;

using static Cards.WebAPI.Models.Configuration.Startup;

[assembly: ApiController]

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.

ConfigureServices(builder.Services, builder.Configuration);

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
ConfigureApp(app);

// Migrate and seed database
ConfigureDatabase(app);

app.Run();
