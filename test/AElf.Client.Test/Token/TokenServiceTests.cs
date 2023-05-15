using System.Linq;
using System.Threading.Tasks;
using AElf.Client.Token;
using AElf.Client.Token.SyncTokenInfo;
using AElf.Contracts.Bridge;
using AElf.Contracts.MultiToken;
using AElf.Contracts.NFT;
using AElf.Types;
using Google.Protobuf;
using Shouldly;
using Xunit.Abstractions;
using ApproveInput = AElf.Contracts.MultiToken.ApproveInput;
using CreateInput = AElf.Contracts.MultiToken.CreateInput;
using TransferInput = AElf.Contracts.MultiToken.TransferInput;

namespace AElf.Client.Test.Token;

[Trait("Category", "TokenContractService")]
public sealed class TokenServiceTests : AElfClientAbpContractServiceTestBase
{
    private readonly ITokenService _tokenService;
    private readonly ISyncTokenInfoQueueService _syncTokenInfoQueueService;
    private readonly ITestOutputHelper _output;

    public TokenServiceTests(ITestOutputHelper output)
    {
        _tokenService = GetRequiredService<ITokenService>();
        _syncTokenInfoQueueService = GetRequiredService<ISyncTokenInfoQueueService>();
        _output = output;
    }

    [Theory]
    [InlineData("USDT")]
    public async Task GetTokenInfoTest(string symbol)
    {
        var tokenInfo = await _tokenService.GetTokenInfoAsync(symbol);
        tokenInfo.Symbol.ShouldBe(symbol);
    }

    [Theory]
    [InlineData("ZVJHCVCzixThco58iqe4qnE79pmxeDuYtMsM8k71RhLLxdqB5", "ELF", 10_00000000)]
    public async Task TransferTest(string address, string symbol, long amount)
    {
        var result = await _tokenService.TransferAsync(new TransferInput
        {
            To = Address.FromBase58(address),
            Symbol = symbol,
            Amount = amount
        });
        result.TransactionResult.Status.ShouldBe(TransactionResultStatus.Mined);
        var logEvent = result.TransactionResult.Logs.First(l => l.Name == nameof(Contracts.MultiToken.Transferred));
        var transferred = new Contracts.MultiToken.Transferred();
        foreach (var indexed in logEvent.Indexed)
        {
            transferred.MergeFrom(indexed);
        }

        transferred.MergeFrom(logEvent.NonIndexed);
        transferred.Symbol.ShouldBe(symbol);
        transferred.To.ToBase58().ShouldBe(address);
        transferred.Amount.ShouldBe(amount);
    }

    [Theory]
    [InlineData("BA994198147")]
    public async Task SyncTokenInfoTest(string symbol)
    {
        _syncTokenInfoQueueService.Enqueue(symbol);
    }

    [Theory]
    [InlineData("CO429872652")]
    public async Task CrossChainCreateNFTProtocolTest(string symbol)
    {
        await _tokenService.CrossChainCreateNFTProtocolAsync(new CrossChainCreateInput
        {
            Symbol = symbol
        });
    }

    [Theory]
    [InlineData("BA417054001", "JQkVTWz5HXxEmNXzTtsAVHC7EUTeiFktzoFUu9TyA6MWngkem")]
    public async Task AddMintersTest(string symbol, string addressBase58)
    {
        var address = Address.FromBase58(addressBase58);
        await _tokenService.AddMintersAsync(new AddMintersInput
        {
            Symbol = symbol,
            MinterList = new MinterList
            {
                Value = {address}
            }
        });
    }

    [Theory]
    [InlineData("ZrAFaqdr79MWYkxA49Hp2LUdSVHdP6fJh3kDw4jmgC7HTgrni")]
    public async Task CreatePortTokenTest(string issuer)
    {
        var result = await _tokenService.CreateTokenAsync(new CreateInput
        {
            Symbol = "PORT",
            TokenName = "Port Token",
            TotalSupply = 10_00000000_00000000,
            Decimals = 8,
            Issuer = Address.FromBase58(issuer),
            IsBurnable = true
        });
        result.TransactionResult.Status.ShouldBe(TransactionResultStatus.Mined);
        var symbol = TokenCreated.Parser.ParseFrom(result.TransactionResult.Logs.First(i => i.Name == nameof(TokenCreated))
            .NonIndexed).Symbol;
        symbol.ShouldBe("PORT");
    }

    [Theory]
    [InlineData(10000000000_00000000,"USDT","225ajURvev5rgX8HnMJ8GjbPnRxUrCHoD7HUjhWQqewEJ5GAv1")]
    public async Task ApproveTokenTest(long amount,string symbol,string spender)
    {
        var result = await _tokenService.ApproveTokenAsync(new ApproveInput
        {
            Amount = long.MaxValue,
            Symbol = symbol,
            Spender = Address.FromBase58(spender)
        });
        result.TransactionResult.Status.ShouldBe(TransactionResultStatus.Mined);
    }

    // [Theory]
    // [InlineData("bb16f381b0f2e795a988285dec3a68affacdccd7d3ac2e74edc808c102efcd95", 228, "9413000000000000000000")]
    // public async Task SwapTokenTest(string swapIdHex, long receiptId, string amount)
    // {
    //     var swapId = Hash.LoadFromHex(swapIdHex);
    //     await _tokenService.SwapTokenAsync(new SwapTokenInput
    //     {
    //         SwapId = swapId,
    //         OriginAmount = amount,
    //         ReceiptId = receiptId
    //     });
    // }

    [Theory]
    [InlineData("ELF","26icJLGpnQke1PRyi3tXsT9maaHJsdFsjMHqtrSVKYVpctCqy4")]
    public async Task GetBalanceTest(string symbol,string owner)
    {
        var result = await _tokenService.GetTokenBalanceAsync(symbol, Address.FromBase58(owner));
        _output.WriteLine(result.Balance.ToString());
    }
}