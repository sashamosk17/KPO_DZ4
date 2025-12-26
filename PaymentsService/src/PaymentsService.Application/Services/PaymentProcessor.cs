using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PaymentsService.Infrastructure.Data;
using PaymentsService.Domain.Entities;
using SharedLibrary.Messaging;

namespace PaymentsService.Application.Services;
/// <summary>
/// Обрабатывает команду оплаты:
/// идемпотентность через Inbox + списание + запись результата в Outbox.
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
        {
            return;
        }

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

        await using var tx = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        var account = await _dbContext.Accounts
            .FirstOrDefaultAsync(a => a.UserId == userId, cancellationToken);

        var isSuccess = account is not null && account.Balance >= amount;

        if (isSuccess)
        {
            account!.Balance -= amount;
        }

        var paymentResult = new PaymentProcessedMessage
        {
            OrderId = orderId,
            Success = isSuccess,
            ErrorMessage = isSuccess ? null : "Insufficient balance"
        };

        var outboxEvent = new OutboxEvent
        {
            Id = Guid.NewGuid(),
            EventType = "PaymentProcessed",
            Payload = JsonSerializer.Serialize(paymentResult)
        };

        existingInbox.IsProcessed = true;

        await _dbContext.OutboxEvents.AddAsync(outboxEvent, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await tx.CommitAsync(cancellationToken);
    }
}
