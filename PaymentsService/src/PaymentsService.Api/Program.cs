using Microsoft.EntityFrameworkCore;
using PaymentsService.Infrastructure.Data;
using PaymentsService.Application.Services;
using SharedLibrary.Messaging;
using Microsoft.OpenApi.Models;


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
{
    var connectionString = builder.Configuration.GetConnectionString("PaymentsDatabase");
    options.UseNpgsql(connectionString);
});

builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IPaymentProcessor, PaymentProcessor>();

builder.Services.AddSingleton<IMessagePublisher>(sp =>
    new RabbitMqPublisher(builder.Configuration.GetConnectionString("RabbitMQ") ?? "amqp://guest:guest@localhost:5672"));

builder.Services.AddSingleton<IMessageSubscriber>(sp =>
    new RabbitMqSubscriber(builder.Configuration.GetConnectionString("RabbitMQ") ?? "amqp://guest:guest@localhost:5672"));

builder.Services.AddHostedService<PaymentsService.Api.BackgroundServices.PaymentCommandSubscriber>();

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
    var db = scope.ServiceProvider.GetRequiredService<PaymentsDbContext>();
    db.Database.Migrate();
}

app.Run();
