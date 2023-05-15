using System.Linq;
using System.Threading.Tasks;
using AElf.Client.Regiment;
using AElf.Contracts.MultiToken;
using AElf.Contracts.Regiment;
using AElf.Types;
using Shouldly;
using Xunit.Abstractions;

namespace AElf.Client.Test.Regiment;

[Trait("Category", "MerkleTreeContractService")]
public sealed class RegimentServiceTests : AElfClientAbpContractServiceTestBase
{
    private readonly IRegimentService _regimentService;
    private readonly ITestOutputHelper _output;

    public RegimentServiceTests(ITestOutputHelper output)
    {
        _output = output;
        _regimentService = GetRequiredService<IRegimentService>();
    }

    private Address _regimentAddress;

    [Theory]
    [InlineData("ZrAFaqdr79MWYkxA49Hp2LUdSVHdP6fJh3kDw4jmgC7HTgrni", true)]
    public async Task CreateRegimentTest(string manager, bool isApproveToJoin)
    {
        var result = await _regimentService.CreateRegimentAsync(new CreateRegimentInput
        {
            Manager = Address.FromBase58(manager),
            IsApproveToJoin = isApproveToJoin
        });
        result.TransactionResult.Status.ShouldBe(TransactionResultStatus.Mined);
        var regiment = RegimentCreated.Parser.ParseFrom(result.TransactionResult.Logs.First(i => i.Name == nameof(RegimentCreated))
            .NonIndexed);
        _output.WriteLine(regiment.RegimentId.ToHex());
        _output.WriteLine(regiment.RegimentAddress.ToBase58());
        _regimentAddress = regiment.RegimentAddress;
    }

    [Theory]
    [InlineData("2hqsqJndRAZGzk96fsEvyuVBTAvoBjcuwTjkuyJffBPueJFrLa","ZrAFaqdr79MWYkxA49Hp2LUdSVHdP6fJh3kDw4jmgC7HTgrni")]
    public async Task AddAdminTest(string admin,string manager)
    {
        var result = await _regimentService.AddAdminAsync(new AddAdminsInput
        {
            RegimentAddress = _regimentAddress,
            NewAdmins = {Address.FromBase58(admin)},
            OriginSenderAddress = Address.FromBase58(manager)
        });
        result.TransactionResult.Status.ShouldBe(TransactionResultStatus.Mined);
    }

    [Theory]
    [InlineData("Taikh8VFhM3qbrkC2hd5SzXFsas72gu31SbMZWmgopehdMVQE")]
    public async Task GetRegimentInfoTest(string regimentAddress)
    {
        var result = await _regimentService.GetRegimentInfoAsync(Address.FromBase58(regimentAddress));
        foreach (var admin in result.Admins)
        {
            _output.WriteLine(admin.ToBase58());
        }
    }
    
    [Theory]
    [InlineData("Taikh8VFhM3qbrkC2hd5SzXFsas72gu31SbMZWmgopehdMVQE")]
    public async Task GetRegimentMemberListTest(string regimentAddress)
    {
        var result = await _regimentService.GetRegimentMemberListAsync(Address.FromBase58(regimentAddress));
        foreach (var member in result.Value)
        {
            _output.WriteLine(member.ToBase58());
        }
    }

    [Theory]
    [InlineData()]
    public async Task GetControllerTest()
    {
        var result = await _regimentService.GetControllerAsync();
        _output.WriteLine(result.ToString());
    }
}