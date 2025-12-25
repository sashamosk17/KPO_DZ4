using Microsoft.EntityFrameworkCore;
using OrdersService.Domain.Entities;

namespace OrdersService.Infrastructure.Data;

/// <summary>
/// Контекст БД заказов.
/// Содержит DbSet для заказов и outbox-событий.
/// Используется EF Core для работы с PostgreSQL.
/// </summary>
public class OrdersDbContext : DbContext
{
	public OrdersDbContext(DbContextOptions<OrdersDbContext> options)
		: base(options)
	{
	}

	public DbSet<Order> Orders => Set<Order>();
	public DbSet<OutboxEvent> OutboxEvents => Set<OutboxEvent>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<Order>(builder =>
		{
			builder.HasKey(o => o.Id);
			builder.Property(o => o.Description).HasMaxLength(500).IsRequired();
		});

		modelBuilder.Entity<OutboxEvent>(builder =>
		{
			builder.HasKey(e => e.Id);
			builder.Property(e => e.EventType).HasMaxLength(200).IsRequired();
			builder.Property(e => e.Payload).IsRequired();
		});
	}
}
