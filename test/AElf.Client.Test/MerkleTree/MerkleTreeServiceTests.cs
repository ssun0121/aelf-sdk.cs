using System.Threading.Tasks;
using AElf.Client.MerkleTree;
using AElf.Contracts.MerkleTreeContract;
using AElf.Types;
using Shouldly;
using Xunit.Abstractions;

namespace AElf.Client.Test.MerkleTree;

[Trait("Category", "MerkleTreeContractService")]
public sealed class MerkleTreeServiceTests : AElfClientAbpContractServiceTestBase
{
    private readonly IMerkleTreeService _merkleTreeService;
    private readonly ITestOutputHelper _output;

    public MerkleTreeServiceTests(ITestOutputHelper output)
    {
        _merkleTreeService = GetRequiredService<IMerkleTreeService>();
        _output = output;
    }

    [Theory]
    [InlineData("225ajURvev5rgX8HnMJ8GjbPnRxUrCHoD7HUjhWQqewEJ5GAv1",
        "2nyC8hqq3pGnRu8gJzCsTaxXB6snfGxmL2viimKXgEfYWGtjEh")]
    public async Task InitializeContractTest(
        string owner, 
        string regimentContractAddress)
    {
        var result = await _merkleTreeService.InitializeContractAsync(new InitializeInput
        {
            Owner = Address.FromBase58(owner),
            RegimentContractAddress = Address.FromBase58(regimentContractAddress)
        });
        result.TransactionResult.Status.ShouldBe(TransactionResultStatus.Mined);
    }

    [Theory]
    [InlineData("6eb87f9487d72ab4d3db59ff610fe47bda6b589c7132fffac45c878542f31d13")]
    public async Task GetLastLeafIndexTest(string spaceId)
    {
        var result = await _merkleTreeService.GetLastLeafIndexAsync(new GetLastLeafIndexInput
        {
            SpaceId = Hash.LoadFromHex(spaceId)
        });
        _output.WriteLine(result.ToString());
    }
}