using AElf.Client.Core;
using AElf.Client.Core.Options;
using AElf.Contracts.MerkleTreeContract;
using AElf.Types;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace AElf.Client.MerkleTree;

public class MerkleTreeService : ContractServiceBase, IMerkleTreeService, ITransientDependency
{
    private readonly IAElfClientService _clientService;
    private readonly AElfClientConfigOptions _clientConfigOptions;
    private readonly AElfContractOptions _contractOptions;
    private readonly string _contractAddress;

    public MerkleTreeService(IAElfClientService clientService,
        IOptionsSnapshot<AElfClientConfigOptions> clientConfigOptions,
        IOptionsSnapshot<AElfContractOptions> contractOptions) : base(clientService,
        Address.FromBase58(contractOptions.Value.MerkleTreeContractAddress))
    {
        _clientService = clientService;
        _clientConfigOptions = clientConfigOptions.Value;
        _contractAddress = contractOptions.Value.MerkleTreeContractAddress;
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

    public async Task<long> GetLastLeafIndexAsync(GetLastLeafIndexInput input)
    {
        var useClientAlias = _clientConfigOptions.ClientAlias;
        var contractAddress = _contractOptions.MerkleTreeContractAddress;
        var result = await _clientService.ViewAsync(contractAddress, "GetLastLeafIndex", input,useClientAlias);
        var actualResult = new Int64Value();
        actualResult.MergeFrom(result);
        return actualResult.Value;
    }
}