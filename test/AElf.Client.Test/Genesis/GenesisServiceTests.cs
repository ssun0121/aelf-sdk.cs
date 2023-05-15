using System.Threading.Tasks;
using AElf.Client.Genesis;
using AElf.Client.Test;
using Shouldly;
using Xunit.Abstractions;

namespace AElf.Client.Abp.Test.Genesis;

[Trait("Category", "GenesisContractService")]

public sealed class GenesisServiceTests : AElfClientAbpContractServiceTestBase
{
    private readonly IGenesisService _genesisService;
    private readonly ITestOutputHelper _output;
    private readonly IDeployContractService _deployService;

    public GenesisServiceTests(ITestOutputHelper output)
    {
        _genesisService = GetRequiredService<IGenesisService>();
        _deployService = GetRequiredService<IDeployContractService>();
        _output = output;
    }

    [Theory]
    [InlineData("AElf.Contracts.Regiment")]
    public async Task DeployContract(string contractName)
    {
        var tuple = await _deployService.DeployContract(contractName);
        _output.WriteLine(tuple.Item2);
        tuple.Item1.ShouldNotBeNull();
    }

    [Theory] 
    [InlineData("AElf.Contracts.MultiToken","JRmBduh4nXWi1aXgdUsj5gJrzeZb2LxmrAbf7W99faZSvoAaE")]
    public async Task UpdateContract(string contractName,string contractAddress)
    {
        var tuple = await _deployService.UpdateContract(contractName,contractAddress);
        _output.WriteLine(tuple.Item2);
    }
}