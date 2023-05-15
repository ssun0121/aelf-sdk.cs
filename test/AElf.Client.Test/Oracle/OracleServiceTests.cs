using System.Linq;
using System.Threading.Tasks;
using AElf.Client.Oracle;
using AElf.Client.Regiment;
using AElf.Contracts.Oracle;
using AElf.Contracts.Regiment;
using AElf.Types;
using Shouldly;
using Xunit.Abstractions;
using AddAdminsInput = AElf.Contracts.Oracle.AddAdminsInput;
using CreateRegimentInput = AElf.Contracts.Oracle.CreateRegimentInput;
using DeleteAdminsInput = AElf.Contracts.Oracle.DeleteAdminsInput;
using DeleteRegimentMemberInput = AElf.Contracts.Oracle.DeleteRegimentMemberInput;
using InitializeInput = AElf.Contracts.Oracle.InitializeInput;

namespace AElf.Client.Test.Oracle;

[Trait("Category", "OracleContractService")]
public sealed class OracleServiceTests : AElfClientAbpContractServiceTestBase
{
    private readonly IOracleService _oracleService;
    private readonly ITestOutputHelper _output;

    public OracleServiceTests(ITestOutputHelper output)
    {
        _output = output;
        _oracleService = GetRequiredService<IOracleService>();
    }
    
    
    private Address _regimentAddress;

    [Theory]
    [InlineData("ZrAFaqdr79MWYkxA49Hp2LUdSVHdP6fJh3kDw4jmgC7HTgrni", true)]
    public async Task CreateRegimentTest(string manager, bool isApproveToJoin)
    {
        var result = await _oracleService.CreateRegimentAsync(new CreateRegimentInput
        {
            Manager = Address.FromBase58(manager),
            IsApproveToJoin = isApproveToJoin
        });
        result.TransactionResult.Status.ShouldBe(TransactionResultStatus.Mined);
        var regiment = RegimentCreated.Parser.ParseFrom(result.TransactionResult.Logs.First(i => i.Name == nameof(RegimentCreated))
            .NonIndexed);
        _output.WriteLine(regiment.RegimentId.ToHex());
        _output.WriteLine(regiment.RegimentAddress.ToBase58());
    }

    [Theory]
    [InlineData("Taikh8VFhM3qbrkC2hd5SzXFsas72gu31SbMZWmgopehdMVQE",
        "225ajURvev5rgX8HnMJ8GjbPnRxUrCHoD7HUjhWQqewEJ5GAv1")]
    public async Task AddAdminTest(string regimentAddress,string admin)
    {
        var result = await _oracleService.AddAdminsAsync(new AddAdminsInput
        {
            RegimentAddress = Address.FromBase58(regimentAddress),
            NewAdmins = {Address.FromBase58(admin)},
            //OriginSenderAddress = Address.FromBase58(manager)
        });
        _output.WriteLine(result.TransactionResult.TransactionId.ToHex());
        result.TransactionResult.Status.ShouldBe(TransactionResultStatus.Mined);
    }

    [Theory]
    [InlineData("Taikh8VFhM3qbrkC2hd5SzXFsas72gu31SbMZWmgopehdMVQE",
        "2hqsqJndRAZGzk96fsEvyuVBTAvoBjcuwTjkuyJffBPueJFrLa",
        "ZrAFaqdr79MWYkxA49Hp2LUdSVHdP6fJh3kDw4jmgC7HTgrni")]
    public async Task DeleteAdminTest(string regimentAddress,string admin,string manager)
    {
        var result = await _oracleService.DeleteAdminAsync(new DeleteAdminsInput
        {
            RegimentAddress = Address.FromBase58(regimentAddress),
            DeleteAdmins = {Address.FromBase58(admin)},
            OriginSenderAddress = Address.FromBase58(manager)
        });
        result.TransactionResult.Status.ShouldBe(TransactionResultStatus.Mined);
    }
    
    [Theory]
    [InlineData("Taikh8VFhM3qbrkC2hd5SzXFsas72gu31SbMZWmgopehdMVQE",
        "2hqsqJndRAZGzk96fsEvyuVBTAvoBjcuwTjkuyJffBPueJFrLa")]
    public async Task DeleteMemberTest(string regimentAddress,string member)
    {
        var result = await _oracleService.DeleteMemberListAsync(new DeleteRegimentMemberInput
        {
            RegimentAddress = Address.FromBase58(regimentAddress),
            DeleteMemberAddress = Address.FromBase58(member),
        });
        result.TransactionResult.Status.ShouldBe(TransactionResultStatus.Mined);
    }
}