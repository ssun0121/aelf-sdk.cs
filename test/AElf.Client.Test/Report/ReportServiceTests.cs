using System.Linq;
using System.Threading.Tasks;
using AElf.Client.Bridge;
using AElf.Client.Report;
using AElf.Contracts.Report;
using AElf.Types;
using Google.Protobuf.WellKnownTypes;
using Shouldly;
using Xunit.Abstractions;

namespace AElf.Client.Test.Report;

[Trait("Category", "ReportContractService")]
public sealed class ReportServiceTests : AElfClientAbpContractServiceTestBase
{
    private readonly IReportService _reportService;
    private readonly ITestOutputHelper _output;

    public ReportServiceTests(ITestOutputHelper output)
    {
        _reportService = GetRequiredService<IReportService>();
        _output = output;
    }

    [Theory]
    [InlineData("xsnQafDAhNTeYcooptETqWnYBksFGGXxfcQyJJ5tmu6Ak9ZZt", 0, 0,
        "ZrAFaqdr79MWYkxA49Hp2LUdSVHdP6fJh3kDw4jmgC7HTgrni", "2nyC8hqq3pGnRu8gJzCsTaxXB6snfGxmL2viimKXgEfYWGtjEh")]
    public async Task InitializeContractTest(
        string oracleContractAddress,
        int reportFee, 
        int applyObserverFee, 
        string whitelistAddress, 
        string regimentContractAddress)
    {
        var result = await _reportService.InitializeContractAsync(new InitializeInput
        {
            OracleContractAddress = Address.FromBase58(oracleContractAddress),
            ReportFee = reportFee,
            ApplyObserverFee = applyObserverFee,
            RegimentContractAddress = Address.FromBase58(regimentContractAddress),
            InitialRegisterWhiteList = { Address.FromBase58(whitelistAddress) }
        });
        result.TransactionResult.Status.ShouldBe(TransactionResultStatus.Mined);
    }

    [Theory]
    [InlineData("0x2b8e182ac87d123237f8e28e4DDe8a2b050a1c96","ab6c0be63b89672561cfb5b574d5ed89df6729e48f6f709bb97517460c66e43c","Kovan")]
    public async Task RegisterOffChainAggregationTest(string token,string regimentId,string chainId)
    {
        var result = await _reportService.RegisterOffChainAggregation(new RegisterOffChainAggregationInput
        {
            Token = token,
            RegimentId = Hash.LoadFromHex(regimentId),
            ChainId = chainId
        });
        result.TransactionResult.Status.ShouldBe(TransactionResultStatus.Mined);
        var offChainAggregationRegistered = OffChainAggregationRegistered.Parser.ParseFrom(result.TransactionResult.Logs
            .First(i => i.Name == nameof(OffChainAggregationRegistered)).NonIndexed);
        offChainAggregationRegistered.Token.ShouldBe(token);
    }

    [Theory]
    [InlineData("Kovan","0x2b8e182ac87d123237f8e28e4DDe8a2b050a1c96","225ajURvev5rgX8HnMJ8GjbPnRxUrCHoD7HUjhWQqewEJ5GAv1")]
    public async Task SetSkipMemberListTest(string chainId,string token,string member)
    {
        var result = await _reportService.SetSkipMemberListAsync(new SetSkipMemberListInput
        {
            ChainId = chainId,
            Token = token,
            Value = new MemberList
            {
                Value = {Address.FromBase58(member)}
            }
        });
        result.TransactionResult.Status.ShouldBe(TransactionResultStatus.Mined);
    }

    [Theory]
    [InlineData("Kovan")]
    public async Task GetTokenByChainId(string chainId)
    {
        var result = await _reportService.GetTokenByChainIdAsync(new StringValue
        {
            Value = chainId
        });
        _output.WriteLine(result.Value);
    }
    
    [Theory]
    [InlineData("Kovan","0xf8F862Aaeb9cb101383d27044202aBbe3a057eCC",6)]
    public async Task GetReport(string chainId,string token,long roundId)
    {
        var result = await _reportService.GetReportAsync(new GetReportInput
        {
            ChainId = chainId,
            Token = token,
            RoundId = roundId
        });
        result.QueryId.ShouldBeNull();
        _output.WriteLine(result.Observations.Value.First().Key);
    }
    
}