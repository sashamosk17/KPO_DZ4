using PaymentsService.Domain.Models;

namespace PaymentsService.Application.Services;

/// <summary>
/// Сервис управления счетами пользователей:
/// создание счёта, пополнение и получение информации о счёте.
/// </summary>
public interface IAccountService
{
    Task<AccountResponse> CreateAccountAsync(CreateAccountRequest request, CancellationToken cancellationToken = default);
    Task<AccountResponse?> GetAccountAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<AccountResponse?> TopupAsync(TopupAccountRequest request, CancellationToken cancellationToken = default);
}
