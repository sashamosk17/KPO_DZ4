using Microsoft.EntityFrameworkCore;
using OrdersService.Infrastructure.Data;
using OrdersService.Application.Services;
using OrdersService.Api.BackgroundServices;
using SharedLibrary.Messaging;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddDbContext<PaymentsDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PaymentsDatabase")));


builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddSingleton<IMessagePublisher>(sp =>
    new RabbitMqPublisher(builder.Configuration.GetConnectionString("RabbitMQ")
        ?? "amqp://guest:guest@rabbitmq:5672"));

builder.Services.AddSingleton<IMessageSubscriber>(sp =>
    new RabbitMqSubscriber(builder.Configuration.GetConnectionString("RabbitMQ")
        ?? "amqp://guest:guest@rabbitmq:5672"));

builder.Services.AddScoped<OutboxPublisher>();
builder.Services.AddHostedService<OutboxPublisherWorker>();
builder.Services.AddHostedService<PaymentResultSubscriber>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
    db.Database.Migrate();
}

app.Run();
