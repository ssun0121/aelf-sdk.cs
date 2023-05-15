using AElf.Client.Core;
using AElf.Client.Core.Options;
using AElf.Contracts.Report;
using AElf.Types;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace AElf.Client.Report;

public class ReportService : ContractServiceBase, IReportService, ITransientDependency
{
    private readonly IAElfClientService _clientService;
    private readonly AElfClientConfigOptions _clientConfigOptions;
    private readonly string _contractAddress;
    private readonly AElfContractOptions _contractOptions;
    
    public ReportService(IAElfClientService clientService,
        IOptionsSnapshot<AElfClientConfigOptions> clientConfigOptions,
        IOptionsSnapshot<AElfContractOptions> contractOptions) : base(clientService, 
        Address.FromBase58(contractOptions.Value.ReportContractAddress))
    {
        _clientService = clientService;
        _clientConfigOptions = clientConfigOptions.Value;
        _contractAddress = contractOptions.Value.ReportContractAddress;
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

    public async Task<SendTransactionResult> RegisterOffChainAggregation(RegisterOffChainAggregationInput input)
    {
        var useClientAlias = _clientConfigOptions.ClientAlias;
        var tx = await PerformSendTransactionAsync("RegisterOffChainAggregation", input, useClientAlias);
        return new SendTransactionResult
        {
            Transaction = tx,
            TransactionResult = await PerformGetTransactionResultAsync(tx.GetHash().ToHex(), useClientAlias)
        };
    }

    public async Task<SendTransactionResult> SetSkipMemberListAsync(SetSkipMemberListInput input)
    {
        var useClientAlias = _clientConfigOptions.ClientAlias;
        var tx = await PerformSendTransactionAsync("SetSkipMemberList", input, useClientAlias);
        return new SendTransactionResult
        {
            Transaction = tx,
            TransactionResult = await PerformGetTransactionResultAsync(tx.GetHash().ToHex(), useClientAlias)
        };
    }

    public async Task<StringValue> GetTokenByChainIdAsync(StringValue input)
    {
        var useClientAlias = _clientConfigOptions.ClientAlias;
        var contractAddress = _contractOptions.ReportContractAddress;
        var result = await _clientService.ViewAsync(contractAddress, "GetTokenByChainId", input,useClientAlias);
        var actualResult = new StringValue();
        actualResult.MergeFrom(result);
        return actualResult;
    }

    public async Task<Contracts.Report.Report> GetReportAsync(GetReportInput input)
    {
        var useClientAlias = _clientConfigOptions.ClientAlias;
        var contractAddress = _contractOptions.ReportContractAddress;
        var result = await _clientService.ViewAsync(contractAddress, "GetReport", input,useClientAlias);
        var actualResult = new Contracts.Report.Report();
        actualResult.MergeFrom(result);
        return actualResult;
    }
}