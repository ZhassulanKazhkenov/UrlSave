using Microsoft.EntityFrameworkCore;
using UrlSave;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
string connection = builder.Configuration.GetConnectionString("DefaultConnection");

// добавляем контекст ApplicationContext в качестве сервиса в приложение
builder.Services.AddDbContext<LinkContext>(options => options.UseSqlServer(connection));
var app = builder.Build();
app.MapControllers();
app.MapGet("/", () => "Hello World!");

app.Run();

