using Microsoft.EntityFrameworkCore;
using PaymentsService.Application.Services;
using PaymentsService.Infrastructure.Data;
using PaymentsService.Api.BackgroundServices;
using SharedLibrary.Messaging;
using PaymentsService.Infrastructure.BackgroundServices;
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

builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IPaymentProcessor, PaymentProcessor>();

builder.Services.AddSingleton<IMessagePublisher>(sp =>
    new RabbitMqPublisher(builder.Configuration.GetConnectionString("RabbitMQ")
        ?? "amqp://guest:guest@rabbitmq:5672"));
builder.Services.AddSingleton<IMessageSubscriber>(sp =>
    new RabbitMqSubscriber(builder.Configuration.GetConnectionString("RabbitMQ")
        ?? "amqp://guest:guest@rabbitmq:5672"));

builder.Services.AddScoped<OutboxPublisher>();
builder.Services.AddHostedService<OutboxPublisherWorker>(); 
builder.Services.AddHostedService<PaymentCommandSubscriber>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();
