using ClinicApi.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(option =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultSQLConnection");
    if (!builder.Environment.IsDevelopment())
    {
        var password = Environment.GetEnvironmentVariable("MSSQL_SA_PASSWORD");
        connectionString = string.Format(connectionString, password);
    }
    option.UseSqlServer(connectionString);

});
builder.Services.AddControllers().AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var basePath = AppContext.BaseDirectory;
    var xmlPath = Path.Combine(basePath, "ClinicApi.xml");
    options.IncludeXmlComments(xmlPath);
});

builder.Configuration.AddJsonFile("config.json",
    optional: true,
    reloadOnChange: true);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

app.Run();
