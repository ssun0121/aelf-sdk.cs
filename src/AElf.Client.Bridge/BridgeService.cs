using AElf.Client.Core;
using AElf.Client.Core.Options;
using AElf.Contracts.Bridge;
using AElf.Types;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace AElf.Client.Bridge;

public class BridgeService : ContractServiceBase, IBridgeService, ITransientDependency
{
    private readonly IAElfClientService _clientService;
    private readonly AElfClientConfigOptions _clientConfigOptions;
    private readonly string _contractAddress;
    private readonly AElfContractOptions _contractOptions;
    
    public BridgeService(IAElfClientService clientService,
        IOptionsSnapshot<AElfClientConfigOptions> clientConfigOptions,
        IOptionsSnapshot<AElfContractOptions> contractOptions) : base(clientService, 
        Address.FromBase58(contractOptions.Value.BridgeContractAddress))
    {
        _clientService = clientService;
        _clientConfigOptions = clientConfigOptions.Value;
        _contractAddress = contractOptions.Value.BridgeContractAddress;
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

    public async Task<SendTransactionResult> CreateSwapAsync(CreateSwapInput createSwapInput)
    {
        var useClientAlias = _clientConfigOptions.ClientAlias;
        var tx = await PerformSendTransactionAsync("CreateSwap", createSwapInput, useClientAlias);
        return new SendTransactionResult
        {
            Transaction = tx,
            TransactionResult = await PerformGetTransactionResultAsync(tx.GetHash().ToHex(), useClientAlias)
        };
    }

    public async Task<SendTransactionResult> ChangeSwapRatioAsync(ChangeSwapRatioInput changeSwapRatioInput)
    {
        var useClientAlias = _clientConfigOptions.ClientAlias;
        var tx = await PerformSendTransactionAsync("ChangeSwapRatio", changeSwapRatioInput, useClientAlias);
        return new SendTransactionResult
        {
            Transaction = tx,
            TransactionResult = await PerformGetTransactionResultAsync(tx.GetHash().ToHex(), useClientAlias)
        };
    }

    public async Task<SendTransactionResult> DepositAsync(DepositInput depositInput)
    {
        var useClientAlias = _clientConfigOptions.ClientAlias;
        var tx = await PerformSendTransactionAsync("Deposit", depositInput, useClientAlias);
        return new SendTransactionResult
        {
            Transaction = tx,
            TransactionResult = await PerformGetTransactionResultAsync(tx.GetHash().ToHex(), useClientAlias)
        };
    }

    public async Task<SendTransactionResult> AddTokenAsync(AddTokenInput addTokenInput)
    {
        var useClientAlias = _clientConfigOptions.ClientAlias;
        var tx = await PerformSendTransactionAsync("AddToken", addTokenInput, useClientAlias);
        return new SendTransactionResult
        {
            Transaction = tx,
            TransactionResult = await PerformGetTransactionResultAsync(tx.GetHash().ToHex(), useClientAlias)
        };
    }

    public async Task<SendTransactionResult> CreateReceiptAsync(CreateReceiptInput createReceiptInput)
    {
        var useClientAlias = _clientConfigOptions.ClientAlias;
        var tx = await PerformSendTransactionAsync("CreateReceipt", createReceiptInput, useClientAlias);
        return new SendTransactionResult
        {
            Transaction = tx,
            TransactionResult = await PerformGetTransactionResultAsync(tx.GetHash().ToHex(), useClientAlias)
        };
    }

    public async Task<SendTransactionResult> SwapTokenAsync(SwapTokenInput swapTokenInput)
    {
        var useClientAlias = _clientConfigOptions.ClientAlias;
        var tx = await PerformSendTransactionAsync("SwapToken", swapTokenInput, useClientAlias);
        return new SendTransactionResult
        {
            Transaction = tx,
            TransactionResult = await PerformGetTransactionResultAsync(tx.GetHash().ToHex(), useClientAlias)
        };
    }

    public async Task<SendTransactionResult> SetGasLimitAsync(SetGasLimitInput input)
    {
        var useClientAlias = _clientConfigOptions.ClientAlias;
        var tx = await PerformSendTransactionAsync("SetGasLimit", input, useClientAlias);
        return new SendTransactionResult
        {
            Transaction = tx,
            TransactionResult = await PerformGetTransactionResultAsync(tx.GetHash().ToHex(), useClientAlias)
        };
    }

    public async Task<Hash> GetSpaceIdBySwapIdAsync(Hash input)
    {
        var useClientAlias = _clientConfigOptions.ClientAlias;
        var result = await _clientService.ViewAsync(_contractAddress, "GetSpaceIdBySwapId", input,useClientAlias);
        var actualResult = new Hash();
        actualResult.MergeFrom(result);
        return actualResult;
    }

    public async Task<SwapAmounts> GetSwapAmountsAsync(GetSwapAmountsInput input)
    {
        var useClientAlias = _clientConfigOptions.ClientAlias;
        var result = await _clientService.ViewAsync(_contractAddress, "GetSwapAmounts", input,useClientAlias);
        var actualResult = new SwapAmounts();
        actualResult.MergeFrom(result);
        return actualResult;
    }

    public async Task<ReceiptInfoList> GetSwappedReceiptInfoAsync(GetSwappedReceiptInfoListInput input)
    {
        var useClientAlias = _clientConfigOptions.ClientAlias;
        var result = await _clientService.ViewAsync(_contractAddress, "GetSwappedReceiptInfoList", input,useClientAlias);
        var actualResult = new ReceiptInfoList();
        actualResult.MergeFrom(result);
        return actualResult;
    }

    public async Task<Int64Value> GetFeeByChainIdAsync(StringValue input)
    {
        var useClientAlias = _clientConfigOptions.ClientAlias;
        var result = await _clientService.ViewAsync(_contractAddress, "GetFeeByChainId", input,useClientAlias);
        var actualResult = new Int64Value();
        actualResult.MergeFrom(result);
        return actualResult;
    }

    public async Task<SwapPairInfo> GetSwapPairInfoAsync(GetSwapPairInfoInput input)
    {
        var useClientAlias = _clientConfigOptions.ClientAlias;
        var result = await _clientService.ViewAsync(_contractAddress, "GetSwapPairInfo", input,useClientAlias);
        var actualResult = new SwapPairInfo();
        actualResult.MergeFrom(result);
        return actualResult;
    }

    public async Task<ReceiptInfo> GetReceiptInfoAsync(GetReceiptInfoInput input)
    {
        var useClientAlias = _clientConfigOptions.ClientAlias;
        var result = await _clientService.ViewAsync(_contractAddress, "GetReceiptInfo", input,useClientAlias);
        var actualResult = new ReceiptInfo();
        actualResult.MergeFrom(result);
        return actualResult;
    }
}