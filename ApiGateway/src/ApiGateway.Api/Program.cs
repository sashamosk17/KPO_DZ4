var builder = WebApplication.CreateBuilder(args);

var ordersUrl = builder.Configuration["Services:OrdersUrl"]
    ?? throw new InvalidOperationException("OrdersUrl not configured");

var paymentsUrl = builder.Configuration["Services:PaymentsUrl"]
    ?? throw new InvalidOperationException("PaymentsUrl not configured");
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient("OrdersService", client =>
{
    client.BaseAddress = new Uri(ordersUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient("PaymentsService", client =>
{
    client.BaseAddress = new Uri(paymentsUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors();
app.UseAuthorization();
app.MapControllers();

app.Run();
