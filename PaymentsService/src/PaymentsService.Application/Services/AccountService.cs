using Microsoft.EntityFrameworkCore;
using PaymentsService.Infrastructure.Data;
using PaymentsService.Domain.Entities;
using PaymentsService.Domain.Models;
using PaymentsService.Application.Services;

namespace PaymentsService.Application.Services;

/// <summary>
/// Реализация сервиса управления счетами пользователей.
/// </summary>
public class AccountService : IAccountService
{
    private readonly PaymentsDbContext _dbContext;

    public AccountService(PaymentsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AccountResponse> CreateAccountAsync(
        CreateAccountRequest request,
        CancellationToken cancellationToken = default)
    {
        var existing = await _dbContext.Accounts
            .FirstOrDefaultAsync(a => a.UserId == request.UserId, cancellationToken);

        if (existing is not null)
        {
            return MapToResponse(existing);
        }

        var account = new Account
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Balance = request.InitialBalance
        };

        await _dbContext.Accounts.AddAsync(account, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapToResponse(account);
    }

    public async Task<AccountResponse?> GetAccountAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var account = await _dbContext.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.UserId == userId, cancellationToken);

        return account is null ? null : MapToResponse(account);
    }

    public async Task<AccountResponse?> TopupAsync(
        TopupAccountRequest request,
        CancellationToken cancellationToken = default)
    {
        var account = await _dbContext.Accounts
            .FirstOrDefaultAsync(a => a.UserId == request.UserId, cancellationToken);

        if (account is null)
        {
            return null;
        }

        account.Balance += request.Amount;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapToResponse(account);
    }

    private static AccountResponse MapToResponse(Account account)
    {
        return new AccountResponse
        {
            Id = account.Id,
            UserId = account.UserId,
            Balance = account.Balance
        };
    }
}
