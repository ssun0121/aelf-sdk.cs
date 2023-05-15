using AElf.Types;

namespace AElf.Client.Genesis;

public interface IDeployContractService
{
    Task<Tuple<Address?, string>> DeployContract(string contractFileName);
    
    Task<Tuple<Address?, string>> UpdateContract(string contractFileName, string contractAddress);
}