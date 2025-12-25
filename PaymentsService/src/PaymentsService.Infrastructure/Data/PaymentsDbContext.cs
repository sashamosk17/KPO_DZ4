using Microsoft.EntityFrameworkCore;
using PaymentsService.Domain.Entities;

namespace PaymentsService.Infrastructure.Data;

/// <summary>
/// Контекст БД платёжного сервиса.
/// Хранит счета пользователей и сущности для Transactional Inbox/Outbox.
/// </summary>
public class PaymentsDbContext : DbContext
{
    public PaymentsDbContext(DbContextOptions<PaymentsDbContext> options)
        : base(options)
    {
    }

    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<InboxEvent> InboxEvents => Set<InboxEvent>();
    public DbSet<OutboxEvent> OutboxEvents => Set<OutboxEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Account>(builder =>
        {
            builder.HasKey(a => a.Id);
            builder.HasIndex(a => a.UserId).IsUnique();
        });

        modelBuilder.Entity<InboxEvent>(builder =>
        {
            builder.HasKey(e => e.Id);
            builder.HasIndex(e => e.MessageId).IsUnique();
            builder.Property(e => e.MessageId).IsRequired();
            builder.Property(e => e.EventType).IsRequired();
            builder.Property(e => e.Payload).IsRequired();
        });

        modelBuilder.Entity<OutboxEvent>(builder =>
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.EventType).IsRequired();
            builder.Property(e => e.Payload).IsRequired();
        });
    }
}
