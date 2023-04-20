using ShippingApp.Data;
using Microsoft.EntityFrameworkCore;
using ShippingApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddCors(options => options.AddPolicy(name: "CorsPolicy",
    policy =>
    {
        policy.WithOrigins().AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin();
    }
    ));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<shipmentAppDatabase>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionString")));
builder.Services.AddScoped<IShipmentService, ShipmentService>();
builder.Services.AddScoped<ICheckpointServices, CheckpointServices>();
builder.Services.AddScoped<IShortestRoute, ShortestRoute>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IApiCallingService, ApiCallingService>();
builder.Services.AddScoped<IMessageQueueService, MessageQueueService>();
builder.Services.AddScoped<IMessageProducerService, MessageProducerService>();
builder.Services.AddHostedService<BackgroundTaskService>();
var app = builder.Build();

// Configure the HTTP request pipelin
app.UseCors("CorsPolicy");
app.UseSwagger();

app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();