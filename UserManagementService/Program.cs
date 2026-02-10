using UserManagementService.Data;
using Microsoft.EntityFrameworkCore;
using UserManagementService.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// DB Context
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Inyección del Servicio
builder.Services.AddScoped<IUserService, UserService>();


// evitar errores de ciclos en los modelos relacionados
builder.Services.AddControllers()
    .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();