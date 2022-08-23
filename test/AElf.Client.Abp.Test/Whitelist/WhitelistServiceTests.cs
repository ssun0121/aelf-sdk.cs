using AElf.Client.Abp.Whitelist;
using AElf.Contracts.Whitelist;
using AElf.Types;
using Google.Protobuf;
using Shouldly;

namespace AElf.Client.Abp.Test.Whitelist;

[Trait("Category", "WhitelistContractService")]
public sealed class WhitelistServiceTests : AElfClientAbpContractServiceTestBase
{
    private readonly IWhitelistService _whitelistService;
    
    private static readonly ByteString Info1 = new PriceTag(){
        Symbol = "ELF",
        Amount = 200_000000
    }.ToByteString();

    private static readonly Hash ProjectId = HashHelper.ComputeFrom("Forest");

    public WhitelistServiceTests()
    {
        _whitelistService = GetRequiredService<IWhitelistService>();
    }

    [Theory]
    [InlineData(true,"JQkVTWz5HXxEmNXzTtsAVHC7EUTeiFktzoFUu9TyA6MWngkem")]
    public async Task<Hash> CreateWhitelistTest(bool isClonable,string creator)
    {
        var extraInfoList = new ExtraInfoList
        {
            Value = {
                new ExtraInfo
                {
                    AddressList = new AddressList{Value = { Address.FromBase58("yoUaWiGzkWhMs8VMJGtZtKKV9FvM2xYnJwULPokyHnmqAoDLz") }},
                    Info = new TagInfo()
                    {
                        TagName = "INFO1",
                        Info = Info1
                    }
                }
            }
        };
        var result = await _whitelistService.CreateWhitelistAsync(new CreateWhitelistInput
        {
            ExtraInfoList = extraInfoList,
            IsCloneable = isClonable,
            Creator = Address.FromBase58(creator),
            ProjectId = ProjectId,
            StrategyType = StrategyType.Basic
        });
        result.TransactionResult.Status.ShouldBe(TransactionResultStatus.Mined);
        var whitelistId = WhitelistCreated.Parser
            .ParseFrom(result.TransactionResult.Logs.First(i => i.Name == nameof(WhitelistCreated)).NonIndexed)
            .WhitelistId;
        return whitelistId;
    }

    [Theory]
    [InlineData()]
    public async Task AddExtraInfoTest()
    {
        
    }
}