using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Shared.DTOs;
using Shared.Interfaces;
using System.Fabric;

namespace BankService
{
    internal sealed class BankService : StatefulService, IBankService
    {
        public BankService(StatefulServiceContext context)
            : base(context) { }

        public async Task<UserAccountDto> GetUserAccountAsync(string userId)
        {
            var accounts = await StateManager.GetOrAddAsync<IReliableDictionary<string, UserAccountDto>>("accounts");

            using var tx = StateManager.CreateTransaction();
            var result = await accounts.TryGetValueAsync(tx, userId);
            return result.HasValue ? result.Value : null;
        }

        public async Task<bool> CheckBalanceAsync(string userId, decimal amount)
        {
            var account = await GetUserAccountAsync(userId);
            return account != null && account.Balance >= amount;
        }

        public async Task<bool> DeductBalanceAsync(string userId, decimal amount)
        {
            var accounts = await StateManager.GetOrAddAsync<IReliableDictionary<string, UserAccountDto>>("accounts");

            using var tx = StateManager.CreateTransaction();
            var result = await accounts.TryGetValueAsync(tx, userId);

            if (!result.HasValue || result.Value.Balance < amount)
                return false;

            var account = result.Value;
            account.Balance -= amount;
            await accounts.SetAsync(tx, userId, account);
            await tx.CommitAsync();
            return true;
        }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return this.CreateServiceRemotingReplicaListeners();
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            var accounts = await StateManager.GetOrAddAsync<IReliableDictionary<string, UserAccountDto>>("accounts");

            using var tx = StateManager.CreateTransaction();
            await accounts.TryAddAsync(tx, "user1", new UserAccountDto { UserId = "user1", Name = "John Doe", Email = "john@example.com", Balance = 500.00m });
            await accounts.TryAddAsync(tx, "user2", new UserAccountDto { UserId = "user2", Name = "Jane Doe", Email = "jane@example.com", Balance = 200.00m });
            await tx.CommitAsync();
        }
    }
}