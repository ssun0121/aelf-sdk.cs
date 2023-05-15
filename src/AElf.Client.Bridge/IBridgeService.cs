using AElf.Client.Core;
using AElf.Contracts.Bridge;
using AElf.Types;
using Google.Protobuf.WellKnownTypes;

namespace AElf.Client.Bridge;

public interface IBridgeService
{
    Task<SendTransactionResult> InitializeContractAsync(InitializeInput initializeInput);

    Task<SendTransactionResult> CreateSwapAsync(CreateSwapInput createSwapInput);

    Task<SendTransactionResult> ChangeSwapRatioAsync(ChangeSwapRatioInput changeSwapRatioInput);

    Task<SendTransactionResult> DepositAsync(DepositInput depositInput);

    Task<SendTransactionResult> AddTokenAsync(AddTokenInput addTokenInput);

    Task<SendTransactionResult> CreateReceiptAsync(CreateReceiptInput createReceiptInput);

    Task<SendTransactionResult> SwapTokenAsync(SwapTokenInput swapTokenInput);

    Task<SendTransactionResult> SetGasLimitAsync(SetGasLimitInput input);

    Task<Hash> GetSpaceIdBySwapIdAsync(Hash input);

    Task<SwapAmounts> GetSwapAmountsAsync(GetSwapAmountsInput input);

    Task<ReceiptInfoList> GetSwappedReceiptInfoAsync(GetSwappedReceiptInfoListInput input);

    Task<Int64Value> GetFeeByChainIdAsync(StringValue input);

    Task<SwapPairInfo> GetSwapPairInfoAsync(GetSwapPairInfoInput input);
    

}