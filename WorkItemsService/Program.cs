using WorkItemsService.Data;
using WorkItemsService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//  DB Context
builder.Services.AddDbContext<WorkItemsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// HttpClient comunicación con UserManagementService
builder.Services.AddHttpClient("UserClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:5231/api/Users");
});

// Inyección del Servicio
builder.Services.AddScoped<IWorkItemService, WorkItemService>();

builder.Services.AddControllers();
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