using System.Linq;
using System.Threading.Tasks;
using AElf.Client.Bridge;
using AElf.Client.Report;
using AElf.Client.Test;
using AElf.Client.Token;
using AElf.Contracts.Bridge;
using AElf.Contracts.Report;
using AElf.Types;
using Google.Protobuf.WellKnownTypes;
using Shouldly;
using Xunit.Abstractions;
using InitializeInput = AElf.Contracts.Bridge.InitializeInput;

namespace AElf.Client.Test.Bridge;

[Trait("Category", "BridgeContractService")]
public sealed class BridgeServiceTests : AElfClientAbpContractServiceTestBase
{
    private readonly IBridgeService _bridgeService;
    private readonly IReportService _reportService;
    private readonly ITestOutputHelper _output;
    private readonly ITokenService _tokenService;

    public BridgeServiceTests(ITestOutputHelper output)
    {
        _bridgeService = GetRequiredService<IBridgeService>();
        _tokenService = GetRequiredService<ITokenService>();
        _reportService = GetRequiredService<IReportService>();
        _output = output;
    }

    [Theory]
    [InlineData(
        "xsnQafDAhNTeYcooptETqWnYBksFGGXxfcQyJJ5tmu6Ak9ZZt",
        "SsSqZWLf7Dk9NWyWyvDwuuY5nzn5n99jiscKZgRPaajZP5p8y",
        "2nyC8hqq3pGnRu8gJzCsTaxXB6snfGxmL2viimKXgEfYWGtjEh",
        "GwsSp1MZPmkMvXdbfSCDydHhZtDpvqkFpmPvStYho288fb7QZ",
        "ZrAFaqdr79MWYkxA49Hp2LUdSVHdP6fJh3kDw4jmgC7HTgrni",
        "ZrAFaqdr79MWYkxA49Hp2LUdSVHdP6fJh3kDw4jmgC7HTgrni"
    )]
    public async Task InitializeContractTest(
        string oracleAddress,
        string merkleTreeAddress,
        string regimentAddress,
        string reportAddress,
        string admin,
        string controller)
    {
        var result = await _bridgeService.InitializeContractAsync(new InitializeInput
        {
            OracleContractAddress = Address.FromBase58(oracleAddress),
            MerkleTreeContractAddress = Address.FromBase58(merkleTreeAddress),
            RegimentContractAddress = Address.FromBase58(regimentAddress),
            ReportContractAddress = Address.FromBase58(reportAddress),
            Admin = Address.FromBase58(admin),
            Controller = Address.FromBase58(controller)
        });
        result.TransactionResult.Status.ShouldBe(TransactionResultStatus.Mined);
    }


    /**
    * To AElf
    */
    [Theory]
    [InlineData("ab6c0be63b89672561cfb5b574d5ed89df6729e48f6f709bb97517460c66e43c", "Kovan", "USDT", 10000000000, 1)]
    public async Task CreateSwapTest(string regimentId, string fromChainId, string symbol, long originShare,
        long targetShare)
    {
        var result = await _bridgeService.CreateSwapAsync(new CreateSwapInput
        {
            RegimentId = Hash.LoadFromHex(regimentId),
            SwapTargetTokenList =
            {
                new SwapTargetToken
                {
                    FromChainId = fromChainId,
                    Symbol = symbol,
                    SwapRatio = new SwapRatio
                    {
                        OriginShare = originShare,
                        TargetShare = targetShare
                    }
                }
            }
        });
        result.TransactionResult.Status.ShouldBe(TransactionResultStatus.Mined);
        var swapId = SwapInfoAdded.Parser.ParseFrom(result.TransactionResult.Logs
            .First(i => i.Name == nameof(SwapInfoAdded))
            .NonIndexed).SwapId;
        _output.WriteLine(swapId.ToHex());
    }


    [Theory]
    [InlineData("4d1e49ba6df2ed3692252e24153773e87ca7c44d11ade34b4a32e73a155e7a38", 1000000000000, 1, "USDT")]
    public async Task ChangeSwapRatioTest(string swapId, long originShare, long targetShare, string symbol)
    {
        var result = await _bridgeService.ChangeSwapRatioAsync(new ChangeSwapRatioInput
        {
            SwapId = Hash.LoadFromHex(swapId),
            SwapRatio = new SwapRatio
            {
                OriginShare = originShare,
                TargetShare = targetShare
            },
            TargetTokenSymbol = symbol
        });
        result.TransactionResult.Status.ShouldBe(TransactionResultStatus.Mined);
    }

    [Theory]
    [InlineData("e1bc1ea8232404fd28e5125eae5f7f31294d340765bdbb42e27b838d641d58e3", 1000_00000000, "ELF")]
    public async Task DepositTest(string swapId, long amount, string symbol)
    {
        var result = await _bridgeService.DepositAsync(new DepositInput
        {
            SwapId = Hash.LoadFromHex(swapId),
            Amount = amount,
            TargetTokenSymbol = symbol
        });
        result.TransactionResult.Status.ShouldBe(TransactionResultStatus.Mined);
    }

    [Theory]
    [InlineData("ad7af9718a9822d4ac878d62fe5209885dc567ab12d96220732c97a79322ebab",
        "0x62449e90ca74737eb344f65022d7bbd4fec7bfc1703ad831a99a86c8545dfdee.23",
        "2BC7WWMNBp4LjmJ48VAfDocEU2Rjg5yhELxT2HewfYxPPrdxA9", "100000000")]
    public async Task SwapTokenTest(string swapId, string receiptId, string receiverAddress, string originAmount)
    {
        var result = await _bridgeService.SwapTokenAsync(new SwapTokenInput
        {
            SwapId = Hash.LoadFromHex(swapId),
            ReceiptId = receiptId,
            ReceiverAddress = Address.FromBase58(receiverAddress),
            OriginAmount = originAmount
        });
        result.TransactionResult.Status.ShouldBe(TransactionResultStatus.Mined);
        var tokenSwapped = TokenSwapped.Parser.ParseFrom(result.TransactionResult.Logs
            .First(i => i.Name == nameof(TokenSwapped))
            .NonIndexed);
        tokenSwapped.Address.ShouldBe(Address.FromBase58("J6zgLjGwd1bxTBpULLXrGVeV74tnS2n74FFJJz7KNdjTYkDF6"));
        tokenSwapped.Symbol.ShouldBe("ELF");
    }


    [Theory]
    [InlineData("e1bc1ea8232404fd28e5125eae5f7f31294d340765bdbb42e27b838d641d58e3", "ELF")]
    public async Task GetSwapPairInfoTest(string swapId, string symbol)
    {
        var result = await _bridgeService.GetSwapPairInfoAsync(new GetSwapPairInfoInput
        {
            SwapId = Hash.LoadFromHex(swapId),
            Symbol = symbol
        });
        _output.WriteLine(result.DepositAmount.ToString());
    }


    /**
     * AElf to
     */
    [Theory]
    [InlineData("Kovan", "ELF")]
    public async Task AddTokenTest(string chainId, string symbol)
    {
        var result = await _bridgeService.AddTokenAsync(new AddTokenInput
        {
            Value =
            {
                new ChainToken
                {
                    ChainId = chainId,
                    Symbol = symbol
                }
            }
        });
        result.TransactionResult.Status.ShouldBe(TransactionResultStatus.Mined);
    }

    [Theory]
    [InlineData("Kovan")]
    public async Task SetGasLimitTest(string chainId)
    {
        var gasLimit = (long) ((21000 + 68 * 3400) * 1.1);
        var result = await _bridgeService.SetGasLimitAsync(new SetGasLimitInput
        {
            GasLimitList =
            {
                new GasLimit
                {
                    ChainId = chainId,
                    GasLimit_ = gasLimit
                }
            }
        });
        result.TransactionResult.Status.ShouldBe(TransactionResultStatus.Mined);
    }

    [Theory]
    [InlineData(160, "ELF", "0xA2263D5c14F9c711A8b3C4AA2FD522Efdb5d5e44", "Goerli")]
    public async Task CreateReceiptTest(long amount, string symbol, string targetAddress, string targetChainId)
    {
        // var balance = await _tokenService.GetTokenBalanceAsync(symbol,
        //     Address.FromBase58("ZrAFaqdr79MWYkxA49Hp2LUdSVHdP6fJh3kDw4jmgC7HTgrni"));
        var result = await _bridgeService.CreateReceiptAsync(new CreateReceiptInput
        {
            Amount = amount,
            Symbol = symbol,
            TargetAddress = targetAddress,
            TargetChainId = targetChainId
        });
        result.TransactionResult.Status.ShouldBe(TransactionResultStatus.Mined);
        var receiptCreated = ReceiptCreated.Parser.ParseFrom(result.TransactionResult.Logs
            .First(i => i.Name == nameof(ReceiptCreated))
            .NonIndexed);
        _output.WriteLine(receiptCreated.ReceiptId);
        _output.WriteLine(receiptCreated.Symbol);
        _output.WriteLine(receiptCreated.TargetAddress);
        _output.WriteLine(receiptCreated.TargetChainId);
        _output.WriteLine(receiptCreated.Amount.ToString());
        // var report = ReportProposed.Parser.ParseFrom(result.TransactionResult.Logs
        //     .First(i => i.Name == nameof(ReportProposed))
        //     .NonIndexed);
        // _output.WriteLine(report.Token);
        // var balance1 = await _tokenService.GetTokenBalanceAsync(symbol,
        //     Address.FromBase58("ZrAFaqdr79MWYkxA49Hp2LUdSVHdP6fJh3kDw4jmgC7HTgrni"));
        // balance1.Balance.ShouldBe(balance.Balance - 300 - 8200000000);
    }


    [Theory]
    [InlineData("1db5c7ac518f8856dc782fbda38e8df2be710fc9430519f306b8466c50d1b978")]
    public async Task GetSpaceIdBySwapIdTest(string swapId)
    {
        var result = await _bridgeService.GetSpaceIdBySwapIdAsync(Hash.LoadFromHex(swapId));
        _output.WriteLine(result.ToHex());
    }

    [Theory]
    [InlineData("0x62449e90ca74737eb344f65022d7bbd4fec7bfc1703ad831a99a86c8545dfdee.1",
        "5d8e387180d43cc3b136a3065062e5fe18e8018f848b2791ffd02dc5390689fb")]
    public async Task GetSwapAmountsTest(string receiptId, string swapId)
    {
        var result = await _bridgeService.GetSwapAmountsAsync(new GetSwapAmountsInput
        {
            ReceiptId = receiptId,
            SwapId = Hash.LoadFromHex(swapId)
        });
        _output.WriteLine(result.Receiver.ToString());
        _output.WriteLine(result.ReceivedAmounts["ELF"].ToString());
    }

    [Theory]
    [InlineData("9a8556e72cfec1afd08beca421ada9a8d56f8102308894e209181367a6415ff9",
        "J6zgLjGwd1bxTBpULLXrGVeV74tnS2n74FFJJz7KNdjTYkDF6")]
    public async Task GetSwappedReceiptInfoTest(string swapId, string receiverAddress)
    {
        var result = await _bridgeService.GetSwappedReceiptInfoAsync(new GetSwappedReceiptInfoListInput
        {
            SwapId = Hash.LoadFromHex(swapId),
            ReceiverAddress = Address.FromBase58(receiverAddress)
        });
        _output.WriteLine(result.Value[0].ReceiptId);
        _output.WriteLine(result.Value[0].AmountMap["ELF"].ToString());
        _output.WriteLine(result.Value[0].ReceivingTime.ToString());
    }

    [Theory]
    [InlineData("Kovan")]
    public async Task GetFeeByChainIdTest(string chainId)
    {
        var result = await _bridgeService.GetFeeByChainIdAsync(new StringValue
        {
            Value = chainId
        });
        _output.WriteLine(result.Value.ToString());
    }
}