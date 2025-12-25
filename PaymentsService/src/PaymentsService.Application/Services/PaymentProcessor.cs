using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PaymentsService.Infrastructure.Data;
using PaymentsService.Domain.Entities;
using SharedLibrary.Messages;


namespace PaymentsService.Application.Services;

/// <summary>
/// Обрабатывает команду оплаты заказа:
/// проверяет, не обрабатывали ли сообщение раньше (Inbox),
/// списывает деньги и пишет результат в Outbox.
/// </summary>
public class PaymentProcessor : IPaymentProcessor
{
    private readonly PaymentsDbContext _dbContext;

    public PaymentProcessor(PaymentsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task ProcessPaymentAsync(
        string messageId,
        Guid userId,
        Guid orderId,
        decimal amount,
        CancellationToken cancellationToken = default)
    {
        var existingInbox = await _dbContext.InboxEvents
            .FirstOrDefaultAsync(e => e.MessageId == messageId, cancellationToken);

        if (existingInbox is not null && existingInbox.IsProcessed)
            return;

        if (existingInbox is null)
        {
            existingInbox = new InboxEvent
            {
                Id = Guid.NewGuid(),
                MessageId = messageId,
                EventType = "PaymentRequested",
                Payload = string.Empty,
                IsProcessed = false
            };

            await _dbContext.InboxEvents.AddAsync(existingInbox, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        await using var transaction = await _dbContext.Database
            .BeginTransactionAsync(cancellationToken);

        var account = await _dbContext.Accounts
            .FirstOrDefaultAsync(a => a.UserId == userId, cancellationToken);

        var success = false;
        string? error = null;

        if (account is null)
        {
            success = false;
            error = "Account not found";
        }
        else if (account.Balance < amount)
        {
            success = false;
            error = "Insufficient funds";
        }
        else
        {
            account.Balance -= amount;
            success = true;
        }

        var resultMessage = new PaymentProcessedMessage
        {
            OrderId = orderId,
            Success = success,
            ErrorMessage = error
        };

        var outboxEvent = new OutboxEvent
        {
            Id = Guid.NewGuid(),
            EventType = "PaymentProcessed",
            Payload = JsonSerializer.Serialize(resultMessage),
            IsPublished = false,
            CreatedAtUtc = DateTime.UtcNow
        };
        existingInbox.IsProcessed = true;

        await _dbContext.OutboxEvents.AddAsync(outboxEvent, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }
}
