using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using UrlSave.Contexts;
using UrlSave.Extensions;
using UrlSave.Jobs;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Down Price Market API",
        Description = "An ASP.NET Core Web API for managing down price market",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Dayan Baumuratov and Zhassulan Kazhkenov",
            Url = new Uri("https://example.com/contact")
        },
        License = new OpenApiLicense
        {
            Name = "MIT",
            Url = new Uri("https://example.com/license")
        }
    });
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

string connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddHangfireInfrastructure(connection);
builder.Services.AddApiVersioningExtension();

builder.Services.AddDbContext<LinkContext>(options => options.UseSqlServer(connection));
var app = builder.Build();

app.UseHangfireDashboard();
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});
RecurringJob.AddOrUpdate<ParceKaspiJob>("parcer", x => x.Execute(), Cron.Minutely);

app.MapControllers();
app.Run();

