using AElf.Client.Core;
using AElf.Client.Core.Options;
using AElf.Contracts.Regiment;
using AElf.Types;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace AElf.Client.Regiment;

public class RegimentService : ContractServiceBase, IRegimentService, ITransientDependency
{
    private readonly IAElfClientService _clientService;
    private readonly AElfClientConfigOptions _clientConfigOptions;
    private readonly string _contractAddress;
    private readonly AElfContractOptions _contractOptions;

    public RegimentService(IAElfClientService clientService,
        IOptionsSnapshot<AElfClientConfigOptions> clientConfigOptions,
        IOptionsSnapshot<AElfContractOptions> contractOptions) : base(clientService,
        Address.FromBase58(contractOptions.Value.RegimentContractAddress))
    {
        _clientService = clientService;
        _clientConfigOptions = clientConfigOptions.Value;
        _contractAddress = contractOptions.Value.RegimentContractAddress;
        _contractOptions = contractOptions.Value;
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

    public async Task<SendTransactionResult> CreateRegimentAsync(CreateRegimentInput createRegimentInput)
    {
        var useClientAlias = _clientConfigOptions.ClientAlias;
        var tx = await PerformSendTransactionAsync("CreateRegiment", createRegimentInput, useClientAlias);
        return new SendTransactionResult
        {
            Transaction = tx,
            TransactionResult = await PerformGetTransactionResultAsync(tx.GetHash().ToHex(), useClientAlias)
        };
    }

    public async Task<SendTransactionResult> AddAdminAsync(AddAdminsInput addAdminsInput)
    {
        var useClientAlias = _clientConfigOptions.ClientAlias;
        var tx = await PerformSendTransactionAsync("AddAdmins", addAdminsInput, useClientAlias);
        return new SendTransactionResult
        {
            Transaction = tx,
            TransactionResult = await PerformGetTransactionResultAsync(tx.GetHash().ToHex(), useClientAlias)
        };
    }

    public async Task<SendTransactionResult> DeleteAdminAsync(DeleteAdminsInput deleteAdminsInput)
    {
        var useClientAlias = _clientConfigOptions.ClientAlias;
        var tx = await PerformSendTransactionAsync("AddAdmins", deleteAdminsInput, useClientAlias);
        return new SendTransactionResult
        {
            Transaction = tx,
            TransactionResult = await PerformGetTransactionResultAsync(tx.GetHash().ToHex(), useClientAlias)
        };
    }

    public async Task<RegimentInfo> GetRegimentInfoAsync(Address input)
    {
        var useClientAlias = _clientConfigOptions.ClientAlias;
        var contractAddress = _contractOptions.RegimentContractAddress;
        var result = await _clientService.ViewAsync(contractAddress, "GetRegimentInfo", input,useClientAlias);
        var actualResult = new RegimentInfo();
        actualResult.MergeFrom(result);
        return actualResult;
    }

    public async Task<RegimentMemberList> GetRegimentMemberListAsync(Address input)
    {
        var useClientAlias = _clientConfigOptions.ClientAlias;
        var contractAddress = _contractOptions.RegimentContractAddress;
        var result = await _clientService.ViewAsync(contractAddress, "GetRegimentMemberList", input,useClientAlias);
        var actualResult = new RegimentMemberList();
        actualResult.MergeFrom(result);
        return actualResult;
    }

    public async Task<Address> GetControllerAsync()
    {
        var useClientAlias = _clientConfigOptions.ClientAlias;
        var contractAddress = _contractOptions.RegimentContractAddress;
        var result = await _clientService.ViewAsync(contractAddress, "GetController", new Empty() ,useClientAlias);
        var actualResult = new Address();
        actualResult.MergeFrom(result);
        return actualResult;
    }
}