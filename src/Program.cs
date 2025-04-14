using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Mapster;
using MapsterMapper;
using Microsoft.OpenApi.Models;
using ProductmanagementCore.Common;
using ProductmanagementCore.Models;
using ProductmanagementCore.Models.Dto;
using ProductmanagementCore.Repository;
using ProductmanagementCore.Services;

var builder = WebApplication.CreateBuilder(args);

// ใช้ Autofac เป็น DI Container
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

// Config settings
var configuration = builder.Configuration;

// 👇 Add services
builder.Services.AddControllers();

// Mapster config
TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ProductManagement", Version = "v1" });
});

// 👇 Register service/repo ด้วย Autofac
builder.Host.ConfigureContainer<ContainerBuilder>(container =>
{
    var serviceAssembly = typeof(UserService).Assembly;
    container.RegisterAssemblyTypes(serviceAssembly)
        .Where(t => t.Name.EndsWith("Service"))
        .AsImplementedInterfaces()
        .SingleInstance();

    var repositoryAssembly = typeof(UserRepository).Assembly;
    container.RegisterAssemblyTypes(repositoryAssembly)
        .Where(t => t.Name.EndsWith("Repository"))
        .AsImplementedInterfaces()
        .SingleInstance();
});

var app = builder.Build();

// 👇 Configure HTTP pipeline
if (!app.Environment.IsProduction())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProductManagement");
        c.RoutePrefix = string.Empty;
    });
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseRouting();
app.UseAuthorization();

app.MapControllers();

app.Run();
