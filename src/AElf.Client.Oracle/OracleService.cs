using AElf.Client.Core;
using AElf.Client.Core.Options;
using AElf.Contracts.Oracle;
using AElf.Types;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace AElf.Client.Oracle;

public class OracleService : ContractServiceBase, IOracleService, ITransientDependency
{
    private readonly IAElfClientService _clientService;
    private readonly AElfClientConfigOptions _clientConfigOptions;
    private readonly string _contractAddress;

    public OracleService(IAElfClientService clientService,
        IOptionsSnapshot<AElfClientConfigOptions> clientConfigOptions,
        IOptionsSnapshot<AElfContractOptions> contractOptions) : base(clientService,
        Address.FromBase58(contractOptions.Value.OracleContractAddress))
    {
        _clientService = clientService;
        _clientConfigOptions = clientConfigOptions.Value;
        _contractAddress = contractOptions.Value.OracleContractAddress;
    }

    public async Task<SendTransactionResult> InitializeContractAsync(InitializeInput initializeInput)
    {
        var useClientAlias = _clientConfigOptions.ClientAlias;
        var tx = await PerformSendTransactionAsync("Initialize", initializeInput, useClientAlias);
        return new SendTransactionResult
        {
            Transaction = tx,
            TransactionResult = await PerformGetTransactionResultAsync(tx.GetHash().ToHex(), useClientAlias)
        };
    }

    public async Task<SendTransactionResult> CreateRegimentAsync(CreateRegimentInput input)
    {
        var useClientAlias = _clientConfigOptions.ClientAlias;
        var tx = await PerformSendTransactionAsync("CreateRegiment", input, useClientAlias);
        return new SendTransactionResult
        {
            Transaction = tx,
            TransactionResult = await PerformGetTransactionResultAsync(tx.GetHash().ToHex(), useClientAlias)
        };
    }

    public async Task<SendTransactionResult> AddAdminsAsync(AddAdminsInput input)
    {
        var useClientAlias = _clientConfigOptions.ClientAlias;
        var tx = await PerformSendTransactionAsync("AddAdmins", input, useClientAlias);
        return new SendTransactionResult
        {
            Transaction = tx,
            TransactionResult = await PerformGetTransactionResultAsync(tx.GetHash().ToHex(), useClientAlias)
        };
    }

    public async Task<SendTransactionResult> DeleteAdminAsync(DeleteAdminsInput deleteAdminsInput)
    {
        var useClientAlias = _clientConfigOptions.ClientAlias;
        var tx = await PerformSendTransactionAsync("DeleteAdmins", deleteAdminsInput, useClientAlias);
        return new SendTransactionResult
        {
            Transaction = tx,
            TransactionResult = await PerformGetTransactionResultAsync(tx.GetHash().ToHex(), useClientAlias)
        };
    }

    public async Task<SendTransactionResult> DeleteMemberListAsync(DeleteRegimentMemberInput input)
    {
        var useClientAlias = _clientConfigOptions.ClientAlias;
        var tx = await PerformSendTransactionAsync("DeleteRegimentMember", input, useClientAlias);
        return new SendTransactionResult
        {
            Transaction = tx,
            TransactionResult = await PerformGetTransactionResultAsync(tx.GetHash().ToHex(), useClientAlias)
        };
    }
}