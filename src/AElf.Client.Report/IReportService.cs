using AElf.Client.Core;
using AElf.Contracts.Report;
using Google.Protobuf.WellKnownTypes;

namespace AElf.Client.Report;

public interface IReportService
{
    Task<SendTransactionResult> InitializeContractAsync(InitializeInput initializeInput);

    Task<SendTransactionResult> RegisterOffChainAggregation(RegisterOffChainAggregationInput input);

    Task<SendTransactionResult> SetSkipMemberListAsync(SetSkipMemberListInput input);

    Task<StringValue> GetTokenByChainIdAsync(StringValue input);

    Task<Contracts.Report.Report> GetReportAsync(GetReportInput input);
    
    

}