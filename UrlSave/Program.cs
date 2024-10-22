using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using UrlSave.Application.Jobs;
using UrlSave.Application.Services;
using UrlSave.DataAccess.Extensions;
using UrlSave.Application.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddScoped<ProductService>();


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
builder.Services.AddLinkDbContext(connection);

builder.Services.AddApiVersioning(config =>
{
    config.DefaultApiVersion = new ApiVersion(1, 0);
    config.AssumeDefaultVersionWhenUnspecified = true;
    config.ReportApiVersions = true;
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

var app = builder.Build();

app.UseHangfireDashboard();
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});


RecurringJob.AddOrUpdate<ParceKaspiJob>("parcer", x => x.ParseKaspiLinks(), "*/2 * * * *");
RecurringJob.AddOrUpdate<NotificationPushJob>("notification", x => x.NotifyPriceChanges(), "*/2 * * * *");
RecurringJob.AddOrUpdate<SendMailJob>("sendMail", x => x.SendPendingNotifications(), "*/1 * * * *");


app.MapControllers();
app.Run();

